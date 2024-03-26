using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using UserService.Controllers;
using UserService.Repo;

namespace UserServiceTests
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void DeleteUser_DeletingAnExistingUser()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, "admin@mail.ru")
                });
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            string deletingUserEmail = "example@example.com";
            Guid deletingUserId = Guid.NewGuid();

            var configMock = new Mock<IConfiguration>();
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(x => x.DeleteUser(deletingUserEmail)).Returns(deletingUserId);

            UserController userController = new UserController(configMock.Object, userRepositoryMock.Object);
            userController.ControllerContext = new ControllerContext();
            userController.ControllerContext.HttpContext = context;

            var expected = deletingUserId;

            // Act
            IActionResult actual = userController.DeleteUser(deletingUserEmail);
            var okResult = actual as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expected, okResult.Value);
        }

        [TestMethod]
        public void DeleteUser_AdministratorRemovalOfHimself()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, "admin@mail.ru")
                });
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            string deletingUserEmail = "admin@mail.ru";

            var configMock = new Mock<IConfiguration>();
            var userRepositoryMock = new Mock<IUserRepository>();

            UserController userController = new UserController(configMock.Object, userRepositoryMock.Object);
            userController.ControllerContext = new ControllerContext();
            userController.ControllerContext.HttpContext = context;

            var expected = "Администратор не может удалить сам себя";

            // Act
            IActionResult actual = userController.DeleteUser(deletingUserEmail);
            var badRequestResult = actual as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(expected, badRequestResult.Value);
        }
    }
}