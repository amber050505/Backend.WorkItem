namespace Backend.WorkItem.Service.WorkItem.Interface
{
    public interface IWorkItemService
    {
        Task<IEnumerable<Model.WorkItem>> GetAllAsync();
        Task<Model.WorkItem> GetByIdAsync(int id);
        Task CreateAsync(Model.WorkItem item);
        Task UpdateAsync(Model.WorkItem item);
        Task DeleteAsync(int id);
    }
}
