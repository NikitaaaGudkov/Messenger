using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserService.AuthorizationModel;
using UserService.Db;
using UserService.Repo;

namespace UserService.Controllers
{
    public static class RSATools
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
    public class UserController(IConfiguration config, IUserRepository userRepository, IDistributedCache cache) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IDistributedCache _cache = cache;

        private static UserRoleModel RoleIdToUserRole(RoleId id)
        {
            if (id == RoleId.Admin) return UserRoleModel.Administrator;
            else
                return UserRoleModel.User;
        }


        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = "Administrator, User")]
        public IActionResult GetUsers()
        {
            if(TryGetValue("users", out IEnumerable<UserModel> users))
            {
                return Ok(users);
            }
            else
            {
                users = _userRepository.GetUsers();
                return Ok(users);
            }
        }


        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteUser([FromBody] string email)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

            var emailClaim = claimsIdentity!.FindFirst(ClaimTypes.Name);

            if (emailClaim!.Value == email)
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

            var emailClaim = claimsIdentity!.FindFirst(ClaimTypes.Name);

            var userId = _userRepository.GetUserId(emailClaim!.Value);

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
                _cache.Remove("authors");
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

            var key = new RsaSecurityKey(RSATools.GetPrivateKey());

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
#pragma warning disable SYSLIB1045 // Преобразовать в "GeneratedRegexAttribute".
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{5,}$");
#pragma warning restore SYSLIB1045 // Преобразовать в "GeneratedRegexAttribute".
        }


        public static bool CheckEmailFormat(string email)
        {
#pragma warning disable SYSLIB1045 // Преобразовать в "GeneratedRegexAttribute".
            return Regex.IsMatch(email, @"^[a-zA-Z\d]+@\S+\.\S+$");
#pragma warning restore SYSLIB1045 // Преобразовать в "GeneratedRegexAttribute".
        }

        public T? GetData<T>(string key)
        {
            var value = _cache.GetString(key);
            if(!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public void SetData<T>(string key, T value)
        {
            string jsonString = JsonSerializer.Serialize(value);
            _cache.SetString(key, jsonString);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var data = GetData<T>(key);
            if(data == null)
            {
                value = default!;
                return false;
            }
            else
            {
                value = data;
                return true;
            }
        }
    }
}
