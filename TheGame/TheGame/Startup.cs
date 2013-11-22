using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheGame.Startup))]
namespace TheGame
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
