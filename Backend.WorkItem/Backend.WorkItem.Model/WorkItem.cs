using System.ComponentModel.DataAnnotations;

namespace Backend.WorkItem.Model
{
    public class WorkItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "標題為必填")]
        [StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}
