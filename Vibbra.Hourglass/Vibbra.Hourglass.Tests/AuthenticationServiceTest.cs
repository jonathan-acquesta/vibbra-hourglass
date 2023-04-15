using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Vibbra.Hourglass.Api.MapperConfig;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Services;

namespace Vibbra.Hourglass.Test
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private Mock<IBaseRepository<UserDomain>> _userRepositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private IMapper _mapper;

        private AuthenticationService _authenticationService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IBaseRepository<UserDomain>>();
            _configurationMock = new Mock<IConfiguration>();
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(typeof(ProfileMapperConfiguration))));

            _authenticationService = new AuthenticationService(_mapper, _userRepositoryMock.Object, _configurationMock.Object);
        }

        [Test]
        public void Authentication_InvalidUser_ShouldThrowInvalidPasswordException()
        {
            // Arrange
            var user = new UserDomain
            {
                Login = "",
                Password = ""
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidPasswordException>(async () => await _authenticationService.Authentication(user));
        }

        [Test]
        public void Authentication_UserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var user = new UserDomain
            {
                Login = "johndoe",
                Password = "password"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync((UserDomain)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _authenticationService.Authentication(user));
            _userRepositoryMock.Verify(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()), Times.Once);
        }

        [Test]
        public async Task Authentication_ValidUser_ShouldReturnTokenAndUser()
        {
            // Arrange
            var user = new UserDomain
            {
                ID = 1,
                Login = "johndoe",
                Password = "password",
                Email = "johndoe@example.com"
            };

            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(user);

            var jwtSecretMock = new Mock<IConfigurationSection>();
            jwtSecretMock.Setup(x => x.Value).Returns("jwtsecrettestkeywithminsizeneeded");

            _configurationMock
                .Setup(x => x.GetSection("JwtSecret"))
                .Returns(jwtSecretMock.Object);

            // Act
            var result = await _authenticationService.Authentication(user);

            // Assert
            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            _userRepositoryMock.Verify(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()), Times.Once);
        }
    }
    
}