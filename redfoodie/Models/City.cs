using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redfoodie.Models
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Place> Places { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public override bool Equals(object other)
        {
            var otherCity = other as City;
            return string.Equals(Id, otherCity?.Id) && string.Equals(Name, otherCity?.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}