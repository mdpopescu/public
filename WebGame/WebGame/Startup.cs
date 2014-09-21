using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebGame.Startup))]
namespace WebGame
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
