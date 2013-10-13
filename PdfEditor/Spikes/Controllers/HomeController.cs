using System.Web.Mvc;
using Pdf.Library;

namespace Spikes.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      var processor = new PdfProcessor();
      var html = processor.ConvertToHtml(@"..\..\form12a.pdf");

      return View(html);
    }
  }
}