using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UserService.AuthorizationModel;
using UserService.Db;
using UserService.Repo;

namespace UserService.Controllers
{
    public static class RsaTools
    {
        public static RSA GetPrivateKey()
        {
            var f = File.ReadAllText("rsa/private_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(f);
            return rsa;
        }
    }


    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public UserController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }


        private static UserRole RoleIdToUserRole(RoleId id)
        {
            if (id == RoleId.Admin) return UserRole.Administrator;
            else
                return UserRole.User;
        }


        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = "Administrator, User")]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            return Ok(users);
        }


        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteUser([FromBody] string email)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Name);

            if (emailClaim.Value == email)
            {
                return BadRequest("Администратор не может удалить сам себя");
            }
            try
            {
                var userId = _userRepository.DeleteUser(email);
                return Ok(userId);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetUserId")]
        [Authorize(Roles = "Administrator, User")]
        public IActionResult GetUserId()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Name);

            var userId = _userRepository.GetUserId(emailClaim.Value);

            return Ok(userId);
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("AddAdmin")]
        public ActionResult AddAdmin([FromBody] LoginModel userLogin)
        {
            try
            {
                if (!CheckEmailFormat(userLogin.Email))
                {
                    return BadRequest("Указан некорректный адрес электронной почты");
                }
                else if (!CheckPasswordFormat(userLogin.Password))
                {
                    return BadRequest("Пароль должен соответствовать следующим критериям: " +
                    "- хотя бы 1 заглавная латинская буква," +
                    "- хотя бы 1 строчная латинская буква," +
                    "- хотя бы 1 цифра," +
                    "- без пробелов," +
                    "- иметь длину не менее 5 символов");
                }

                _userRepository.UserAdd(userLogin.Email, userLogin.Password, Db.RoleId.Admin);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("AddUser")]
        public ActionResult AddUser([FromBody] LoginModel userLogin)
        {
            try
            {
                if(!CheckEmailFormat(userLogin.Email))
                {
                    return BadRequest("Указан некорректный адрес электронной почты");
                }
                else if(!CheckPasswordFormat(userLogin.Password))
                {
                    return BadRequest("Пароль должен соответствовать следующим критериям: " +
                    "- хотя бы 1 заглавная латинская буква," +
                    "- хотя бы 1 строчная латинская буква," +
                    "- хотя бы 1 цифра," +
                    "- без пробелов," +
                    "- иметь длину не менее 5 символов");
                }

                _userRepository.UserAdd(userLogin.Email, userLogin.Password, Db.RoleId.User);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login([FromBody] LoginModel userLogin)
        {
            try
            {
                var idAndRole = _userRepository.UserCheck(userLogin.Email, userLogin.Password);

                var user = new UserModel { Email = userLogin.Email, Role = RoleIdToUserRole(idAndRole.RoleId) };

                var token = GenerateToken(user, idAndRole);

                return Ok(token);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        private string GenerateToken(UserModel user, IdAndRoleForLogin idAndRole)
        {
            var userId = idAndRole.Id;

            var key = new RsaSecurityKey(RsaTools.GetPrivateKey());

            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], 
                _config["Jwt:Audience"],
                claim,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public static bool CheckPasswordFormat(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{5,}$");
        }


        public static bool CheckEmailFormat(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z\d]+@\S+\.\S+$");
        }
    }
}
