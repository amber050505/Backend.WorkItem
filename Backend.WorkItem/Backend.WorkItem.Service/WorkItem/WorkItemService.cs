using Backend.WorkItem.Repository.WorkItem.Interface;
using Backend.WorkItem.Service.WorkItem.Interface;

namespace Backend.WorkItem.Service.WorkItem
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _repo;
        public WorkItemService(IWorkItemRepository repo) => _repo = repo;

        public Task<IEnumerable<Model.WorkItem>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Model.WorkItem> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<int> CreateAsync(Model.WorkItem item)
        {
            item.Title = item.Title?.Trim();
            return await _repo.CreateAsync(item);
        }

        public Task<bool> UpdateAsync(Model.WorkItem item)
        {
            item.Title = item.Title?.Trim();
            return _repo.UpdateAsync(item);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
