using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class BaseViewModel
    {
        [Required]
        public int CityId;

        private readonly int? _parentId;
        public string Href;
        public string Name;

        protected BaseViewModel(BaseViewModel model)
        {
            CityId = model.CityId;
            _parentId = model._parentId;
            Href = model.Href;
            Name = model.Name;
            Cities = model.Cities;
        }

        public BaseViewModel()
        {
        }

        public IEnumerable<City> Cities { get; set; }
    }
}