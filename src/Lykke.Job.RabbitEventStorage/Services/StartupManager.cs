using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Sdk;
using Lykke.Common;

namespace Lykke.Job.RabbitEventStorage.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly List<IStartStop> _rabbitSubscribers;

        public StartupManager(List<IStartStop> rabbitSubscribers)
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
