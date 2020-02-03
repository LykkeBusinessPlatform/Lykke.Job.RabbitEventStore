using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.RabbitEventStorage.Domain;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.Sdk;

namespace Lykke.Job.RabbitEventStorage.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly List<IRabbitStorageSubscriber> _rabbitSubscribers;

        public ShutdownManager(
            List<IRabbitStorageSubscriber> rabbitSubscribers)
        {
            _rabbitSubscribers = rabbitSubscribers;
        }

        public async Task StopAsync()
        {
            foreach (var subscriber in _rabbitSubscribers)
            {
                subscriber.Stop();
            }
        }
    }
}
