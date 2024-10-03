using UserService.Db;

namespace UserService.AuthorizationModel
{
    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class IdAndRoleForLogin
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public RoleId RoleId { get; set; }
    }
}
