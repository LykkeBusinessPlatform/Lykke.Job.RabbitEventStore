using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Common.Log;
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
        private readonly RabbitMqSettings _settings;

        public RabbitModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue.RabbitEventStorageJob.Rabbit;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var rabbitService = x.Resolve<IRabbitService>();
                    var logFactory = x.Resolve<ILogFactory>();

                    var exchanges = rabbitService.GetAllExchangesAsync().GetAwaiter().GetResult();

                    exchanges = exchanges
                        .Where(y => _settings.ExchangeRegexps.Any(er => Regex.IsMatch(y.Name, er)));

                    if (_settings.ExludeExchangeRegexps != null && _settings.ExludeExchangeRegexps.Count > 0)
                        exchanges = exchanges
                        .Where(y => _settings.ExludeExchangeRegexps.All(er => !Regex.IsMatch(y.Name, er)));

                    var result = new List<IStartStop>();

                    result.AddRange(
                        exchanges.Select(exchange =>
                            new RabbitStorageSubscriber(
                                rabbitService,
                                _settings.ConnectionString,
                                exchange.Name,
                                logFactory)));

                    return result;
                })
                .As<List<IStartStop>>()
                .SingleInstance();
        }
    }
}
