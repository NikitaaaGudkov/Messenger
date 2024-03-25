using MessageLibrary;
namespace MessageService.Repo
{
    public interface IMessageRepository
    {
        public void SendMessage(MessageDto messageDto);
        public IEnumerable<MessageDto> ReceiveMessages(Guid consumerId);
    }
}
