using UserService.DTO;

namespace UserService.Client
{
    /// <summary>
    /// Интерфейс, позволяющий отправлять и получать сообщения
    /// </summary>
    public interface IMessageClient
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="messageDto">Сообщение</param>
        /// <returns></returns>
        public Task SendMessage(string messageServiceAddress, MessageDto messageDto);

        /// <summary>
        /// Получить сообщения
        /// </summary>
        /// <param name="consumerId">Идентификатор получателя</param>
        /// <returns>Перечисление сообщений</returns>
        public Task<IEnumerable<MessageDto>> ReceiveMessages(string messageServiceAddress, Guid consumerId);
    }
}
