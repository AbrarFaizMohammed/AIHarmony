using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AIHarmony.Controllers
{
    
    public class AISelectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Userlogout()
        {
            HttpContext context = ControllerContext.HttpContext;
            context.Response.Cookies.Delete("UserTokenCookie");
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult getIndex()
        {
            string[] selectedAIoptions = Request.Form["AIName"];

            ViewData["SelectedAIOptions"] = selectedAIoptions.Length;

            if (selectedAIoptions.Length == 0 && Request.Path == "/AISelection/getIndex")
            {
                TempData["error"] = "Must select alteast one option";
                return RedirectToAction("Index", "AISelection");
            }
            if(selectedAIoptions.Length == 2)
            {
                ViewData["SelectedAIValue1"] = $"{selectedAIoptions[0]}";
                ViewData["SelectedAIValue2"] = $"{selectedAIoptions[1]}";
                return RedirectToAction("comparechat", "chat", new {compareOption1=$"{selectedAIoptions[0]}", compareOption2 = $"{selectedAIoptions[1]}" });
            }

            ViewData["SelectedAIValue1"] = $"{selectedAIoptions[0]}";
            return RedirectToAction("Index", "chat", new {selectedAIoption = $"{selectedAIoptions[0]}"});
        }
    }
}
