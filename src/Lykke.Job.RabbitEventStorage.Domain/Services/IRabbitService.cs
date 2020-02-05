using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Job.RabbitEventStorage.Domain.Services
{
    public interface IRabbitService
    {
        Task<IEnumerable<ExchangeEntity>> GetAllExchangesAsync();

        Task SaveMessageAsync(RabbitMessage message);

        Task<string> RestoreAsync(string exchangeName, string queue, DateTime dateFrom, DateTime dateTo);
    }
}
