using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSphere.Entities
{
    [Table("Attachment")]
    public class TAttachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttachmentId { get; set; }

        [Required] public int TaskId { get; set; }
        [Required, StringLength(255)] public string? Filename { get; set; }

        [StringLength(255)]
        public string? FilePath { get; set; }

        [Required] public int UploadedBy { get; set; }
        public DateTime? UploadedAt { get; set; }

        [ForeignKey("TaskId")]
        public virtual PTask? Task { get; set; }

        [ForeignKey("UploadedBy")]
        public virtual User? Uploader { get; set; }
    }
}
