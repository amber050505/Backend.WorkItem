using Backend.WorkItem.Model;

namespace Backend.WorkItem.Repository.WorkItem.Interface
{
    public interface IWorkItemRepository
    {
        Task<WorkItemList> GetAllAsync(int page);
        Task<Model.WorkItem?> GetByIdAsync(int id);
        Task<int> CreateAsync(Model.WorkItem item);
        Task<bool> UpdateAsync(Model.WorkItem item);
        Task<bool> DeleteAsync(int id);
    }
}
