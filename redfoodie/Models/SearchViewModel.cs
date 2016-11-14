using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class SearchViewModel
    {
        [Display(Name = "Find restaurant, cuisine, locality...")]
        public string SearchTxt { get; set; }

        [Display(Name = "Type Location Name")]
        public string SearchTxtLoc { get; set; }
    }
}