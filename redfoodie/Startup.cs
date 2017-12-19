using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(redfoodie.Startup))]
namespace redfoodie
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

//TODO: Update NuGet packages for solution