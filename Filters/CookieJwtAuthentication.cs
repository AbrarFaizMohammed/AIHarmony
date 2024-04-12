using AIHarmony.data;
using AIHarmony.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace AIHarmony.Filters
{
    public class CookieJwtAuthentication 
    {
        private readonly RequestDelegate _next;

        public CookieJwtAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the request is targeting a specific controller
           if(context.Request.Path == "/AISelection" || context.Request.Path == "/chat" 
                || context.Request.Path == "/chat/comparechat" || context.Request.Path== "/AddConfidentialInformation" ||
                context.Request.Path == "/AddConfidentialInformation/getIndex" || context.Request.Path == "/AddConfidentialInformation/deleteWord")
            {
                
             var usertoken = context.Request.Cookies["UserTokenCookie"];
             string? userId = ValidateToken(usertoken);
                if (usertoken != null && userId!=null)
                {

                    context.Items["UserId"] = userId;
                   
                }
                else
                {
                    context.Response.Cookies.Delete("UserTokenCookie");
                    context.Response.Redirect("/");
                }         
               

               
            }

            // Continue the request pipeline
            await _next(context);
        }

        private string? ValidateToken(string token)
        {
            if (token == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt_key = Environment.GetEnvironmentVariable("JWT_KEY");
            var key = Encoding.ASCII.GetBytes(jwt_key);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = (jwtToken.Claims.First(x=>x.Type == "userId").Value).ToString();

                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

    }

}
