using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Api.DTOs
{
    public class TimeRequestDTO
    {
        public DateTime StartedAt { get; set; }

        public DateTime EndedAt { get; set; }

        public int UserID { get; set; }

        public int ProjectID { get; set; }
    }
}
