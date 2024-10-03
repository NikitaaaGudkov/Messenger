namespace UserService.AuthorizationModel
{
    /// <summary>
    /// Данные пользователя для входа в приложения
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
