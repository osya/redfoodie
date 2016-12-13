using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class RestaurantSearchViewModel
    {
        [Display(Name = "Find restaurant, cuisine, locality...")]
        public string SearchTxt { get; set; }

        [Display(Name = "Type Location Name")]
        public string SearchTxtLoc { get; set; }
    }


    public class CityViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PlaceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CityViewModel City { get; set; }
    }

    public class CuisineViewModel
    {
        public string Name { get; set; }
    }

    public class VoteViewModel
    {
        public UserViewModel ApplicationUser { get; set; }
        public string ReviewText { get; set; }
    }

    public class RestaurantGroupViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageFullFileName { get; set; }
    }

    public class RestaurantViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageFullFileName { get; set; }
        public PlaceViewModel Place { get; set; }
        public ICollection<CuisineViewModel> Cuisines { get; set; }
        public ICollection<VoteViewModel> Votes { get; set; }
        public int PercentRate { get; set; }
        public string HeartClass => PercentRate >= 90
            ? "rateHigherHeart"
            : PercentRate >= 70
                ? "rateHighHeart"
                : PercentRate >= 50
                    ? "rateMiddleHeart"
                    : PercentRate >= 30 ? "rateLowHeart" : "rateLowerHeart";
        public ICollection<RestaurantGroupViewModel> Groups { get; set; }
    }
}