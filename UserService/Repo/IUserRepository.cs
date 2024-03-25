using UserService.AuthorizationModel;
using UserService.Db;

namespace UserService.Repo
{
    public interface IUserRepository
    {
        public void UserAdd(string email, string password, RoleId roleId);
        public IdAndRoleForLogin UserCheck(string email, string password);
        public IEnumerable<UserModel> GetUsers();
        public Guid DeleteUser(string email);
        public Guid GetUserId(string email);
    }
}
