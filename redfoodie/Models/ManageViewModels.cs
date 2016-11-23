using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace redfoodie.Models
{
    public class ProfileSettingsViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [MinLength(4)]
        public string UserName { get; set; }

        public SelectList Cities { get; set; }

        [Required]
        [Display(Name = "Select City")]
        public int SelectedCity { get; set; }

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
        [MaxLength(80)]
        public string Bio { get; set; }

        /// <summary>
        /// This property is here only for render _ShortUrl partial view
        /// </summary>
        public string ShortUrl { get; set; }

        /// <summary>
        /// This property is here only for render _Notification Settings partial view
        /// </summary>
        public bool FollowMail { get; set; }
        public bool ReplyReviewmail { get; set; }
        public bool ThanksFavoritemail { get; set; }
    }

    public class ViewProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [MinLength(4)]
        public string UserName { get; set; }

        public string ImagePath { get; set; }
    }

    public class ShortUrlViewModel
    {
        [Display(Name = "Username")]
        [MinLength(4)]
        public string ShortUrl { get; set; }
    }

    public class NotificationSettingsViewModel
    {
        [Display(Name = "Someone follows me")]
        public bool FollowMail { get; set; }
        [Display(Name = "A restaurant owner replies to my review")]
        public bool ReplyReviewmail { get; set; }
        [Display(Name = "Someone thanked my reviews / liked photos")]
        public bool ThanksFavoritemail { get; set; }
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
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class DeactivateViewModel
    {
        [Display(Name = "Confirm new password")]
        public string Reason { get; set; }
    }

    public class ImagePopupViewModel
    {
        [Required]
        [Display(Name = "Upload Picture")]
        [HttpPostedFileExtensions(Extensions = "jpg,jpeg,png")]
        public HttpPostedFileBase UserFile { get; set; }
    }
}