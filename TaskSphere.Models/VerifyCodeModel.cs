using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSphere.Models
{
    public class VerifyCodeModel
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed.")]
        public string Code { get; set; }
    }
}
