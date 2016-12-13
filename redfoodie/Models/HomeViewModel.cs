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
        public IEnumerable<Cuisine> CuisinesOdd { get; set; }
        public IEnumerable<Cuisine> CuisinesEven { get; set; }
        public IEnumerable<Place> PlacesOdd { get; set; }
        public IEnumerable<Place> PlacesEven { get; set; }
        public IEnumerable<RestaurantGroupViewModel> RestaurantGroups { get; set; }
    }
}