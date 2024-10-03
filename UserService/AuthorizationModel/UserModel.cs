namespace UserService.AuthorizationModel
{
    /// <summary>
    /// Модель пользователя, используемая в бизнес-логике
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public UserRoleModel Role { get; set; }
    }
}
