using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.RabbitEventStorage.AzureRepositories.Entities;
using Lykke.Job.RabbitEventStorage.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.RabbitEventStorage.AzureRepositories.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private const string DateFormat = "MM.dd.yyyy";
        private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffK";

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

        public async Task SaveAsync(string exchangeName, DateTime date, string messagePayload)
        {
            var message = new MessageEntity(GetPartitionKey(exchangeName, date.Date), date.ToString(TimestampFormat))
            {
                MessagePayload = messagePayload
            };

            await _storage.InsertAsync(message);
        }

        public async Task<(string continuationToken, IEnumerable<string> messages)> 
            GetAsync(string exchangeName, DateTime date, DateTime dateFrom, DateTime dateTo, int take = 100, string continuationToken = null)
        {
            var query = new TableQuery<MessageEntity>()
                .Where(
                    TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(nameof(MessageEntity.PartitionKey), QueryComparisons.Equal,
                            GetPartitionKey(exchangeName, date)),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(nameof(MessageEntity.RowKey), QueryComparisons.GreaterThanOrEqual,
                            dateFrom.ToString(TimestampFormat))),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(MessageEntity.RowKey), QueryComparisons.LessThanOrEqual,
                        dateTo.ToString(TimestampFormat))));
            
            var result = await _storage.GetDataWithContinuationTokenAsync(
                query,
                take,
                continuationToken);

            return (result.ContinuationToken, result.Entities.Select(x => x.MessagePayload));
        }
    }
}
