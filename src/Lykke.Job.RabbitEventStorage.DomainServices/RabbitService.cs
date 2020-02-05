using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.RabbitEventStorage.Domain;
using Lykke.Job.RabbitEventStorage.Domain.Repositories;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace Lykke.Job.RabbitEventStorage.DomainServices
{
    public class RabbitService : IRabbitService
    {
        private readonly RabbitMqManagementApiClient _rabbitMqManagementApiClient;
        private readonly IMessageRepository _messageRepository;
        private readonly string _rabbitMqConnectionString;
        private readonly ILog _log;

        public RabbitService(
            RabbitMqManagementApiClient rabbitMqManagementApiClient,
            IMessageRepository messageRepository,
            string rabbitMqConnectionString,
            ILogFactory logFactory)
        {
            _rabbitMqManagementApiClient = rabbitMqManagementApiClient;
            _messageRepository = messageRepository;
            _rabbitMqConnectionString = rabbitMqConnectionString;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IEnumerable<ExchangeEntity>> GetAllExchangesAsync()
        {
            var allExchanges = await _rabbitMqManagementApiClient.GetExchangesAsync();

            return allExchanges.Select(x => new ExchangeEntity() { Name = x.Name, Type = x.Type, });
        }

        public async Task SaveMessageAsync(RabbitMessage message)
        {
            await _messageRepository.SaveAsync(message.ExchangeName, DateTime.UtcNow, message.Payload);
        }

        public async Task<string> RestoreAsync(string exchangeName, string queueName, DateTime dateFrom, DateTime dateTo)
        {
            var allQueues = await _rabbitMqManagementApiClient.GetQueuesAsync();

            var queue = allQueues.SingleOrDefault(x => x.Name == queueName);
            
            if (queue == null)
            {
                return "Queue not found.";
            }

            var id = Guid.NewGuid().ToString();
            
            Task.Run(() => RestoreAsync(id, exchangeName, queueName, dateFrom, dateTo));

            return id;
        }

        private async Task RestoreAsync(string id, string exchangeName, string queueName, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = _rabbitMqConnectionString
                };
                
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    foreach (var date in GetDateRange(dateFrom, dateTo))
                    {
                        _log.Info($"Restoring date {date}.", new {id});
                        
                        string continuationToken = null;

                        do
                        {
                            var (cToken, messages) = await _messageRepository.GetAsync(exchangeName, date, dateFrom, dateTo, 100,
                                continuationToken);

                            var messagesArr = messages.ToArray();
                            
                            _log.Info($"Fetched {messagesArr.Length} messages.", new {id});

                            foreach (var message in messagesArr)
                            {
                                var body = Encoding.UTF8.GetBytes(message);
    
                                channel.BasicPublish("",
                                    queueName,
                                    body: body);
                            }

                            _log.Info($"Restored {messagesArr.Length} messages.", new {id});

                            continuationToken = cToken;
                        }
                        while (!string.IsNullOrEmpty(continuationToken));
                        
                        
                        _log.Info($"Done restoring date {date}.", new {id});
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "Error while restoring.", new { id });
            }
        }
        
        private static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            while (startDate.Date <= endDate.Date)
            {
                yield return startDate.Date;
                startDate = startDate.Date.AddDays(1);
            }
        }
    }
}
