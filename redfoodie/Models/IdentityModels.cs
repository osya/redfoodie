using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace redfoodie.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("City")]
        public string CityId { get; set; }
        public virtual City City { get; set; }

        public string Twitter { get; set; }

        public string Facebook { get; set; }

        public string Website { get; set; }

        [MaxLength(80)]
        public string Bio { get; set; }

        public string ShortUrl { get; set; }

        public bool FollowMail { get; set; }
        
        public bool ReplyReviewmail { get; set; }
        
        public bool ThanksFavoritemail { get; set; }

        public string ImageFileName { get; set; }

        public string UserPath
            =>
            Path.Combine("/Content/thumbs/user/",
                $"{UserName.Replace(" ", "-").ToLower()}{(Birthday != DateTime.MinValue ? $"-{Birthday.Year}" : string.Empty)}")
            ;
        public string ImageFullFileName => ImageFileName != null ? Path.Combine(UserPath, ImageFileName) : "/Content/collections/imgs/user.svg";

        public DateTime Birthday { get; set; } = (DateTime)SqlDateTime.MinValue;

        public bool Verified { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Follow> Follows { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public sealed partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(Environment.ExpandEnvironmentVariables(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString), false)
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}