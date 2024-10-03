namespace MessageService.DTO
{
    /// <summary>
    /// Сообщение пользователя, передаваемое по сети
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Идентификатор отправителя
        /// </summary>
        public Guid SenderId { get; set; }
        /// <summary>
        /// Идентификатор получателя
        /// </summary>
        public Guid ConsumerId { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; } = null!;
        /// <summary>
        /// Дата и время отправки
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
