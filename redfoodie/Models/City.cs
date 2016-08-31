using System;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public int? ParentId { get; set; }
        public string Href { get; set; }
        public string Name { get; set; }
    }
}