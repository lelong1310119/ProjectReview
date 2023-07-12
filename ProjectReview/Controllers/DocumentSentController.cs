using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Controllers
{
    public class DocumentSentController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
