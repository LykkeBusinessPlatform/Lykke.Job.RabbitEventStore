using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.RabbitEventStorage.Domain.Repositories
{
    public interface IMessageRepository
    {
        Task SaveAsync(string exchangeName, DateTime date, string messagePayload);

        Task<(string continuationToken, IEnumerable<string> messages)>
            GetAsync(string exchangeName, DateTime date, DateTime dateFrom, DateTime dateTo, int take = 100, string continuationToken = null);
    }
}
