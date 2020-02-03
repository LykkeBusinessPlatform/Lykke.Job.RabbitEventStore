using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common;
using Lykke.Sdk;

namespace Lykke.Job.RabbitEventStorage.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly List<IStartStop> _rabbitSubscribers;

        public ShutdownManager(
            List<IStartStop> rabbitSubscribers)
        {
            _rabbitSubscribers = rabbitSubscribers;
        }

        public Task StopAsync()
        {
            foreach (var subscriber in _rabbitSubscribers)
            {
                subscriber.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
