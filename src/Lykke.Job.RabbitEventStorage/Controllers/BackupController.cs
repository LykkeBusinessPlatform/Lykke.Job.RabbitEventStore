using System;
using System.Threading.Tasks;
using Lykke.Job.RabbitEventStorage.Domain.Services;
using Lykke.Job.RabbitEventStorage.DomainServices;
using Lykke.Job.RabbitEventStorage.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Job.RabbitEventStorage.Controllers
{
    [ApiController]
    [Route("api/backup")]
    public class BackupController : Controller
    {
        private readonly IRabbitService _rabbitService;

        public BackupController(
            IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }

        [HttpPost]
        public async Task<IActionResult> ApplyBackupAsync(
            [FromBody] BackupRequest model)
        {
            return Ok(await _rabbitService.RestoreAsync(
                model.Exchange,
                model.Queue,
                model.DateTimeFrom,
                model.DateTimeTo));
        }
    }
}