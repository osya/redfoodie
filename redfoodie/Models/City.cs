using System.ComponentModel.DataAnnotations;

namespace redfoodie.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}