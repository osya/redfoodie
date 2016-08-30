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

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var cityNames = new List<string>()
            {
                "Delhi NCR", "Amritsar", "Chandigarh", "Jaipur", "Ludhiana", "Mumbai", "Pune"
            };

            foreach (var cityName in cityNames)
                context.Cities.Add(new City {Name=cityName});

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
