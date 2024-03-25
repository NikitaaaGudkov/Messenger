using UserService.Db;

namespace UserService.AuthorizationModel
{
    public class IdAndRoleForLogin
    {
        public Guid Id { get; set; }
        public RoleId RoleId { get; set; }
    }
}
