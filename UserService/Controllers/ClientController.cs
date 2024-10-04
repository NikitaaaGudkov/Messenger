using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Client;
using UserService.DTO;
using UserService.Repo;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController(IMessageClient messageClient, IUserRepository userRepository, IConfiguration config) : ControllerBase
    {
        private readonly IMessageClient _messageClient = messageClient;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _config = config;

        [HttpGet]
        [Route("GetMessages")]
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> GetMessages()
        {
            string messageServiceAddress = _config.GetConnectionString("message_service_address")!;

            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            var idClaim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);

            var clientId = Guid.Parse(idClaim!.Value);

            var getMessagesTask = _messageClient.ReceiveMessages(messageServiceAddress, clientId);

            var messages = await getMessagesTask;

            return Ok(messages);
        }


        [HttpPost]
        [Route("AddMessage")]
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> AddMessages(Guid consumerId, string text)
        {
            string messageServiceAddress = _config.GetConnectionString("message_service_address")!;
            try
            {
                if (_userRepository.CheckUserById(consumerId))
                {
                    var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

                    var idClaim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);

                    var senderId = Guid.Parse(idClaim!.Value);

                    var messageDto = new MessageDto()
                    {
                        SenderId = senderId,
                        ConsumerId = consumerId,
                        Text = text,
                        DateTime = DateTime.Now
                    };

                    var sendMessagesTask = _messageClient.SendMessage(messageServiceAddress, messageDto);

                    await sendMessagesTask;

                    return Ok();
                }
                return BadRequest($"Пользователя с id \"{consumerId}\" в базе нет");    
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
