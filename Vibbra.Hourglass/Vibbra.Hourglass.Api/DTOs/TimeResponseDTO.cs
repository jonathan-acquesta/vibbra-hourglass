using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Api.DTOs
{
    public class TimeResponseDTO
    {
        public int ID { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.MinValue;

        public DateTime EndedAt { get; set; } = DateTime.MinValue;

        public int UserID { get; set; }

        public int ProjectID { get; set; }
    }
}
