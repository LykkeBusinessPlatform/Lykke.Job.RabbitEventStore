using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.RabbitEventStorage.AzureRepositories.Entities;
using Lykke.Job.RabbitEventStorage.Domain.Repositories;

namespace Lykke.Job.RabbitEventStorage.AzureRepositories.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private const string DateFormat = "MM.dd.yyyy";
        private const string TimestampFormat = "yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK";

        private static readonly char[] SplittingChars = { '_' };
        private readonly INoSQLTableStorage<MessageEntity> _storage;

        public MessageRepository(INoSQLTableStorage<MessageEntity> storage)
        {
            _storage = storage;
        }

        private static string GetPartitionKey(string exchangeName, DateTime date)
        {
            return $"{exchangeName}_{date.Date.ToString(DateFormat)}";
        }

        public async Task SaveAsync(string exchangeName, DateTime date, long timestamp,string messagePayload)
        {
            var message = new MessageEntity(GetPartitionKey(exchangeName, date), timestamp.ToString(TimestampFormat))
            {
                MessagePayload = messagePayload
            };

            await _storage.InsertAsync(message);
        }

        public async Task<(string ContinuationToken, IEnumerable<(string ExchangeName, string MessagePayload)> Messages)> 
            GetAsync(string exchangeName, DateTime date, int take = 100, string continuationToken = null)
        {
            var result = await _storage.GetDataWithContinuationTokenAsync(
                GetPartitionKey(exchangeName, date),
                take,
                continuationToken);

            return (result.ContinuationToken, result.Entities.Select(x =>
                (x.PartitionKey.Split(SplittingChars, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(),
                    x.MessagePayload)));
        }
    }
}
