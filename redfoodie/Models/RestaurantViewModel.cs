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

    public class RestaurantViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
        [MaxLength(255)]
        public string ImageFileName { get; set; }
        [MaxLength(255)]
        public string UniqueName { get; set; }
    }
}