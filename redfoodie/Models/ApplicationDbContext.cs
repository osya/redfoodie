using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using commons;
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
            foreach(var group in Commons.RestaurantGroups)
            {
                context.RestaurantGroups.Add(new RestaurantGroup { Id = Commons.GetStringId(group.Key), Name = group.Key, ImageFileName = group.Value });
            }
            context.SaveChanges();
            
            foreach (var cuisine in Commons.CuisineList)
            {
                context.Cuisines.Add(new Cuisine { Id = Commons.GetStringId(cuisine), Name = cuisine });
            }
            context.SaveChanges();

            foreach (var city in Commons.CitiesPlaces)
            {
                var cityId = Commons.GetStringId(city.Key);
                context.Cities.Add(new City { Id = cityId, Name = city.Key });
                foreach (var place in city.Value)
                {
                    context.Places.Add(new Place {Name = place, CityId = cityId});
                }
            }
            context.SaveChanges();

            var restaurants = new Dictionary<string, Dictionary<string, Restaurant[]>>
            {
                { "DelhiNCR", new Dictionary<string, Restaurant[]>
                    {
                        {
                            "Rajouri garden", new []
                            {
                                new Restaurant
                                {
                                    Name = "Qubitos - The Terrace Cafe",
                                    ImageFileName = "80x80_11817061_1621696381420078-qubitos-the-terrace-cafe-rajouri-garden-new-delhi.jpg",
                                    Location = CreatePoint(28.636326,77.12610700000005),
                                    Cuisines = new[] { context.Cuisines.Find("Chinese"), context.Cuisines.Find("European"), context.Cuisines.Find("Mexican"), context.Cuisines.Find("NorthIndian"), context.Cuisines.Find("Thai") },
                                    Groups = new[] {context.RestaurantGroups.Find("Rooftops") }
                                }
                            }
                        }
                        ,{
                            "Greater kailash (gk) 2", new []
                            {
                                new Restaurant
                                {
                                    Name = "Starbucks",
                                    ImageFileName = "80x80_150079_640496632666436-starbucks-greater-kailash-gk-2-new-delhi.jpg",
                                    Location = CreatePoint(28.535360000000004,77.24210240370485),
                                    Cuisines = new[] { context.Cuisines.Find("Cafe") },
                                    Groups = new[] {context.RestaurantGroups.Find("Cafes") }
                                }
                            }
                        }
                        ,{
                            "Safdarjung", new []
                            {
                                new Restaurant
                                {
                                    Name = "Rajinder Da Dhaba",
                                    ImageFileName = "80x80_Rajinder-rajinder-da-dhaba-safdarjung-new-delhi-2f2f7259314f2cdd48b0d5df9e200d92.jpg",
                                    Location = CreatePoint(28.56564230243836,77.19939769289022),
                                    Cuisines = new[] { context.Cuisines.Find("Mughlai"), context.Cuisines.Find("NorthIndian") },
                                    Groups = new[] {context.RestaurantGroups.Find("ButterChicken") }
                                }
                            }
                        }
                        ,{
                            "Sardar patel marg", new[]
                            {
                                new Restaurant
                                {
                                    Name = "Bukhara - ITC Maurya",
                                    ImageFileName = "80x80_photo259174768231688715-bukhara-itc-maurya-sardar-patel-marg-new-delhi.jpg",
                                    Location = CreatePoint(28.59671111966005,77.17371255693047),
                                    Cuisines = new[] { context.Cuisines.Find("NorthIndian") },
                                    Groups = new[] {context.RestaurantGroups.Find("Trending") }
                                }
                            }
                        },
                        { "gurgaon", new []
                            {
                                new Restaurant
                                {
                                    Name = "The Wine Company",
                                    ImageFileName = "80x80_850632371-the-wine-company-dlf-cyber-city-gurgaon.jpg",
                                    Location = CreatePoint(28.496189943024365, 77.08889253743439),
                                    Cuisines = new[] { context.Cuisines.Find("European"), context.Cuisines.Find("Italian") },
                                    Groups = new[] {context.RestaurantGroups.Find("Trending"), context.RestaurantGroups.Find("BestBars") }
                                }
                            }
                        },
                        { "Connaught place", new[] 
                            {
                                new Restaurant {
                                    Name = "Rodeo",
                                    ImageFileName = "80x80_IMG_20151201-rodeo-connaught-place-new-delhi.jpg",
                                    Location = CreatePoint(28.633100127829337, 77.21808030608213),
                                    Cuisines = new[] { context.Cuisines.Find("Continental"), context.Cuisines.Find("Italian"), context.Cuisines.Find("Mexican"), context.Cuisines.Find("NorthIndian"), context.Cuisines.Find("TexMex") },
                                    Groups = new[] {context.RestaurantGroups.Find("Trending"), context.RestaurantGroups.Find("MexicanMagic") }
                                }
                            }
                        },
                        { "Chanakyapuri", new[] 
                            {
                                new Restaurant
                                {
                                    Name = "Capital Kitchen - Taj Palace Hotel",
                                    ImageFileName = "80x80_14925471_10157806793365085-capital-kitchen-taj-palace-hotel-chanakyapuri.jpg",
                                    Location = CreatePoint(28.59196, 77.188775),
                                    Cuisines = new[] { context.Cuisines.Find("Asian"), context.Cuisines.Find("European"), context.Cuisines.Find("NorthIndian") }
                                }
                            }
                        },
                        { "Saket", new [] 
                            {
                                new Restaurant
                                {
                                    Name = "Big Chill",
                                    ImageFileName = "80x80_IMG_20150503_151819739-big-chill-saket-new-delhi-f768d40df32020adb9b502c674a7597b.jpg",
                                    Location = CreatePoint(28.528271920975754,77.21789698482507),
                                    Cuisines = new [] { context.Cuisines.Find("Continental"), context.Cuisines.Find("Desserts"), context.Cuisines.Find("Italian") },
                                    Groups = new[] {context.RestaurantGroups.Find("Trending") }
                                },
                                new Restaurant
                                {
                                    Name = "Yum Yum Cha",
                                    ImageFileName = "80x80_review-yum-yum-cha-saket-new-delhi-32fa6ed732ce57d247948973624bfccc.jpg",
                                    Location = CreatePoint(28.528877090780124, 77.21914628051763),
                                    Cuisines = new [] { context.Cuisines.Find("Chinese"), context.Cuisines.Find("Japanese") }
                                }
                            }
                        },
                        { "Dwarka", new [] 
                            {
                                new Restaurant
                                {Name = "Indus Express - Vivanta by Taj",
                                    ImageFileName = "80x80_Indus_Express_4x3-03-indus-express-vivanta-by-taj-dwarka-new-delhi.jpg",
                                    Location = CreatePoint(28.558566986896484,77.06312557476463),
                                    Cuisines = new [] { context.Cuisines.Find("Mughlai"), context.Cuisines.Find("North Indian") },
                                    Groups = new[] { context.RestaurantGroups.Find("NewlyOpened"), context.RestaurantGroups.Find("15Off") }
                                },
                                new Restaurant
                                {
                                    Name = "Biryani Blues",
                                    ImageFileName = "80x80_11042949_611278912305865-biryani-blues-sector-11.jpg",
                                    Location = CreatePoint(28.374221, 77.320361),
                                    Cuisines = new [] { context.Cuisines.Find("Biryani"), context.Cuisines.Find("Hyderabadi") }
                                }
                            }
                        }
                    }
                }
            };

            foreach (var item in restaurants)
            {
                var city = context.Cities.Find(item.Key);
                if (item.Value == null) continue;
                foreach (var place in item.Value)
                {
                    var placeEnt = context.Places.FirstOrDefault(p => string.Equals(p.Name, place.Key) && p.CityId == city.Id);
                    if (placeEnt == null) continue;
                    placeEnt.CityId = city?.Id;
                    foreach (var restaurant in place.Value)
                    {
                        restaurant.PlaceId = placeEnt.Id;
                        context.Restaurants.Add(restaurant);
                    }
                }
            }

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
                    if (Commons.Rnd.NextDouble() >= 0.5)
                    {
                        context.Votes.Add(new Vote
                        {
                            ApplicationUser = user,
                            Restaurant = restaurant,
                            Value = Commons.Rnd.NextDouble() >= 0.5,
                            ReviewText = $"Some review of {restaurant.Name} by {user.UserName}"
                        });
                    }
                }
            }
            context.SaveChanges();

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
        public DbSet<Cuisine> Cuisines { get; set; }
        public DbSet<RestaurantGroup> RestaurantGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
