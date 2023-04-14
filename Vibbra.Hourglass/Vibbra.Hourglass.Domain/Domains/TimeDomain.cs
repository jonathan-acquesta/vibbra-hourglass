using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Base;

namespace Vibbra.Hourglass.Domain.Domains
{
    public class TimeDomain : BaseDomain
    {
        public int ID { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime EndedAt { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public UserDomain User { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public ProjectDomain Project { get; set; }
    }
}
