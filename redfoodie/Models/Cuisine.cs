using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class Cuisine
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Restaurant> Restaurants { get; set; }
    }
}