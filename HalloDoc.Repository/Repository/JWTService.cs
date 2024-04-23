using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HalloDoc.Repository.Repository
{
    public class JWTService : IJWTInterface
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration Configuration;
        private readonly HelloDocContext _context;

        public JWTService(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, HelloDocContext context  )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = Configuration;
            this._context = context;
        }
        public string GenerateJWTAuthetication(UserInfo userinfo)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userinfo.Username),
                new Claim(ClaimTypes.Role, userinfo.Role),
                new Claim("FirstName", userinfo.FirstName),
                new Claim("UserId", userinfo.UserId.ToString()),
                new Claim("Username", userinfo.Username.ToString()),
                new Claim("Role", userinfo.Role.ToString()),
                new Claim("RoleID", userinfo.RoleID.ToString()),
                new Claim("AspNetUserId", userinfo.AspNetUserId)
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(60);
            var token = new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler)
        {
            jwtSecurityTokenHandler = null;
            if (token == null)
                return false;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
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

                jwtSecurityTokenHandler = (JwtSecurityToken)validatedToken;

                if (jwtSecurityTokenHandler != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public List<string> getManuByID(string RoleID)
        {
            List<RoleMenu> data = _context.RoleMenus.Where(r => r.RoleId == Int32.Parse(RoleID)).ToList();
            List<string> list = new List<string>();
            foreach (var item in data)
            {
                string str = _context.Menus.FirstOrDefault(e => e.MenuId == item.MenuId).Name;
                list.Add(str);
            }
            return list;
        }
        public class CustomAuthorize : Attribute, IAuthorizationFilter
        {
            private readonly List<string> _role;
            private readonly string _menu;
            public CustomAuthorize(string role = "", string menu = "")
            {
                _role = role.Split(",").ToList();
                _menu = menu;
            }
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var jwtService = context.HttpContext.RequestServices.GetService<IJWTInterface>();
                if (jwtService == null)
                {
                    
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    return;
                }

                var request = context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    return;
                }

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);
                List<string> str = null;
                if (_role.Contains("Admin") || _role.Contains("Physician"))
                {
                    var RoleID = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "RoleID").Value;
                    str = new List<string>();
                    var Accessrepo = context.HttpContext.RequestServices.GetService<IJWTInterface>();
                    str = Accessrepo.getManuByID(RoleID);
                }
                if (roleClaim == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    return;
                }
                var flag = false;
                foreach (var role in _role)
                {
                    if (string.IsNullOrWhiteSpace(role) || roleClaim.Value != role)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
                if (_role.Contains("Admin") || _role.Contains("Physician"))
                {
                    if (flag == false || !str.Contains(_menu))
                    {
                        context.Result = new RedirectResult("../Home/AccessDenied");
                    }
                }
                if (!flag)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                }

            }
        }
    }
}



