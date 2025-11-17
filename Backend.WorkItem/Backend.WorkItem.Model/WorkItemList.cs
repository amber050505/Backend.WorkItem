namespace Backend.WorkItem.Model
{
    public class WorkItemList
    {
        public IEnumerable<WorkItem> Items { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
