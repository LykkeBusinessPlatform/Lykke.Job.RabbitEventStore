using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitEventStorage.Settings.JobSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string DataConnString { get; set; }
    }
}
