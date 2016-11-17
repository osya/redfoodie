using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class MessagePostViewModel
    {
        [Required]
        [Display(Name = "To:")]
        public string[] To { get; set; }

        //        public ICollection<System.Web.Mvc.SelectListItem> Users { get; set; }
        //        public IEnumerable<System.Web.Mvc.SelectListItem> Users { get; set; }

        [Required]
        [Display(Name = "Message:")]
        public string Message { get; set; }
    }
}