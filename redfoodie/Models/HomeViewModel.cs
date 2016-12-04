using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class HomeViewModel
    {
        [Display(Name = "Email App Link")]
        [EmailAddress]
        public string SubscriptionEmail { get; set; }

        public IEnumerable<UserViewModel> SuggestedUsers { get; set; }
    }
}