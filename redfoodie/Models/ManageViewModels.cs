using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace redfoodie.Models
{
    public class ProfileSettingsViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Select City")]
        public string City { get; set; }

        [Display(Name = "Twitter Handle")]
        [Url]
        public string Twitter { get; set; }

        [Display(Name = "Facebook Page Url")]
        [Url]
        public string Facebook { get; set; }

        [Display(Name = "Website or Blog")]
        [Url]
        public string Website { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [Display(Name = "Username")]
        [Url]
        public string ShortUrl { get; set; }

        [Display(Name = "Someone follows me")]
        public bool FollowMail { get; set; }
        [Display(Name = "A restaurant owner replies to my review")]
        public bool ReplyReviewmail { get; set; }
        [Display(Name = "Someone thanked my reviews / liked photos")]
        public bool ThanksFavoritemail { get; set; }

        public bool HasPassword { get; set; }
//        public IList<UserLoginInfo> Logins { get; set; }
        //        public bool TwoFactor { get; set; }
        //        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}