using MessageService.DTO;

namespace MessageService.Repo
{
    /// <summary>
    /// Интерфейс, позволяющий работать с базой данных сообщений
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="messageDto">Объект сообщения</param>
        public void SendMessage(MessageDto messageDto);

        /// <summary>
        /// Получить сообщения
        /// </summary>
        /// <param name="consumerId">Идентификатор получателя сообщений</param>
        /// <returns>Перечисление объектов сообщений</returns>
        public IEnumerable<MessageDto> ReceiveMessages(Guid consumerId);
    }
}
