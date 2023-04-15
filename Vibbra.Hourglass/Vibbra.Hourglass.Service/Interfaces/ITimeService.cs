using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Service.Interfaces
{
    public interface ITimeService
    {
        Task<TimeDomain> Add(TimeDomain time);

        Task<List<TimeDomain>> FindAllByProject(int projectID);

        Task<TimeDomain> Update(TimeDomain time);
    }
}