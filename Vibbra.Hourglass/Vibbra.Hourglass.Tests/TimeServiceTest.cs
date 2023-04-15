using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Interfaces;
using Vibbra.Hourglass.Service.Services;

namespace Vibbra.Hourglass.Test
{

    [TestFixture]
    public class TimeServiceTest
    {
        private TimeService _timeService;
        private Mock<IBaseRepository<TimeDomain>> _timeRepositoryMock;
        private Mock<IBaseRepository<UserDomain>> _userRepositoryMock;
        private Mock<IBaseRepository<ProjectDomain>> _projectRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _timeRepositoryMock = new Mock<IBaseRepository<TimeDomain>>();
            _userRepositoryMock = new Mock<IBaseRepository<UserDomain>>();
            _projectRepositoryMock = new Mock<IBaseRepository<ProjectDomain>>();
            _timeService = new TimeService(_timeRepositoryMock.Object, _userRepositoryMock.Object, _projectRepositoryMock.Object);
        }

        [Test]
        public async Task Add_ShouldThrowRequiredFieldException_WhenStartedAtIsMinValue()
        {
            // Arrange
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.MinValue,
                EndedAt = DateTime.Now
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowRequiredFieldException_WhenEndedAtIsMinValue()
        {
            // Arrange
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.MinValue
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowRequiredFieldException_WhenProjectIDIsZero()
        {
            // Arrange
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 0,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowRequiredFieldException_WhenUserIDIsZero()
        {
            // Arrange
            var time = new TimeDomain
            {
                UserID = 0,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowRequiredFieldException_WhenEndDateLessThanStartDate()
        {
            // Arrange
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now.AddDays(1),
                EndedAt = DateTime.Now
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _timeService.Add(time));
        }


        [Test]
        public async Task Add_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync((UserDomain)null);
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new UserDomain());
            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync((ProjectDomain)null);
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldThrowDuplicateItemException_WhenAddingDuplicateTimePosting()
        {
            // Arrange
            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new UserDomain());
            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new ProjectDomain());
            _timeRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<TimeDomain, bool>>>()))
                .ReturnsAsync(new List<TimeDomain> { new TimeDomain() });
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _timeService.Add(time));
        }

        [Test]
        public async Task Add_ShouldCallInsertAndCommitAsync_WhenTimeIsValid()
        {
            // Arrange
            _userRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new UserDomain());
            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new ProjectDomain());
            _timeRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<TimeDomain, bool>>>()))
                .ReturnsAsync(new List<TimeDomain>());
            var time = new TimeDomain
            {
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act
            await _timeService.Add(time);

            // Assert
            _timeRepositoryMock.Verify(x => x.Insert(It.IsAny<TimeDomain>()), Times.Once);
            _timeRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task FindAllByProject_ShouldReturnList_WhenProjectHasTimePostings()
        {
            // Arrange
            var projectID = 1;
            var times = new List<TimeDomain>
    {
        new TimeDomain
        {
            UserID = 1,
            ProjectID = projectID,
            StartedAt = DateTime.Now,
            EndedAt = DateTime.Now.AddDays(1)
        },
        new TimeDomain
        {
            UserID = 2,
            ProjectID = projectID,
            StartedAt = DateTime.Now.AddDays(2),
            EndedAt = DateTime.Now.AddDays(3)
        }
    };
            _timeRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<TimeDomain, bool>>>()))
                .ReturnsAsync(times);

            // Act
            var result = await _timeService.FindAllByProject(projectID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(times.Count, result.Count);
        }

        [Test]
        public async Task FindAllByProject_ShouldThrowNotFoundException_WhenProjectHasNoTimePostings()
        {
            // Arrange
            var projectID = 1;
            _timeRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<TimeDomain, bool>>>()))
                .ReturnsAsync(new List<TimeDomain>());

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _timeService.FindAllByProject(projectID));
        }

        [Test]
        public async Task Update_ShouldThrowNotFoundException_WhenTimePostingDoesNotExist()
        {
            // Arrange
            _timeRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<TimeDomain, bool>>>()))
                .ReturnsAsync((TimeDomain)null);
            var time = new TimeDomain
            {
                ID = 1,
                UserID = 1,
                ProjectID = 1,
                StartedAt = DateTime.Now,
                EndedAt = DateTime.Now.AddDays(1)
            };

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _timeService.Update(time));
        }
    }
}