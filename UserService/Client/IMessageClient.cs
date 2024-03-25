using MessageLibrary;

namespace UserService.Client
{
    public interface IMessageClient
    {
        public Task SendMessage(MessageDto messageDto);
        public Task<IEnumerable<MessageDto>> ReceiveMessages(Guid consumerId);
    }
}
