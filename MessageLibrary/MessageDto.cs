namespace MessageLibrary
{
    public class MessageDto
    {
        public Guid SenderId { get; set; }
        public Guid ConsumerId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
