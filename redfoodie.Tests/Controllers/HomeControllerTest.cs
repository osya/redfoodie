using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using commons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using redfoodie.Controllers;
using redfoodie.Models;
using Moq;

namespace redfoodie.Tests.Controllers
{
    // TODO: Implement copying or generating database for testing
    [TestClass]
    public class HomeControllerTest
    {
        private static readonly List<City> Cities = new List<City>();
        private static readonly List<Place> Places = new List<Place>();

        public HomeControllerTest()
        {
            foreach (var city in Commons.CitiesPlaces)
            {
                var cityId = Commons.GetStringId(city.Key);
                Cities.Add(new City { Id = cityId, Name = city.Key });
                foreach (var place in city.Value)
                {
                    Places.Add(new Place { Name = place, CityId = cityId, Restaurants = new List<Restaurant>() });
                }
            }
        }

        private class MockHttpSession : HttpSessionStateBase
        {
            private readonly Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

            public override object this[string name]
            {
                get { return _sessionDictionary[name]; }
                set { _sessionDictionary[name] = value; }
            }
        }

        private static ApplicationDbContext Db {
            get
            {
                // Create a list of users
                var usersQueriableList = new[]
                {
                    new ApplicationUser
                    {
                        UserName = "1@ya.ru",
                        Email = "1@ya.ru",
                        Birthday = new DateTime(1980, 1, 1),
                        Verified = true,
                    },
                    new ApplicationUser
                    {
                        UserName = "Satinder",
                        Email = "2@ya.ru",
                        Birthday = new DateTime(1911, 1, 1),
                        ImageFileName = "146x146_47d93a09346478c900d31e1920c57bd5c627d612.jpg",
                        Verified = true
                    },
                    new ApplicationUser
                    {
                        UserName = "Kalki",
                        Email = "3@ya.ru",
                        Birthday = new DateTime(1912, 1, 1),
                        ImageFileName = "146x146_489b42243d3b25094806a8bad87d482981c2d519.jpg",
                        Verified = true
                    },
                    new ApplicationUser
                    {
                        UserName = "Suneha Prakash",
                        Email = "4@ya.ru",
                        Birthday = new DateTime(1969, 1, 1)
                    }
                }.AsQueryable();

                foreach (var user in usersQueriableList)
                {
                    user.Votes = new List<Vote> { new Vote { UserId = user.Id, Value = Commons.Rnd.Next(100) < 50 } };
                }

                // Force DbSet to return the IQueryable members of our converted list object as its data source
                var mockUsers = new Mock<DbSet<ApplicationUser>>();
                mockUsers.As<IDbAsyncEnumerable<ApplicationUser>>().Setup(m => m.GetAsyncEnumerator())
                    .Returns(new AsyncEnumerator<ApplicationUser>(usersQueriableList.GetEnumerator()));
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider)
                    .Returns(new AsyncQueryProvider<ApplicationUser>(usersQueriableList.Provider));
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(usersQueriableList.Expression);
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(usersQueriableList.ElementType);
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(usersQueriableList.GetEnumerator());

                // Create a list of cities
                var citiesQueriableList = Cities.AsQueryable();

                // Force DbSet to return the IQueryable members of our converted list object as its data source
                var mockCities = new Mock<DbSet<City>>();
                mockCities.As<IDbAsyncEnumerable<City>>().Setup(m => m.GetAsyncEnumerator())
                    .Returns(new AsyncEnumerator<City>(citiesQueriableList.GetEnumerator()));
                mockCities.As<IQueryable<City>>().Setup(m => m.Provider)
                    .Returns(new AsyncQueryProvider<City>(citiesQueriableList.Provider));
                mockCities.As<IQueryable<City>>().Setup(m => m.Expression).Returns(citiesQueriableList.Expression);
                mockCities.As<IQueryable<City>>().Setup(m => m.ElementType).Returns(citiesQueriableList.ElementType);
                mockCities.As<IQueryable<City>>().Setup(m => m.GetEnumerator()).Returns(citiesQueriableList.GetEnumerator());
//                mockCities.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => Cities.FirstOrDefault(d => Equals(d.Id, ids[0])));

                // Create a list of places
                var placesQueriableList = Places.AsQueryable();

                // Force DbSet to return the IQueryable members of our converted list object as its data source
                var mockPlaces = new Mock<DbSet<Place>>();
                mockPlaces.As<IDbAsyncEnumerable<Place>>().Setup(m => m.GetAsyncEnumerator())
                    .Returns(new AsyncEnumerator<Place>(placesQueriableList.GetEnumerator()));
                mockPlaces.As<IQueryable<Place>>().Setup(m => m.Provider)
                    .Returns(new AsyncQueryProvider<Place>(placesQueriableList.Provider));
                mockPlaces.As<IQueryable<Place>>().Setup(m => m.Expression).Returns(placesQueriableList.Expression);
                mockPlaces.As<IQueryable<Place>>().Setup(m => m.ElementType).Returns(placesQueriableList.ElementType);
                mockPlaces.As<IQueryable<Place>>().Setup(m => m.GetEnumerator()).Returns(placesQueriableList.GetEnumerator());

//                // Create a list of cuisines
//                var cuisinesQueriableList = Commons.CuisineList.Select(c => new Cuisine { Id = Commons.GetStringId(c), Name = c }).AsQueryable();
//
//                // Force DbSet to return the IQueryable members of our converted list object as its data source
//                var mockCuisines = new Mock<DbSet<Cuisine>>();
//                mockCuisines.As<IDbAsyncEnumerable<Cuisine>>().Setup(m => m.GetAsyncEnumerator())
//                    .Returns(new AsyncEnumerator<Cuisine>(cuisinesQueriableList.GetEnumerator()));
//                mockCuisines.As<IQueryable<Cuisine>>().Setup(m => m.Provider)
//                    .Returns(new AsyncQueryProvider<Cuisine>(cuisinesQueriableList.Provider));
//                mockCuisines.As<IQueryable<Cuisine>>().Setup(m => m.Expression).Returns(cuisinesQueriableList.Expression);
//                mockCuisines.As<IQueryable<Cuisine>>().Setup(m => m.ElementType).Returns(cuisinesQueriableList.ElementType);
//                mockCuisines.As<IQueryable<Cuisine>>().Setup(m => m.GetEnumerator()).Returns(cuisinesQueriableList.GetEnumerator());
//
//                // Create a list of restaurant groups
//                var restaurantGroupsQueriableList = Commons.RestaurantGroups.Select(g => new RestaurantGroup { Id = Commons.GetStringId(g.Key), Name = g.Key, ImageFileName = g.Value }).AsQueryable();
//
//                // Force DbSet to return the IQueryable members of our converted list object as its data source
//                var mockRestaurantGroups = new Mock<DbSet<RestaurantGroup>>();
//                mockRestaurantGroups.As<IDbAsyncEnumerable<RestaurantGroup>>().Setup(m => m.GetAsyncEnumerator())
//                    .Returns(new AsyncEnumerator<RestaurantGroup>(restaurantGroupsQueriableList.GetEnumerator()));
//                mockRestaurantGroups.As<IQueryable<RestaurantGroup>>().Setup(m => m.Provider)
//                    .Returns(new AsyncQueryProvider<RestaurantGroup>(restaurantGroupsQueriableList.Provider));
//                mockRestaurantGroups.As<IQueryable<RestaurantGroup>>().Setup(m => m.Expression).Returns(restaurantGroupsQueriableList.Expression);
//                mockRestaurantGroups.As<IQueryable<RestaurantGroup>>().Setup(m => m.ElementType).Returns(restaurantGroupsQueriableList.ElementType);
//                mockRestaurantGroups.As<IQueryable<RestaurantGroup>>().Setup(m => m.GetEnumerator()).Returns(restaurantGroupsQueriableList.GetEnumerator());

//                return new ApplicationDbContext { Users = mockUsers.Object, Cities = mockCities.Object, Places = mockPlaces.Object, Cuisines = mockCuisines.Object, RestaurantGroups = mockRestaurantGroups.Object };
                return null;
            }
        }

//        private static ControllerContext ControllerContext
//        {
//            get
//            {
//                var request = new Mock<HttpRequestBase>();
//                request.Setup(x => x.IsAuthenticated).Returns(false);
//                var httpContext = new Mock<HttpContextBase>();
//                httpContext.Setup(x => x.Request).Returns(request.Object);
//
//                var session = new MockHttpSession();
//                httpContext.Setup(x => x.Session).Returns(session);
//
//                var user = new Mock<IPrincipal>();
//                var identity = new Mock<IIdentity>();
//                user.Setup(x => x.Identity).Returns(identity.Object);
//                httpContext.Setup(x => x.User).Returns(user.Object);
//
//                var controllerContext = new Mock<ControllerContext>();
//                controllerContext.Setup(x => x.HttpContext).Returns(httpContext.Object);
                
//                return controllerContext.Object;
//            }
//        }

//        [TestMethod]
//        public async Task Index()
//        {
//            // Arrange
//            var controller = new HomeController { ControllerContext = ControllerContext, Db = Db};
//
//            // Act
//            var result = await controller.Index() as ViewResult;
//
//            // Assert
//            Assert.IsNotNull(result);
//        }

//        [TestMethod]
//        public async Task ChangeCity()
//        {
//            // Arrange
//            var controller = new HomeController { ControllerContext = ControllerContext, Db = Db };
//            var curCity = Cities[Commons.Rnd.Next(Cities.Count)];
//
//            // Act
//            var result = await controller.Index(curCity.Id) as ViewResult;
//            
//            // Assert
//            Assert.IsNotNull(controller.HttpContext.Session);
//            Assert.IsNotNull(result);
//            Assert.AreEqual(curCity, controller.HttpContext.Session["currentCity"]);
//        }

//        [TestMethod]
//        public void About()
//        {
//            // Arrange
//            var controller = new HomeController();
//
//            // Act
//            var result = controller.About() as ViewResult;
//
//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
//        }

//        [TestMethod]
//        public void Contact()
//        {
//            // Arrange
//            var controller = new HomeController();
//
//            // Act
//            var result = controller.Contact() as ViewResult;
//
//            // Assert
//            Assert.IsNotNull(result);
//        }

//        [TestMethod]
//        public async Task ReverseGeocode()
//        {
//            // Arrange
//            var testData = new[]
//            {
//                new Tuple<double, double, string>(31.6382146, 74.8660257, "Amritsar"),
//                new Tuple<double, double, string>(30.7333, 76.7794, "Chandigarh"),
//                new Tuple<double, double, string>(26.9124, 75.7873, "Jaipur"),
//                new Tuple<double, double, string>(30.8851301, 75.7953848, "Ludhiana"),
//                new Tuple<double, double, string>(19.0728300, 72.8826100, "Mumbai"),
//                new Tuple<double, double, string>(28.5280255, 77.2487134, "DelhiNCR")
//            };
//
//            var controller = new HomeController { ControllerContext = ControllerContext, Db = Db };
//
//            foreach (var dataItem in testData)
//            {
//                // Act
//                var result = await controller.ReverseGeocode(dataItem.Item1, dataItem.Item2);
//
//                // Assert
//                Assert.IsNotNull(result);
//                Assert.IsTrue((bool)result.Data.GetType().GetProperty("Success").GetValue(result.Data, null));
//
//                var objectValue = result.Data.GetType().GetProperty("Object").GetValue(result.Data, null);
//                var cityValue = objectValue.GetType().GetProperty("City").GetValue(objectValue, null);
//                Assert.AreEqual(cityValue.GetType().GetProperty("Id").GetValue(cityValue, null), dataItem.Item3);
//            }
//        }
    }
}
