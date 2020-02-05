using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Lykke.Job.RabbitEventStorage.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BackupRequest
    {
        [Required]
        public string Exchange { set; get; }
        
        [Required]
        public string Queue { set; get; }
        
        [Required]
        public DateTime DateTimeFrom { set; get; }
        
        [Required]
        public DateTime DateTimeTo { set; get; }
    }
}