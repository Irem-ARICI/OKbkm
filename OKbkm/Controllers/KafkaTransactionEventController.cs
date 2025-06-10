using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using System.Threading.Tasks;
using OKbkm.Services;

namespace OKbkm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaTransactionEventController : ControllerBase
    {
        private readonly KafkaProducerService _kafka;

        public KafkaTransactionEventController(KafkaProducerService kafka)
        {
            _kafka = kafka;
        }

        /// <summary>
        /// Kafka'ya örnek işlem mesajı gönderir.
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendTransactionEvent([FromBody] TransactionEvent evt)
        {
            if (evt == null || string.IsNullOrEmpty(evt.AccountNo) || string.IsNullOrEmpty(evt.Type))
            {
                return BadRequest("Eksik veya geçersiz veri gönderildi.");
            }

            await _kafka.SendMessageAsync("deposit-topic", evt);    // topic adı buradan veriliyor
            return Ok("Kafka mesajı başarıyla gönderildi.");
        }
    }
}
