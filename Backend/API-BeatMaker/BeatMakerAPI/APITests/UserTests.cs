using Application;
using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using AutoMapper;
using Domain;
using FluentValidation;
using Moq;

namespace APITests
{
    public class UserTests
    {

        private UserService _userService;
        private Mock<IUserRepository> _userRepo = new Mock<IUserRepository>();
        public UserTests()
        {
            IValidator<UserDTO> validator = new UserValidator();
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<UserDTO, User>();
            }).CreateMapper();
            _userService = new UserService(_userRepo.Object, mapper, validator);
        }

        /// <summary>
        /// test if a user was created.
        /// </summary>
        /// <param name="userId_"></param>
        /// <param name="username_"></param>
        /// <param name="password_"></param>
        /// <param name="email_"></param>
        [Theory]
        [InlineData(1, "Casper", "gal12345", "adof@gg.org")]
        [InlineData(2, "Anders", "MegetSikkertKodeord", "anders@zomf.org")]
        [InlineData(3, "Magus", "jegelskerbannaner1234", "casp@zomr.org")]
        public void TestIfUserWasCreated(int userId_, string username_, string password_, string email_)
        {
            // Arrange
            User user = new User() { Id = userId_, Username = username_, Password = password_, Email = email_};
            _userRepo.Setup(x => x.GetUserByEmailOrUsername(username_)).Returns(user);

            // Act
            User userTest = _userService.GetUserByEmailOrUsername(username_);

            // Assert
            Assert.Equal(email_, userTest.Email);
        }

        /// <summary>
        /// Test if the password was updated on that user.
        /// </summary>
        [Fact]
        public void TestIfPasswordWasUpdated()
        {
            // Arrange
            User user = new User() { Id = 0, Email = "test@gmail.com", Password = "password12345", Username = "HelloBabt" };
            User userUpdate = new User() { Id = 0, Email = "test@gmail.com", Password = "password123456", Username = "HelloBabt" };
            UserDTO userDTO = new UserDTO() { Email = "test@gmail.com", Password = "password123456", Username = "HelloBabt" };
            _userRepo.Setup(x => x.UpdateUser(It.IsAny<User>())).Returns(user);
            // Act
            _userService.UpdateUserPassword(userDTO);
            // Assert
            _userRepo.Verify(r => r.UpdateUser(It.IsAny<User>()), Times.Once);
        }
        /// <summary>
        /// Test if the user was deleted.
        /// </summary>
        [Fact]
        public void TestIfUserWasDeleted()
        {
            User user = new User() { Id = 1, Username = "test", Password = "SecretPass123", Email = "ab@gmail.com" };
            _userRepo.Setup(x => x.GetUserByEmailOrUsername(user.Email)).Returns(user);
            _userRepo.Setup(x => x.DeleteUser(It.IsAny<string>()));
            // Act
            _userService.DeleteUser(user.Email);
            // Assert
            _userRepo.Verify(r => r.DeleteUser(It.IsAny<string>()), Times.Once);
        }


        /* Failure Condition Tests */

        /// <summary>
        /// Test if the validator caught something
        /// </summary>
        /// <param name="username_"></param>
        /// <param name="password_"></param>
        /// <param name="email_"></param>
        /// <param name="expected_"></param>
        [Theory]
        [InlineData("HejHans", "gg69", "smol@boy.com", typeof(ValidationException))]
        [InlineData("", "gg696969", "smol@boy.com", typeof(ValidationException))]
        [InlineData(null, "gg696969", "smol@boy.com", typeof(ValidationException))]
        [InlineData("HejHans", "", "smol@boy.com", typeof(ValidationException))]
        [InlineData("HejHans", null, "smol@boy.com", typeof(ValidationException))]
        [InlineData("HejHans", "gg696969", "", typeof(ValidationException))]
        [InlineData("HejHans", "gg696969", null, typeof(ValidationException))]
        [InlineData("HejHans", "gg696969", "smol@boy.com", typeof(ValidationException))]
        public void TestIfUserValidationFailed(string username_, string password_, string email_, Type expected_)
        {
            //Arrange
            UserDTO userDTO = new UserDTO() { Username = username_, Password = password_, Email = email_ };

            //Act & Assert
            try
            {
                _userService.CreateNewUser(userDTO);
            }
            catch (Exception e)
            {
                Assert.Equal(expected_, e.GetType());
            };

        }
    }
}