﻿using Backend.WorkItem.Controllers.WorkItem;
using Backend.WorkItem.Service.WorkItem.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace Backend.WorkItem.Test
{
    [TestFixture]
    public class Create_WorkItem_RedirectToListWithSuccessMessage_Test
    {
        private Mock<IWorkItemService> _mockService;
        private WorkItemController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IWorkItemService>();
            _controller = new WorkItemController(_mockService.Object);
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task Create_WorkItem_RedirectToListWithSuccessMessage()
        {
            var model = new Model.WorkItem { Title = "Test Title" };
            _mockService.Setup(service => service.CreateAsync(It.Is<Model.WorkItem>(item => item.Title != ""))).ReturnsAsync(100);

            var result = await _controller.Create(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("List"));
            Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo("新增成功"));
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }
    }
}
