using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.CrossCutting;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Interfaces;

namespace Vibbra.Hourglass.Service.Services
{
    public class ProjectService : IProjectService
    {
        #region Fields

        private readonly IBaseRepository<ProjectDomain> _projectRepository;

        private readonly IBaseRepository<UserDomain> _userRepository;

        #endregion

        #region Constructor

        public ProjectService(
            IBaseRepository<ProjectDomain> projectRepository, IBaseRepository<UserDomain> userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Methods

        public async Task<ProjectDomain> Add(ProjectDomain project)
        {
            if (project == null || String.IsNullOrEmpty(project.Title) || String.IsNullOrEmpty(project.Description))
                throw new RequiredFieldException("Required fields not filled in");

            await ValidateTitleDuplicatedNew(project);

            project.Users = await MapUsers(project);

            var result = await _projectRepository.Insert(project);

            await _projectRepository.CommitAsync();

            return result;
        }

        private async Task<IList<UserDomain>> MapUsers(ProjectDomain project)
        {
            var users = await _userRepository.Select(x => project.Users.Select(z => z.ID).Contains(x.ID));
            if (project.Users.Count > users.Count)
            {
                throw new NotFoundException("User not found");
            }

            return users;
        }

        public async Task<ProjectDomain> Find(int id)
        {
            var projectDB = await _projectRepository.SelectFirstBy(x => x.ID == id, "Users");

            if (projectDB == null)
                throw new NotFoundException("Project not found.");

            return projectDB;
        }

        public async Task<List<ProjectDomain>> FindAll()
        {
            var projectsDB = (await _projectRepository.Select("Users")).ToList();

            if (projectsDB.Count == 0)
                throw new NotFoundException("No projects found.");

            return projectsDB;
        }

        public async Task<ProjectDomain> Update(ProjectDomain project)
        {
            var projectOnDb = await _projectRepository.SelectFirstBy(x => x.ID == project.ID, "Users");

            if (projectOnDb == null)
                throw new NotFoundException("Could not find the project to update");

            if (project == null || String.IsNullOrEmpty(project.Title) || String.IsNullOrEmpty(project.Description))
                throw new RequiredFieldException("Required fields not filled in");

            projectOnDb.Title = project.Title;
            projectOnDb.Description = project.Description;

            projectOnDb.Users = await MapUsers(project); //Todo: Questionar a dinamica de atualização dos usuários no projeto

            await ValidateTitleDuplicatedUpdate(project);

            await _projectRepository.Update(projectOnDb);

            await _projectRepository.CommitAsync();

            return projectOnDb;
        }

        private async Task ValidateTitleDuplicatedNew(ProjectDomain project)
        {
            var projectDB = await _projectRepository
               .Select(x => x.Title == project.Title);

            if (projectDB.Count > 0)
                throw new DuplicateItemException("Title already registered");
        }

        private async Task ValidateTitleDuplicatedUpdate(ProjectDomain project)
        {
            var projectDB = await _projectRepository
               .Select(x => x.Title == project.Title && x.ID != project.ID);

            if (projectDB.Count > 0)
                throw new DuplicateItemException("Title already registered");
        }

        #endregion
    }
}
