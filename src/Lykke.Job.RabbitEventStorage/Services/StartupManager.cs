using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.RabbitEventStorage.Domain;
using Lykke.Job.RabbitEventStorage.Settings.JobSettings;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Sdk;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Newtonsoft.Json.Linq;

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

        public async Task StartAsync()
        {
            foreach (var subscriber in _rabbitSubscribers)
            {
                subscriber.Start();
            }
        }
    }
}
