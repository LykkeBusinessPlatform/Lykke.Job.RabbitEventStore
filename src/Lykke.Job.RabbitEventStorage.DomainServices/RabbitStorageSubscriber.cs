using System;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.Job.RabbitEventStorage.Domain;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.RabbitEventStorage.DomainServices
{
    public class RabbitStorageSubscriber : IStartStop
    {
        private readonly IRabbitService _rabbitService;
        private RabbitMqSubscriber<string> _subscriber;

        private readonly string _connectionString;
        private readonly string _exchangeName;
        
        private readonly ILogFactory _logFactory;

        public RabbitStorageSubscriber(
            IRabbitService rabbitService,
            string connectionString,
            string exchangeName,
            ILogFactory logFactory)
        {
            _rabbitService = rabbitService;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_connectionString,
                    _exchangeName,
                    "rabbiteventstoragejob")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<string>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        settings,
                        TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new DefaultStringDeserializer())
                .Subscribe(
                    x => _rabbitService.SaveMessageAsync(
                        new RabbitMessage
                        {
                            ExchangeName = _exchangeName,
                            Payload = x
                        }))
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        public void Dispose()
        {
            _subscriber.Dispose();
        }
    }
}
