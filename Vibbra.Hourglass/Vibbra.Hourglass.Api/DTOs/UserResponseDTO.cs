using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Api.DTOs
{
    public class UserResponseDTO
    {
        public int ID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;
    }
}
