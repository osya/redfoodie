using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
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
        private class MockHttpSession : HttpSessionStateBase
        {
            private readonly Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

            public override object this[string name]
            {
                get { return _sessionDictionary[name]; }
                set { _sessionDictionary[name] = value; }
            }
        }

        private static readonly Random Rnd = new Random();

        private static City[] Cities => new[]
        {
            new City {Id = 1, Name = "Delhi NCR"},
            new City {Id = 2, Name = "Amritsar"},
            new City {Id = 3, Name = "Chandigarh"},
            new City {Id = 4, Name = "Jaipur"},
            new City {Id = 5, Name = "Ludhiana"},
            new City {Id = 6, Name = "Mumbai"},
            new City {Id = 7, Name = "Pune"}
        };

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
                    user.Votes = new List<Vote> { new Vote { UserId = user.Id, Value = Rnd.Next(100) < 50 } };
                }

                // Force DbSet to return the IQueryable members of our converted list object as its data source
                var mockUsers = new Mock<DbSet<ApplicationUser>>();
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(usersQueriableList.Provider);
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(usersQueriableList.Expression);
                mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(usersQueriableList.GetEnumerator());

                // Create a list of cities
                
                var citiesQueriableList = Cities.AsQueryable();

                // Force DbSet to return the IQueryable members of our converted list object as its data source
                var mockCities = new Mock<DbSet<City>>();
                mockCities.As<IQueryable<City>>().Setup(m => m.Provider).Returns(citiesQueriableList.Provider);
                mockCities.As<IQueryable<City>>().Setup(m => m.Expression).Returns(citiesQueriableList.Expression);
                mockCities.As<IQueryable<City>>().Setup(m => m.ElementType).Returns(citiesQueriableList.ElementType);
                mockCities.As<IQueryable<City>>().Setup(m => m.GetEnumerator()).Returns(citiesQueriableList.GetEnumerator());
                mockCities.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => Cities.FirstOrDefault(d => d.Id == (int)ids[0]));

                return new ApplicationDbContext { Users = mockUsers.Object, Cities = mockCities.Object };
            }
        }

        private static ControllerContext ControllerContext
        {
            get
            {
                var request = new Mock<HttpRequestBase>();
                request.Setup(x => x.IsAuthenticated).Returns(false);
                var httpContext = new Mock<HttpContextBase>();
                httpContext.Setup(x => x.Request).Returns(request.Object);

                var session = new MockHttpSession();
                httpContext.Setup(x => x.Session).Returns(session);

                var user = new Mock<IPrincipal>();
                var identity = new Mock<IIdentity>();
                user.Setup(x => x.Identity).Returns(identity.Object);
                httpContext.Setup(x => x.User).Returns(user.Object);

                var controllerContext = new Mock<ControllerContext>();
                controllerContext.Setup(x => x.HttpContext).Returns(httpContext.Object);
                
                return controllerContext.Object;
            }
        }

        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new HomeController { ControllerContext = ControllerContext, Db = Db};

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ChangeCity()
        {
            // Arrange
            var controller = new HomeController { ControllerContext = ControllerContext, Db = Db };
            var curCity = Cities[Rnd.Next(Cities.Length)];

            // Act
            var result = controller.Index(curCity.Id) as ViewResult;
            
            // Assert
            Assert.IsNotNull(controller.HttpContext.Session);
            Assert.IsNotNull(result);
            Assert.AreEqual(curCity, controller.HttpContext.Session["currentCity"]);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
