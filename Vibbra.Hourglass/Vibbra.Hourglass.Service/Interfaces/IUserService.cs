using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDomain> Add(UserDomain user);

        Task<UserDomain> Find(int id);

        Task<UserDomain> Update(UserDomain user);
    }
}