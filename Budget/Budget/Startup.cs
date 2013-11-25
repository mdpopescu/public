using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Budget.Startup))]
namespace Budget
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
