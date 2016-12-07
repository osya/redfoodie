using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.IO;

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

        public string ImageFileName { get; set; }

        [Index(IsUnique = true)]
        [MaxLength(255)]
        public string ImageFullFileName => ImageFileName != null
            ? Path.Combine("~/Content/thumbs/business/", $"{Name.Replace(" ", "-").ToLower()}-{Place.Name.Replace(" ", "-").ToLower()}", ImageFileName) : string.Empty;

        public DbGeography Location { get; set; }

        public ICollection<Cuisine> Cuisines { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}