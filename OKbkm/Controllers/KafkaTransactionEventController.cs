using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using OKbkm.Services;
using System.Threading.Tasks;

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

        [HttpPost("send")]
        public async Task<IActionResult> SendTransactionEvent([FromBody] TransactionEvent evt)
        {
            if (evt == null || string.IsNullOrEmpty(evt.AccountNo) || string.IsNullOrEmpty(evt.Type))
            {
                return BadRequest("Eksik veya geçersiz veri gönderildi.");
            }

            // Type'a göre topic belirle
            string topic = evt.Type switch
            {
                "Deposit" => "deposit-topic",
                "Withdraw" => "withdraw-topic",
                "Transfer-Sent" => "transfer-topic",
                "Transfer-Received" => "transfer-topic",
                _ => null
            };

            if (topic == null)
            {
                return BadRequest($"Geçersiz işlem türü: {evt.Type}");
            }

            await _kafka.SendMessageAsync(topic, evt);
            return Ok("Kafka mesajı başarıyla gönderildi.");
        }
    }
}
