﻿using HalloDoc.Entity.Models.ViewModel;
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
        public JWTService(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = Configuration;
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
        public class CustomAuthorize : Attribute, IAuthorizationFilter
        {
            private readonly string _role;
            public CustomAuthorize(string role = "")
            {
                _role = role;
            }
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var jwtService = context.HttpContext.RequestServices.GetService<IJWTInterface>();

                if (jwtService == null)
                {
                    if (_role == "Admin")
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    }
                    else if (_role == "Patient")
                    {
                       context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                    }
                    return;
                }

                var request = context.HttpContext.Request;
                var token = request.Cookies["jwt"];

                if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
                {
                    if (_role == "Patient")
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                    }
                    else if (_role == "Admin")
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    }
                    return;
                }

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                if (roleClaim == null)
                {
                    if (_role == "Patient")
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
                    }
                    else if (_role == "Admin")
                    {
                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminHome", action = "Login" }));
                    }
                    return;
                }

                if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                }
            }
        }
    }
}
//            private readonly IConfiguration _configuration;

//            public JWTService(IConfiguration configuration)
//            {
//                _configuration = configuration;
//            }

//            public string GenerateToken(AspNetUser user)
//            {
//                var claims = new List<Claim>
//            {
//                //new Claim(ClaimTypes.Email, user.Email),
//                //new Claim(ClaimTypes.Role, user.AspNetUserRoles.FirstOrDefault().Roleid),
//                //new Claim("UserID", user.Id),
//                //new Claim("Username", user.Name)
//            };

//                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(_configuration["Jwt:Key"])));
//                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//                var expires = DateTime.UtcNow.AddHours(2);

//                var token = new JwtSecurityToken(
//                    Convert.ToString(_configuration["Jwt:Issuer"]),
//                    Convert.ToString(_configuration["Jwt:Audience"]),
//                    claims,
//                    expires: expires,
//                    signingCredentials: creds
//                );

//                return new JwtSecurityTokenHandler().WriteToken(token);
//            }

//            public bool ValidateToken(string token, out JwtSecurityToken jWTSecurityToken)
//            {
//                jWTSecurityToken = null;

//                if (token == null)
//                    return false;

//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.UTF8.GetBytes(Convert.ToString(_configuration["Jwt:Key"]));
//                try
//                {
//                    tokenHandler.ValidateToken(token, new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,
//                        IssuerSigningKey = new SymmetricSecurityKey(key)
//    ,
//                        ValidateIssuer = false,
//                        ValidateAudience = false,
//                        ClockSkew = TimeSpan.Zero,
//                    }, out SecurityToken validatedToken);

//                    // Corrected access to the validatedToken
//                    jWTSecurityToken = (JwtSecurityToken)validatedToken;

//                    if (jWTSecurityToken != null)
//                    {
//                        return true;
//                    }
//                    return false;
//                }
//                catch
//                {
//                    return false;
//                }
//            }
//        }

//        public class CustomAuthorize : Attribute, IAuthorizationFilter
//        {
//            private readonly string _role;

//            public CustomAuthorize(string role = "")
//            {
//                _role = role;
//            }

//            public void OnAuthorization(AuthorizationFilterContext context)
//            {
//                var jwtService = context.HttpContext.RequestServices.GetService<JWTService>();

//                if (jwtService == null)
//                {
//                    if (_role == "2")
//                    {
//                        //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "Login" }));
//                    }
//                    else if (_role == "1")
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
//                    }
//                    return;
//                }

//                var request = context.HttpContext.Request;
//                var token = request.Cookies["jwt"];

//                if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
//                {
//                    if (_role == "1")
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
//                    }
//                    else if (_role == "2")
//                    {
//                       // context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Patient", action = "Login" }));
//                    }
//                    return;
//                }

//                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

//                if (roleClaim == null)
//                {
//                    if (_role == "1")
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Login" }));
//                    }
//                    else if (_role == "2")
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Patient", action = "Login" }));
//                    }
//                    return;
//                }

//                if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
//                {
//                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
//                }
//            }
//        }
//    }



