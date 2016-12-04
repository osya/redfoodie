using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace redfoodie.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("FollowUser")]
        public string FollowUserId { get; set; }
        public virtual ApplicationUser FollowUser { get; set; }

    }
}