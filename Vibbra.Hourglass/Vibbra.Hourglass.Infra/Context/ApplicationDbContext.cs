using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Builders;

namespace Vibbra.Hourglass.Infra.Context
{
    public class ApplicationDbContext : DbContext
    {
        #region DbSet

        public virtual DbSet<UserDomain> Users { get; set; }

        public virtual DbSet<ProjectDomain> Project { get; set; }

        public virtual DbSet<TimeDomain> Times { get; set; }

        #endregion

        #region Constructor

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
            Database.Migrate();
        }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDomain>(new UserConfiguration().Configure);
            modelBuilder.Entity<ProjectDomain>(new ProjectConfiguration().Configure);
            modelBuilder.Entity<TimeDomain>(new TimeConfiguration().Configure);
           
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
