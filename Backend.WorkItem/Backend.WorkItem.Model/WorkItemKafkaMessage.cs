namespace Backend.WorkItem.Model
{
    public class WorkItemKafkaMessage
    {
        public string Operation { get; set; }
        public int? Id { get; set; }
        public string Title { get; set; }

        public string? Description { get; set; }
    }
}
