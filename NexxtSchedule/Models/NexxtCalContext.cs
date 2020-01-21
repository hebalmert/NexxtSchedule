using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace NexxtSchedule.Models
{
    public class NexxtCalContext : DbContext
    {
        public NexxtCalContext() : base("DefaultConnection")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Country> Countries { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Company> Companies { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Tax> Taxes { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Identification> Identifications { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.HeadText> HeadTexts { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Professional> Professionals { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Client> Clients { get; set; }

        public System.Data.Entity.DbSet<NexxtSchedule.Models.Event> Events { get; set; }
    }
}