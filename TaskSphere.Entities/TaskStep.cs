using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSphere.Entities
{
    [Table("TaskStep")]
    public class TaskStep
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = null!;

        [Required]
        public DateTime StepDate { get; set; }

        [Required]
        public int UserId { get; set; }

        // Navigation Property back to Task
        [ForeignKey("TaskId")]
        public virtual PTask? Task { get; set; }
    }
}
