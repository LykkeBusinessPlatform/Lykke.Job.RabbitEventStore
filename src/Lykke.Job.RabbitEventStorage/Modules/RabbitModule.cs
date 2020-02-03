using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.RabbitEventStorage.Domain;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.Job.RabbitEventStorage.DomainServices;
using Lykke.Job.RabbitEventStorage.Settings;
using Lykke.Job.RabbitEventStorage.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.RabbitEventStorage.Modules
{
    [UsedImplicitly]
    public class RabbitModule : Module
    {
        private readonly RabbitEventStorageJobSettings _settings;

        public RabbitModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue.RabbitEventStorageJob;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var rabbitService = x.Resolve<IRabbitService>();
                    var logFactory = x.Resolve<ILogFactory>();

                    var exchanges = rabbitService.GetAllExchangesAsync().GetAwaiter().GetResult();

                    exchanges = exchanges.Where(y => Regex.IsMatch(y.Name, _settings.Rabbit.ExchangeRegex)).ToList();

                    var result = new List<IRabbitStorageSubscriber>();
                    
                    result.AddRange(exchanges.Select(exchange => new RabbitStorageSubscriber(rabbitService,
                        _settings.Rabbit.ConnectionString, exchange.Name, logFactory)));

                    return result;
                })
                .As<List<IRabbitStorageSubscriber>>()
                .SingleInstance();
        }
    }
}