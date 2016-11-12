using System.Collections.Generic;

namespace redfoodie.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var defaultCity = context.Cities.Add(new City { Name = "Delhi NCR" });
            var cities = new List<City>
            {
                new City { ParentId = defaultCity.Id, Name = "Amritsar"},
                new City { ParentId = defaultCity.Id, Name = "Chandigarh"},
                new City { ParentId = defaultCity.Id, Name = "Jaipur"},
                new City { ParentId = defaultCity.Id, Name = "Ludhiana"},
                new City { ParentId = defaultCity.Id, Name = "Mumbai"},
                new City { ParentId = defaultCity.Id, Name = "Pune"}
            };

            foreach (var city in cities)
                context.Cities.Add(city);

            base.Seed(context);
        }
    }


    public sealed partial class ApplicationDbContext
    {
        public DbSet<Restaurant> Restaurants { get; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
