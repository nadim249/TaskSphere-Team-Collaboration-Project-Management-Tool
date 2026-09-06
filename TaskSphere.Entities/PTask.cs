using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace TaskSphere.Entities
{
    [Table("Task")]
    public class PTask
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int AssignTo { get; set; }

        [Required] [StringLength(200)] 
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; }= null!;
        [Required]
        [StringLength(30)]
        public string Status { get; set; }=null!;
        [Required]
        [StringLength(30)]
        public string Priority { get; set; }= null!;
        [Required]
        public DateTime DueDate { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("AssignTo")]
        public virtual User? Assignee { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<TAttachment> Attachments { get; set; } = new List<TAttachment>();


    }
}
