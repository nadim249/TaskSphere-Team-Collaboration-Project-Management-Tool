using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskSphere.Entities
{
    [Table("Project")]
    public class Project
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = null!;

        [Required]

        public DateTime? Deadline { get; set; }
       
        [Required]
        public string Priority { get; set; } = null!;
        [Required]
        public string Status { get; set; } = null!;

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        // Inside Project.cs
        public virtual ICollection<PTask> Tasks { get; set; } = new List<PTask>();
        


    }
}