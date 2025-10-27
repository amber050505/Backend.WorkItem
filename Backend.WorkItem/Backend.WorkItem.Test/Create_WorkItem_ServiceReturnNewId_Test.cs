using Backend.WorkItem.Repository.WorkItem.Interface;
using Backend.WorkItem.Service.WorkItem;
using Moq;

namespace Backend.WorkItem.Test
{
    public class Create_WorkItem_ServiceReturnNewId_Test
    {
        private Mock<IWorkItemRepository> _mockRepo;
        private WorkItemService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IWorkItemRepository>();
            _service = new WorkItemService(_mockRepo.Object);
        }

        [Test]
        public async Task Create_WorkItem_ServiceReturnNewId()
        {
            var workItem = new Model.WorkItem { Title = "Test Title" };
            _mockRepo.Setup(repo => repo.CreateAsync(It.Is<Model.WorkItem>(item => item.Title != ""))).ReturnsAsync(100);

            var workItemId = await _service.CreateAsync(workItem);
            Assert.That(workItemId, Is.EqualTo(100));
        }
    }
}