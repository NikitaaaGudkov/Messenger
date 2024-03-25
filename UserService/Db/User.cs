namespace UserService.Db
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public RoleId RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
