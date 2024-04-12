using AIHarmony.data;
using AIHarmony.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIHarmony.Controllers
{
    public class chat : Controller
    {
        private readonly Applicationdbcontext _db;

        public chat(Applicationdbcontext db)
        {
            _db = db;
        }

        public ViewResult Index(string selectedAIoption)
        {
            HttpContext context = ControllerContext.HttpContext;
            var userId = context.Items["UserId"];
          
                context.Items["selectedAiOption"] = selectedAIoption;
                ViewData["SelectedAIValue1"] = context.Items["selectedAiOption"];
                ViewData["UserIdentification"] = userId;
            
            if (userId != null) {
                var userDetails = _db.Users.SingleOrDefault<Users>(x => x.usedId == Guid.Parse(userId.ToString()));
                TempData["UserName"] = $"{userDetails.firstName} {userDetails.lastName}";
            }
            
            return View();
        }

        public ViewResult comparechat(string compareOption1, string compareOption2)
        {
            HttpContext context = ControllerContext.HttpContext;
            var userId = context.Items["UserId"];

            context.Items["selectedAiOption1"] = compareOption1;
            context.Items["selectedAiOption2"] = compareOption2;

            ViewData["SelectedAIValue1"] = context.Items["selectedAiOption1"];
            ViewData["SelectedAIValue2"] = context.Items["selectedAiOption2"];
            ViewData["UserIdentification"] = userId;

            if (userId != null)
            {
                var userDetails = _db.Users.SingleOrDefault<Users>(x => x.usedId == Guid.Parse(userId.ToString()));
                TempData["UserName"] = $"{userDetails.firstName} {userDetails.lastName}";
            }
            return View();
        }
    }
}
