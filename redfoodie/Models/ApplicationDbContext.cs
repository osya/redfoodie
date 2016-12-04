using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace redfoodie.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        private static DbGeography CreatePoint(double latitude, double longitude)
        {
            var point = string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", longitude, latitude);
            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(point, 4326);
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var data = new Dictionary<string, Dictionary<string, Restaurant[]>>
            {
                { "Delhi NCR", new Dictionary<string, Restaurant[]>
                {
                    {"Chanakyapuri", new[] {new Restaurant {
                        UniqueName = "capital-kitchen-taj-palace-hotel-chanakyapuri",
                        Name = "Capital Kitchen - Taj Palace Hotel",
                        ImageFileName = "80x80_14925471_10157806793365085_6999532131779578556_n-capital-kitchen-taj-palace-hotel-chanakyapuri-dff0fe8246e5b1b39279454475cc1.jpg",
                        Location = CreatePoint(28.59196, 77.188775)
                    } } },
                    { "Saket", new [] {new Restaurant {
                        UniqueName = "yum-yum-cha-saket-new-delhi",
                        Name = "Yum Yum Cha",
                        ImageFileName = "80x80_review-yum-yum-cha-saket-new-delhi-32fa6ed732ce57d247948973624bfccc.jpg",
                        Location = CreatePoint(28.528877090780124, 77.21914628051763)
                    } } },
                    { "Dwarka", new [] {new Restaurant {
                        UniqueName = "biryani-blues-dwarka-new-delhi",
                        Name = "Biryani Blues",
                        ImageFileName = "80x80_11042949_611278912305865_7781689162808340120_n-biryani-blues-sector-11-67defe9453e5ac16b7cad1107155a399.jpg",
                        Location = CreatePoint(28.374221, 77.320361)
                    } } }
                } },
                { "Amritsar", null },
                { "Chandigarh", null },
                { "Jaipur", null },
                { "Ludhiana", null },
                { "Mumbai", null },
                { "Pune", null }
            };

            foreach (var item in data)
            {
                var addedCity = context.Cities.Add(new City { Name = item.Key });
                context.SaveChanges();
                if (item.Value == null) continue;
                foreach (var place in item.Value)
                {
                    var addedPlace = context.Places.Add(new Place { Name = place.Key, CityId = addedCity.Id });
                    context.SaveChanges();
                    foreach (var restaurant in place.Value)
                    {
                        restaurant.PlaceId = addedPlace.Id;
                        context.Restaurants.Add(restaurant);
                    }
                }
            }

            var rand = new Random();
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);
            userManager.Create(new ApplicationUser { UserName = "1@ya.ru", Email = "1@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1980, 1, 1), Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Satinder", Email = "2@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1911, 1, 1), ImageFileName = "146x146_47d93a09346478c900d31e1920c57bd5c627d612.jpg", Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Kalki", Email = "3@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1912, 1, 1), ImageFileName = "146x146_489b42243d3b25094806a8bad87d482981c2d519.jpg", Verified = true }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Suneha Prakash", Email = "4@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1969, 1, 1) }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "VforVictory", Email = "5@ya.ru", City = context.Cities.Local.First(), Birthday = new DateTime(1978, 1, 1), ImageFileName = "146x146_013977b1504db7471d0f2e16b9aff2a3cb47a50f.jpg" }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Raman", Email = "6@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Veggie Foodie", Email = "7@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Himani Gupta", Email = "8@ya.ru", City = context.Cities.Local.First() }, "Aa123#");
            userManager.Create(new ApplicationUser { UserName = "Gopal Duggal", Email = "9@ya.ru", City = context.Cities.Local.First() }, "Aa123#");

            foreach (var user in context.Users.Local)
            {
                foreach (var restaurant in context.Restaurants.Local)
                {
                    context.Votes.Add(new Vote { ApplicationUser = user, Restaurant = restaurant, Value = rand.NextDouble() >= 0.5 });
                }
            }

            base.Seed(context);
        }
    }


    public sealed partial class ApplicationDbContext
    {
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
