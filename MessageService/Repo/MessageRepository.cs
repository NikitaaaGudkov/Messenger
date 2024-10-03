using AutoMapper;

using MessageService.Db;
using MessageService.DTO;

namespace MessageService.Repo
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(MessageContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public IEnumerable<MessageDto> ReceiveMessages(Guid consumerId)
        {
            using (_context)
            {
                var messages = _context.Messages.Where(mes => mes.ConsumerId == consumerId && mes.IsReceived == false).ToList();
                foreach (var message in messages)
                {
                    message.IsReceived = true;
                }
                _context.SaveChanges();
                return messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();
            }
        }

        public void SendMessage(MessageDto messageDto)
        {
            using (_context)
            {
                var message = new Message()
                { 
                    SenderId = messageDto.SenderId, 
                    ConsumerId = messageDto.ConsumerId, 
                    Text = messageDto.Text, 
                    IsReceived = false,
                    DateTime = messageDto.DateTime
                };

                _context.Add(message);
                _context.SaveChanges();
            }
        }
    }
}
