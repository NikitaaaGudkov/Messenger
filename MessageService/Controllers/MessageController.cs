using MessageService.DTO;
using MessageService.Repo;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }


        [HttpPost]
        [Route("SendMessage")]
        public IActionResult SendMessage([FromBody] MessageDto messageDto)
        {
            try
            {
                _messageRepository.SendMessage(messageDto);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("ReceiveMessages")]
        public IActionResult ReceiveMessages([FromQuery] Guid consumerId)
        {
            try
            {
                var messages = _messageRepository.ReceiveMessages(consumerId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
