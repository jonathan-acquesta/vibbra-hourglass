using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Api.DTOs
{
    public class ProjectRequestPutDTO
    {
        public int ID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<int> Users { get; set; } = new List<int>();
    }
}
