using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notificacoes.Models;
using Notificacoes.Services;

namespace Notificacoes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificacaoController : ControllerBase
    {

        private readonly ILogger<NotificacaoController> _logger;
        private readonly IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> _messageService;

        public NotificacaoController(ILogger<NotificacaoController> logger, IMessageService<ServiceBusMessage, ServiceBusReceivedMessage> messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [HttpPost(Name = "PostMessage")]
        public ActionResult<string> PostMessage([FromBody] Message msg)
        {
            _messageService.PostMessage(new ServiceBusMessage(JsonConvert.SerializeObject(msg.Content)), msg.Topic);
            return StatusCode(200, "Mensagem enviada com sucesso.");
        }

        [HttpGet(Name = "GetMessages")]
        public ActionResult<string[]> GetMessages(
            [FromQuery] string topicName,
            [FromQuery] string subscriptionName,
            [FromQuery] int number
        )
        {
            var messages = _messageService.GetMessages(number, topicName, subscriptionName);
            return StatusCode(200, messages.Select(m => m.Body.ToString()).ToList());
        }
    }
}
