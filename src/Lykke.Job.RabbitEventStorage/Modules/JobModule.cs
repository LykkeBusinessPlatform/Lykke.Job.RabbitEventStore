using Autofac;
using Lykke.Job.RabbitEventStorage.Services;
using Lykke.Job.RabbitEventStorage.Settings;
using Lykke.Job.RabbitEventStorage.Settings.JobSettings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.Job.RabbitEventStorage.DomainServices;
using Lykke.SettingsReader;

namespace Lykke.Job.RabbitEventStorage.Modules
{
    public class JobModule : Module
    {
        private readonly RabbitEventStorageJobSettings _settings;

        public JobModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue.RabbitEventStorageJob;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<RabbitService>()
                .As<IRabbitService>()
                .WithParameter("rabbitMqConnectionString", _settings.Rabbit.ConnectionString)
                .SingleInstance();

            builder.RegisterInstance(
                new RabbitMqManagementApiClient(
                    _settings.Rabbit.ManagementUrl,
                    _settings.Rabbit.Username,
                    _settings.Rabbit.Password));
        }
    }
}
