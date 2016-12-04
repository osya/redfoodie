using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Place> Places { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public override bool Equals(object other)
        {
            var otherCity = other as City;
            return Id == otherCity?.Id && string.Equals(Name, otherCity.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id*397) ^ (Name?.GetHashCode() ?? 0);
            }
        }
    }
}