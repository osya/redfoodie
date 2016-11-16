using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
}