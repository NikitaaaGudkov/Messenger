using UserService.AuthorizationModel;
using UserService.Db;

namespace UserService.Repo
{
    /// <summary>
    /// Интерфейс, позволяющий работать с базой данных пользователей
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Добавление пользователя в систему
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <param name="password">Пароль</param>
        /// <param name="roleId">Роль пользователя</param>
        public void UserAdd(string email, string password, RoleId roleId);

        /// <summary>
        /// Проверка наличия пользователя
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <param name="password">Пароль</param>
        /// <returns>Идентификатор и роль пользователя</returns>
        public IdAndRoleForLogin UserCheck(string email, string password);

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <returns>Перечисление пользователей</returns>
        public IEnumerable<UserModel> GetUsers();

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <returns>Идентификатор удаляемого пользователя</returns>
        public Guid DeleteUser(string email);

        /// <summary>
        /// Получить идентификатор пользователя
        /// </summary>
        /// <param name="email">Электронная почта</param>
        /// <returns>Идентификатор пользователя</returns>
        public Guid GetUserId(string email);

        /// <summary>
        /// Проверка наличия пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>True, если пользователь с таким ID существует, иначе - false</returns>
        public bool CheckUserById(Guid userId);
    }
}
