using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        public ExternalLoginConfirmationViewModel(string email)
        {
            Email = email;
        }

        /// <summary>
        /// Do no tdelete this constructor. If delete it there will be error in runtime during Facebook registration
        /// </summary>
        public ExternalLoginConfirmationViewModel()
        {
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ForgootPasswordEmailViewModel
    {
        [Required]
        public string CallbackUrl { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public LoginViewModel()
        {
            RememberMe = true;
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$", ErrorMessage = "Passwords must have at least one digit('0' - '9'), at least one lowercase ('a'-'z'), at least one uppercase('A' - 'Z'), and at least one non letter or digit character")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string UserName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Repeat password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        /// This property named "Username" instead of "Email" because to be able LastPass store all fields from all 
        /// forms necessary to exists one form with only one input field with id/name "...user..."
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Username { get; set; }
    }
}
