using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace redfoodie.Models
{
    public class Restaurant
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("Place")]
        public int PlaceId { get; set; }
        public virtual Place Place { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        public string ImageFileName { get; set; }

        private static string CleanStr(string s)
        {
            return new Regex("[\\(\\)]").Replace(new Regex(" ?(-| ) ?").Replace(s, "-"), "").ToLower();
        }

        public string ImageFullFileName => ImageFileName != null
            ? Path.Combine("/Content/thumbs/business/", $"{CleanStr(Name)}-{CleanStr(Place.Name)}-{CleanStr(Place.City.Name)}", ImageFileName)
            : string.Empty;

        public DbGeography Location { get; set; }

        public virtual ICollection<Cuisine> Cuisines { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
        public int PercentRate => (int)(Votes.Average(v => v.Value ? 1 : 0) * 100);
        public virtual ICollection<RestaurantGroup> Groups { get; set; }
    }
}