using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Sdk;
using Lykke.Job.RabbitEventStorage.Domain.Services;

namespace Lykke.Job.RabbitEventStorage.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly List<IRabbitStorageSubscriber> _rabbitSubscribers;

        public StartupManager(
            List<IRabbitStorageSubscriber> rabbitSubscribers)
        {
            _rabbitSubscribers = rabbitSubscribers;
        }

        public Task StartAsync()
        {
            foreach (var subscriber in _rabbitSubscribers)
            {
                subscriber.Start();
            }

            return Task.CompletedTask;
        }
    }
}
