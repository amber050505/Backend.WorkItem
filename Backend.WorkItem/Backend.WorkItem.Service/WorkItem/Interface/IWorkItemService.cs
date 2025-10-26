namespace Backend.WorkItem.Service.WorkItem.Interface
{
    public interface IWorkItemService
    {
        Task<IEnumerable<Model.WorkItem>> GetAllAsync();
        Task<Model.WorkItem> GetByIdAsync(int id);
        Task<int> CreateAsync(Model.WorkItem item);
        Task<bool> UpdateAsync(Model.WorkItem item);
        Task<bool> DeleteAsync(int id);
    }
}
