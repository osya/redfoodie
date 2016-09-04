using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using redfoodie;
using redfoodie.Controllers;
using redfoodie.Models;

namespace redfoodie.Tests.Controllers
{
    // TODO: Implement copying or generating database for testing
    [TestClass]
    public class HomeControllerTest
    {
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
            var controller = new HomeController();

            // Act
            // TODO: Get city names from database
            var result = controller.Index("amritsar") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(((redfoodie.Models.BaseViewModel)result.Model).Name, "Amritsar");
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
