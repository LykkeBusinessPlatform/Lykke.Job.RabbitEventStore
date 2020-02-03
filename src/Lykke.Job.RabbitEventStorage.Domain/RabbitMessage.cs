namespace Lykke.Job.RabbitEventStorage.Domain
{
    public class RabbitMessage
    {
        public string ExchangeName { get; set; }

        public string Payload { get; set; }
    }
}
