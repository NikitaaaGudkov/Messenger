using Microsoft.EntityFrameworkCore;

namespace UserService.Db
{
    public class UserContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        private readonly string _connectionString = null!;
        public UserContext()
        {
            
        }

        public UserContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .LogTo(Console.WriteLine)
                .UseNpgsql(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.HasIndex(e => e.Email).IsUnique();

                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");

                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.Salt).HasColumnName("salt");

                entity.Property(e => e.RoleId).HasConversion<int>();
            });

            modelBuilder
                .Entity<Role>(entity =>
                {
                    entity.ToTable("roles");
                    entity.Property(e => e.RoleId).HasConversion<int>();
                });

            modelBuilder
                .Entity<Role>().HasData(
                Enum.GetValues(typeof(RoleId))
                .Cast<RoleId>()
                .Select(e => new Role()
                {
                    RoleId = e,
                    Name = e.ToString()
                }));
        }
    }
}
