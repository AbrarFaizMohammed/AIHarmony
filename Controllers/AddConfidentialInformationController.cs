using AIHarmony.data;
using AIHarmony.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIHarmony.Controllers
{
    public class AddConfidentialInformationController : Controller
    {
        private readonly Applicationdbcontext _db;

        public AddConfidentialInformationController(Applicationdbcontext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            HttpContext context = ControllerContext.HttpContext;
            var userId =context.Items["UserId"];

            var confidentialWords = _db.ConfidentialWords.Where(x=>x.UserId == Guid.Parse(userId.ToString())).Select(x=>x.Word).ToList();
          
            ViewData["UserConfidentialWords"] = confidentialWords;
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult getIndex() {

            var formdata = Request.Form["word"];
            HttpContext context = ControllerContext.HttpContext;
            var userId = context.Items["UserId"];

            ConfidentialWords confidentialWords = new ConfidentialWords();
            if(formdata[0]!=null)
            {
                confidentialWords.UserId = Guid.Parse(userId.ToString());
                confidentialWords.Word = formdata[0];
                _db.ConfidentialWords.Add(confidentialWords);
                _db.SaveChanges();
                return RedirectToAction("Index", "AddConfidentialInformation");
            }
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult deleteWord()
        {
            var formdata = Request.Form["WordName"];
            HttpContext context = ControllerContext.HttpContext;
            var userId = Guid.Parse(context.Items["UserId"].ToString());

            var wordInfo= _db.ConfidentialWords.FirstOrDefault(x=>x.Word == formdata[0] && x.UserId == userId);

            if(wordInfo != null)
            {
                _db.ConfidentialWords.Remove(wordInfo);
                _db.SaveChanges();
                return RedirectToAction("Index", "AddConfidentialInformation");
            }

            return View();
        }
    }
}
