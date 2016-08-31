using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using redfoodie.Models;

namespace redfoodie.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var defaultCity = context.Cities.Add(new City() {Name = "Delhi NCR" });
            var cities = new List<City>()
            {
                new City { ParentId = defaultCity.Id, Href = "/amritsar", Name = "Amritsar"},
                new City { ParentId = defaultCity.Id, Href = "/chandigarh", Name = "Chandigarh"},
                new City { ParentId = defaultCity.Id, Href = "/jaipur", Name = "Jaipur"},
                new City { ParentId = defaultCity.Id, Href = "/ludhiana", Name = "Ludhiana"},
                new City { ParentId = defaultCity.Id, Href = "/mumbai", Name = "Mumbai"},
                new City { ParentId = defaultCity.Id, Href = "/pune", Name = "Pune"}
            };

            foreach (var city in cities)
                context.Cities.Add(city);

            base.Seed(context);
        }
    }


    public sealed partial class ApplicationDbContext
    {
        public DbSet<Restaurant> Restaurants { get; private set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
