using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class BaseViewModel
    {
        [Required]
        public int CityId;
        public int? ParentId;
        public string Href;
        public string Name;
        public IEnumerable<City> Cities { get; set; }
    }
}