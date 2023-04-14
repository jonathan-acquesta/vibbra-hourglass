using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(string, UserDomain)> Authentication(UserDomain user);
    }
}
