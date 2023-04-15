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
    public class ProjectServiceTest
    {
        private Mock<IBaseRepository<ProjectDomain>> _projectRepositoryMock;
        private Mock<IBaseRepository<UserDomain>> _userRepositoryMock;
        private IMapper _mapper;
        private Mock<IConfiguration> _configurationMock;
        private IProjectService _projectService;

        [SetUp]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IBaseRepository<ProjectDomain>>();
            _userRepositoryMock = new Mock<IBaseRepository<UserDomain>>();
            _configurationMock = new Mock<IConfiguration>();
            _projectService = new ProjectService(_projectRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public void Add_NullProject_ShouldThrowRequiredFieldException()
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Add(null));
        }

        [Test]
        public void Add_NullTitle_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                Description = "Test project"
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Add(project));
        }

        [Test]
        public void Add_NullDescription_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                Title = "Test project"
            };

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Add(project));
        }

        [Test]
        public async Task Add_DuplicatedTitle_ShouldThrowDuplicateItemException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                Title = "Test project",
                Description = "Test project description"
            };

            _projectRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new List<ProjectDomain> { project });

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _projectService.Add(project));
        }

        [Test]
        public async Task Add_ValidProject_ShouldReturnProject()
        {
            // Arrange
            var project = new ProjectDomain
            {
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } }
            };

            var mappedUsers = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } };
            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(mappedUsers);

            _projectRepositoryMock
                .Setup(x => x.Insert(project))
                .ReturnsAsync(project);

            _projectRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new List<ProjectDomain> { });

            // Act
            var result = await _projectService.Add(project);

            // Assert
            Assert.IsNotNull(result);
            _projectRepositoryMock.Verify(x => x.Insert(project), Times.Once);
            _projectRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }


        [Test]
        public async Task Add_ValidProjectWithNonexistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } }
            };

            var mappedUsers = new List<UserDomain>();
            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain>());

            _projectRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new List<ProjectDomain> { });

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync((ProjectDomain)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _projectService.Add(project));
        }

        [Test]
        public void Find_InvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            int invalidId = 0;

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _projectService.Find(invalidId));
        }

        [Test]
        public async Task FindAll_NoProjects_ShouldThrowNotFoundException()
        {
            // Arrange
            _projectRepositoryMock
                .Setup(x => x.Select("Users"))
                .ReturnsAsync(new List<ProjectDomain>());

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _projectService.FindAll());
        }

        [Test]
        public async Task Update_NonexistentProject_ShouldThrowNotFoundException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync((ProjectDomain)null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () => await _projectService.Update(project));
        }

        [Test]
        public void Update_NullProject_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var projectDB = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
               .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
               .ReturnsAsync(projectDB);

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Update(null));
        }

        [Test]
        public void Update_NullTitle_ShouldThrowRequiredFieldException()
        {  // Arrange
            var projectDB = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            var project = new ProjectDomain
            {
                ID = 1,
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
               .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
               .ReturnsAsync(projectDB);

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Update(project));
        }

        [Test]
        public void Update_NullDescription_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var projectDB = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync(projectDB);

            // Act & Assert
            Assert.ThrowsAsync<RequiredFieldException>(async () => await _projectService.Update(project));
        }

        [Test]
        public async Task Update_DuplicatedTitle_ShouldThrowDuplicateItemException()
        {
            // Arrange
            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description"
            };

            var existingProject = new ProjectDomain
            {
                ID = 2,
                Title = "Existing project",
                Description = "Existing project description"
            };

            _projectRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
                .ReturnsAsync(new List<ProjectDomain> { existingProject });

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync(existingProject);

            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(new List<UserDomain>());

            // Act & Assert
            Assert.ThrowsAsync<DuplicateItemException>(async () => await _projectService.Update(project));
        }

        [Test]
        public async Task Update_ValidProject_ShouldReturnProject()
        {
            // Arrange
            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } }
            };

            var existingProject = new ProjectDomain
            {
                ID = 1,
                Title = "Existing project",
                Description = "Existing project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync(existingProject);

            var mappedUsers = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } };
            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(mappedUsers);

            _projectRepositoryMock
               .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
               .ReturnsAsync(new List<ProjectDomain> { });

            // Act
            var result = await _projectService.Update(project);

            // Assert
            Assert.IsNotNull(result);
            _projectRepositoryMock.Verify(x => x.Update(existingProject), Times.Once);
            _projectRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task Find_ValidId_ShouldReturnProject()
        {
            // Arrange
            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.Find(project.ID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ID, project.ID);
        }

        [Test]
        public async Task Update_ValidProject_ShouldReturnUpdatedProject()
        {
            // Arrange
            var project = new ProjectDomain
            {
                ID = 1,
                Title = "Test project",
                Description = "Test project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } }
            };

            var existingProject = new ProjectDomain
            {
                ID = 1,
                Title = "Existing project",
                Description = "Existing project description",
                Users = new List<UserDomain> { new UserDomain { ID = 1 } }
            };

            _projectRepositoryMock
               .Setup(x => x.Select(It.IsAny<Expression<Func<ProjectDomain, bool>>>()))
               .ReturnsAsync(new List<ProjectDomain> { });

            var mappedUsers = new List<UserDomain> { new UserDomain { ID = 1 }, new UserDomain { ID = 2 } };
            _userRepositoryMock
                .Setup(x => x.Select(It.IsAny<Expression<Func<UserDomain, bool>>>()))
                .ReturnsAsync(mappedUsers);

            _projectRepositoryMock
                .Setup(x => x.SelectFirstBy(It.IsAny<Expression<Func<ProjectDomain, bool>>>(), "Users"))
                .ReturnsAsync(existingProject);

            _projectRepositoryMock
                    .Setup(x => x.Update(It.IsAny<ProjectDomain>()))
                    .Callback<ProjectDomain>(x => project = x)
                    .Returns(Task.CompletedTask);


            // Act
            var result = await _projectService.Update(project);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(project.ID, result.ID);
            Assert.AreEqual(project.Title, result.Title);
            Assert.AreEqual(project.Description, result.Description);
            Assert.AreEqual(project.Users.Count, result.Users.Count);
            _projectRepositoryMock.Verify(x => x.Update(existingProject), Times.Once);
            _projectRepositoryMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}