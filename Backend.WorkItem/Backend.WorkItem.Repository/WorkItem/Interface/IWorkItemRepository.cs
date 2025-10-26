using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.WorkItem.Repository.WorkItem.Interface
{
    public interface IWorkItemRepository
    {
        Task<IEnumerable<Model.WorkItem>> GetAllAsync();
        Task<Model.WorkItem?> GetByIdAsync(int id);
        Task<int> CreateAsync(Model.WorkItem item);
        Task<bool> UpdateAsync(Model.WorkItem item);
        Task<bool> DeleteAsync(int id);
    }
}
