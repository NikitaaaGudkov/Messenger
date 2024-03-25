using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
                _userRepository.UserAdd(userLogin.Email, userLogin.Password, Db.RoleId.Admin);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("AddUser")]
        public ActionResult AddUser([FromBody] LoginModel userLogin)
        {
            try
            {
                _userRepository.UserAdd(userLogin.Email, userLogin.Password, Db.RoleId.User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return Ok();
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
    }
}
