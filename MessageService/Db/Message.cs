namespace MessageService.Db
{
    public class Message
    {
        public int Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ConsumerId { get; set; }
        public string Text { get; set; }
        public bool IsReceived { get; set; }
        public DateTime DateTime { get; set; }
    }
}
