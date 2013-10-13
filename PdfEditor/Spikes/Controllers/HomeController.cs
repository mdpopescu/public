using System.Web.Mvc;
using Pdf.Library;
using iTextSharp.text.pdf;

namespace Spikes.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      using (var reader = new PdfReader(@"S:\git\PdfEditor\form12a.pdf"))
      {
        var processor = new PdfProcessor();
        var html = processor.ConvertToHtml(reader, 1);

        //return View(html);
        return Content(html);
      }
    }
  }
}