using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "New password is required")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
           ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Password confirmation does not match the new password.")]
        public string ConfirmPassword { get; set; }
    }
}
