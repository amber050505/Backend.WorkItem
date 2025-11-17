using Backend.WorkItem.Model;

namespace Backend.WorkItem.Service.WorkItem.Interface
{
    public interface IWorkItemService
    {
        Task<WorkItemList> GetAllAsync(int page);
        Task<Model.WorkItem> GetByIdAsync(int id);
        Task CreateAsync(Model.WorkItem item);
        Task UpdateAsync(Model.WorkItem item);
        Task DeleteAsync(int id);
    }
}
