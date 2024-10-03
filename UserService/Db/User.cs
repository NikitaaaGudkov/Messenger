namespace UserService.Db
{
    /// <summary>
    /// Класс для создания таблицы пользователей
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Электронная почта пользователя
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public byte[] Password { get; set; } = null!;

        /// <summary>
        /// Соль
        /// </summary>
        public byte[] Salt { get; set; } = null!;

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public RoleId RoleId { get; set; }

        /// <summary>
        /// Навигационное свойство, связывающее пользователя с его ролью
        /// </summary>
        public virtual Role Role { get; set; } = null!;
    }
}
