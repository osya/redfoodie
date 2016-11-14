using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class InviteFriendsViewModel
    {
        [Display(Name = "Email address")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Find foodies on Redfoodie")]
        public string SearchUsers { get; set; }
    }
}