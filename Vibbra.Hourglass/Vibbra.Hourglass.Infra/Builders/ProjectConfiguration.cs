using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Infra.Builders
{
    public class ProjectConfiguration : IEntityTypeConfiguration<ProjectDomain>
    {
        public void Configure(EntityTypeBuilder<ProjectDomain> builder)
        {
            builder.HasKey(k => k.ID);
            builder.Property(p => p.Title).IsRequired();
            builder.Property(p => p.Description).IsRequired();
        }
    }
}
