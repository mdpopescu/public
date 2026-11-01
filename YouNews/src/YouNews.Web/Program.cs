using YouNews.Web;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});
builder.AddYouNews();

var app = builder.Build();
await app.UseYouNewsAsync();
await app.RunAsync();

/// <summary>Lets the web tests host the application.</summary>
public partial class Program;