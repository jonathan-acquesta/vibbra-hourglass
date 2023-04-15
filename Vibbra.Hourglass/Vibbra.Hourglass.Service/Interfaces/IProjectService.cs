using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Service.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDomain> Add(ProjectDomain project);

        Task<ProjectDomain> Find(int id);

        Task<List<ProjectDomain>> FindAll();

        Task<ProjectDomain> Update(ProjectDomain project);
    }
}