using System.IdentityModel.Tokens.Jwt;

namespace HalloDoc.Models
{
    public static class Crredntials
    {

        private static IHttpContextAccessor _httpContextAccessor;
        static Crredntials()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }
        public static string? Role()
        {
            string cookieValue;
            string role = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                role = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "Role").Value;
            }
            return role;
        }
        public static int RoleID()
        {
            string cookieValue;
            int RoleID = 0;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                RoleID = int.Parse(DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "RoleID").Value);
            }
            return RoleID;
        }
        public static string? UserName()
        {
            string cookieValue;
            string UserName = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserName = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "Username").Value;
            }
            return UserName;
        }
        public static string? UserID()
        {
            string cookieValue;
            string UserID = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserId").Value;
            }
            return UserID;
        }
        public static string? CurrentStatus()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["Status"];
            return Status;
        }
        public static string? CurrentStatusName()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["StatusName"];
            return Status;
        }
        public static string? AspNetUserId()
        {
            string cookieValue;
            string UserID = null;
            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();
                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "AspNetUserId").Value;
            }
            return UserID;
        }


    }
}
