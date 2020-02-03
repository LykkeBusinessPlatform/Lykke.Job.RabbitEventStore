using Autofac;
using Common;

namespace Lykke.Job.RabbitEventStorage.Domain.Services
{
    public interface IRabbitStorageSubscriber : IStartable, IStopable
    {
        
    }
}