using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibbra.Hourglass.Infra.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();

        Task Rollback();
    }
}
