using JetBrains.Annotations;

namespace Lykke.Job.RabbitEventStorage.Settings.JobSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitEventStorageJobSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings Rabbit { get; set; }
    }
}
