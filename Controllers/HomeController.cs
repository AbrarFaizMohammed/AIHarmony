using AIHarmony.data;
using AIHarmony.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace AIHarmony.Controllers
{
   
    public class HomeController : Controller
    {
       //private readonly ILogger<HomeController> _logger;
        private readonly Applicationdbcontext _db;
        private readonly IConfiguration _config;

        /* public HomeController(ILogger<HomeController> logger)
         {
             _logger = logger;
         }*/

        public HomeController(Applicationdbcontext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Users obj)
        {
            if(obj.password == null)
            {
                ModelState.AddModelError("password", "The password field is required");
            }
            
            if (obj.emailId !=null && obj.password!= null)
            {
                var usersdata = _db.Users.SingleOrDefault<Users>(x => x.emailId == obj.emailId);
                if (usersdata == null)
                {
                    ModelState.AddModelError("emailid", "No user found");
                }
                else
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(obj.password,usersdata.password);
                    if(isPasswordValid)
                    {
                        var token = GenerateToken(usersdata);
                        // Store the token in a cookie
                        Response.Cookies.Append("UserTokenCookie", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, 
                            SameSite = SameSiteMode.None, 
                            Expires = DateTime.Now.AddHours(1) 
                        });

                        return RedirectToAction("Index", "AISelection");
                    }
                    else
                    {
                        ModelState.AddModelError("emailid", "Invalid Emailid/Password");
                    }

                }
            }

            return View(obj);
        }

        public string GenerateToken(Users objUser)
        {

            var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("userId", objUser.usedId.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                                        _config["Jwt:Issuer"],
                                        claims,
                                        expires: DateTime.Now.AddHours(1),
                                        signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public IActionResult signup()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult signup(Users obj)
        {

            Users newUser = new Users();
            if (ModelState.IsValid)
            {
                newUser.firstName = obj.firstName;
                newUser.lastName = obj.lastName;
                newUser.emailId = obj.emailId;
                newUser.password = BCrypt.Net.BCrypt.HashPassword(obj.password);

                _db.Users.Add(newUser);
                _db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
