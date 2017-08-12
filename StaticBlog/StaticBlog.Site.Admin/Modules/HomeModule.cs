using Nancy;

namespace StaticBlog.Site.Admin.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters => "Hello, World.";
        }
    }
}