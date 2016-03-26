using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialClone.Web.Startup))]
namespace SocialClone.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
