using System.ComponentModel.DataAnnotations;

namespace TaskSphere.Models;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
}