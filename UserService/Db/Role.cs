namespace UserService.Db
{
    /// <summary>
    /// Класс для создания таблицы ролей пользователей
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public RoleId RoleId { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Навигационное свойство, связывающее роль с таблицей пользователей
        /// </summary>
        public virtual List<User> Users { get; set; } = null!;
    }
}
