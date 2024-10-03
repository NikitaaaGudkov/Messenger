namespace MessageService.Db
{
    /// <summary>
    /// Класс для создания таблицы сообщений
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор отправителя сообщения
        /// </summary>
        public Guid SenderId { get; set; }
        /// <summary>
        /// Идентификатор получателя сообщения
        /// </summary>
        public Guid ConsumerId { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; } = null!;
        /// <summary>
        /// Статус доставки сообщения
        /// </summary>
        public bool IsReceived { get; set; }
        /// <summary>
        /// Время создания сообщения
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
