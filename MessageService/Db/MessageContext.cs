using Microsoft.EntityFrameworkCore;

namespace MessageService.Db
{
    public class MessageContext : DbContext
    {
        private string _connectionString;

        public MessageContext()
        {
            
        }

        public MessageContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .UseNpgsql(_connectionString)
            .LogTo(Console.WriteLine);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("messages_pkey");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(message => message.SenderId)
                .HasColumnName("sender_id");
                entity.Property(message => message.ConsumerId)
                .HasColumnName("consumer_id");
                entity.Property(message => message.Text)
                    .HasColumnName("text");
                entity.Property(message => message.IsReceived)
                    .HasColumnName("is_received");
                entity.Property(message => message.DateTime)
                    .HasColumnName("date_time");
            });
        }
    }
}
