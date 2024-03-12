using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using HalloDoc.Repository.Repository.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDoc.Controllers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CheckProviderAccess : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CheckProviderAccess(string role)
        {
            _role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJWTInterface>();
            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("../AdminHome/Login");
                return;
            }
            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];
            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("../AdminHome/Login");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("../AdminHome/Login");
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roles.Value != _role)
            {
                filterContext.Result = new RedirectResult("../Home/AuthError");
            }

        }

    }
}
