using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redfoodie.Models
{
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}