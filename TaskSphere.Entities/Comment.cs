using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSphere.Entities
{
    [Table("Comment")]
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("TaskId")]
        public virtual PTask? Task { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
