using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitEventStorage.Settings.JobSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ManagementUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public List<string> ExchangeRegexps { set; get; }

        [Optional]
        public List<string> ExludeExchangeRegexps { set; get; }
    }
}
