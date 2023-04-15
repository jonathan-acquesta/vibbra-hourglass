using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Services;

namespace Vibbra.Hourglass.Test
{
    [TestFixture]
    public class UserServiceTest
    {
        private UserService _userService;
        private Mock<IBaseRepository<UserDomain>> _userRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IBaseRepository<UserDomain>>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Test]
        public async Task Add_ValidUser_ShouldInsertUser()
        {
            // Arrange
            var user = new UserDomain
            {
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            _userRepositoryMock
                .Setup(x => x.Insert(user))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain> { });

            // Act
            var result = await _userService.Add(user);

            // Assert
            Assert.AreEqual(user, result);
            _userRepositoryMock.Verify(x => x.Insert(user), Times.Once);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void Add_UserWithRequiredFieldNotFilled_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var user = new UserDomain
            {
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com"
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _userService.Add(user));
            _userRepositoryMock.Verify(x => x.Insert(user), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public void Add_UserWithEmailDuplicated_ShouldThrowDuplicateItemException()
        {
            // Arrange
            var user = new UserDomain
            {
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain> { user });

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _userService.Add(user));
            _userRepositoryMock.Verify(x => x.Insert(user), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public async Task Find_UserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.Find(user.ID);

            // Assert
            Assert.AreEqual(user, result);
            _userRepositoryMock.Verify(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()), Times.Once);
        }

        [Test]
        public void Find_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            int userId = 1;

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync((UserDomain)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _userService.Find(userId));
            _userRepositoryMock.Verify(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()), Times.Once);
        }

        [Test]
        public async Task Update_UserExistsAndValidFields_ShouldUpdateUser()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            var updatedUser = new UserDomain
            {
                ID = user.ID,
                Name = "Jane Doe",
                Login = "janedoe",
                Email = "janedoe@example.com",
                Password = "newpassword"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.Update(It.IsAny<UserDomain>()))
                .Callback<UserDomain>(u => user = u)
                .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain> { });

            // Act
            var result = await _userService.Update(updatedUser);

            // Assert
            Assert.AreEqual(updatedUser.Name, user.Name);
            Assert.AreEqual(updatedUser.Login, user.Login);
            Assert.AreEqual(updatedUser.Email, user.Email);
            Assert.AreEqual(updatedUser.Password, user.Password);
            _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void Update_UserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync((UserDomain)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _userService.Update(user));
            _userRepositoryMock.Verify(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()), Times.Once);
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserDomain>()), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public void Update_UserWithRequiredFieldNotFilled_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _userService.Update(user));
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserDomain>()), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public void Update_UserWithEmailDuplicated_ShouldThrowDuplicateItemException()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            var duplicatedUser = new UserDomain
            {
                ID = 2,
                Name = "Jane Doe",
                Login = "janedoe",
                Email = "johndoe@example.com",
                Password = "newpassword"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                    .ReturnsAsync(duplicatedUser);

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain> { user });

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _userService.Update(user));
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserDomain>()), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public void Update_UserWithLoginDuplicated_ShouldThrowDuplicateItemException()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Name = "John Doe",
                Login = "johndoe",
                Email = "johndoe@example.com",
                Password = "password"
            };

            var duplicatedUser = new UserDomain
            {
                ID = 2,
                Name = "Jane Doe",
                Login = "johndoe",
                Email = "janedoe@example.com",
                Password = "newpassword"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(duplicatedUser);

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain> { user });

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _userService.Update(user));
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserDomain>()), Times.Never);
            _userRepositoryMock.Verify(x => x.CommitAsync(), Times.Never);
        }
    }
}