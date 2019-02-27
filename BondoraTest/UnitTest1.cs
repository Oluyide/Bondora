using System;
using System.IO;
using System.Web.Mvc;
using Bondora2.Controllers;
using BusinessLogic.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BondoraTest
{
    [TestClass]
    public class UnitTest1
    { 
        [TestMethod]
        public void ReturnsAllInventoriesView()
        {
            var mockService = new Mock<IInventory>();
            InventoriesController controllerUnderTest = new InventoriesController(mockService.Object);
            var result = controllerUnderTest.Index().Result as ViewResult;
            Assert.AreEqual("", result.ViewName);
            mockService.Verify();
        }

        [TestMethod]
        public void RentItemTest()
        {
            
            int Id = 1;
            var mockService = new Mock<IInventory>();
            InventoriesController controllerUnderTest = new InventoriesController(mockService.Object);
            var result = controllerUnderTest.RentItem(Id).Result as JsonResult;
            Assert.IsNotNull(result, "result must not be null");
            mockService.Verify();
        }

        [TestMethod]
        public void CalculataAndDownloadInvoiceForAuser()
        {
            string userId = "48c8a50d - d479 - 4340 - 8cbf - 319108673589";
            var mockService = new Mock<IInventory>();
            InventoriesController controllerUnderTest = new InventoriesController(mockService.Object);
            var result = controllerUnderTest.DownloadInvoice(userId).Result as FileContentResult;
            Assert.IsNotNull(result, "result must not be null");
            mockService.Verify();
        }
    }
}
