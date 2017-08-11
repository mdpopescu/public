using Microsoft.Owin;
using Owin;
using StaticBlog.Site.Admin;

[assembly: OwinStartup(typeof(Startup))]

namespace StaticBlog.Site.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}