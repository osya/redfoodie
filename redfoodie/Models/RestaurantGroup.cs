using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace redfoodie.Models
{
    public class RestaurantGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        public string ImageFileName { get; set; }

        public string ImageFullFileName => ImageFileName != null
            ? Path.Combine("/Content/thumbs/restaurant_groups/", ImageFileName)
            : string.Empty;

        public ICollection<Restaurant> Restaurants { get; set; }
    }
}