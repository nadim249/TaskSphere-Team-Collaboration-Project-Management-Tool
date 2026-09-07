using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskSphere.Entities
{
    [Table("Notification")]
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Required] public int UserId { get; set; } // The user who *receives* the notification

        [Required] [StringLength(200)] public string Title { get; set; } = null!;

        [Required] public string Message { get; set; } = null!;

        [StringLength(50)] public string? Type { get; set; } // e.g., "Task", "Comment", "Attachment"

        public DateTime? CreatedAt { get; set; }

        public int? RelatedTaskId { get; set; }
        public int? RelatedProjectId { get; set; }
        [Required]
        public bool IsRead { get; set; } = false;

    }
}