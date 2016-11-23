﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
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

        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ChangeCity()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            var session = new MockHttpSession();
            controllerContext.Setup(p => p.HttpContext.Session).Returns(session);
            
            // Create a list of cities
            var cities = new List<City>
            {
                new City {Id = 1, ParentId = null, Name = "Delhi NCR"},
                new City {Id = 2, ParentId = 0, Name = "Amritsar"},
                new City {Id = 3, ParentId = 0, Name = "Chandigarh"},
                new City {Id = 4, ParentId = 0, Name = "Jaipur"},
                new City {Id = 5, ParentId = 0, Name = "Ludhiana"},
                new City {Id = 6, ParentId = 0, Name = "Mumbai"},
                new City {Id = 7, ParentId = 0, Name = "Pune"}
            };
            var citiesQueriableList = cities.AsQueryable();

            // Force DbSet to return the IQueryable members of our converted list object as its data source
            var mockSet = new Mock<DbSet<City>>();
            mockSet.As<IQueryable<City>>().Setup(m => m.Provider).Returns(citiesQueriableList.Provider);
            mockSet.As<IQueryable<City>>().Setup(m => m.Expression).Returns(citiesQueriableList.Expression);
            mockSet.As<IQueryable<City>>().Setup(m => m.ElementType).Returns(citiesQueriableList.ElementType);
            mockSet.As<IQueryable<City>>().Setup(m => m.GetEnumerator()).Returns(citiesQueriableList.GetEnumerator());
            mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => cities.FirstOrDefault(d => d.Id == (int)ids[0]));

            var dbContext = new ApplicationDbContext { Cities = mockSet.Object };

            var controller = new HomeController {ControllerContext = controllerContext.Object, Context = dbContext};

            // Act
            var curCity = cities[Rnd.Next(cities.Count)];
            var result = controller.Index(curCity.Id) as ViewResult;
            
            // Assert
            Debug.Assert(controller.HttpContext.Session != null, "controller.HttpContext.Session != null");
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
            Debug.Assert(result != null, "result != null");
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
