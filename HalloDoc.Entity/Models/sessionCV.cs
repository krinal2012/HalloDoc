//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HalloDoc.Entity.Models
//{
//    public class sessionCV
//        {
//            private static IHttpContextAccessor _httpContextAccessor;
//            static sessionCV()
//            {
//                _httpContextAccessor = new HttpContextAccessor();
//            }
//            public static string? UserName()
//            {
//                string? UserName = null;
//                if (_httpContextAccessor.HttpContext.Session.GetString("UserName") != null)
//                {
//                    UserName = _httpContextAccessor.HttpContext.Session.GetString("UserName").ToString();
//                }
//                return UserName;
//            }
//            public static string? UserID()
//            {
//                string? UserID = null;
//                if (_httpContextAccessor.HttpContext.Session.GetString("UserID") != null)
//                {
//                    UserID = _httpContextAccessor.HttpContext.Session.GetString("UserID");

//                }
//                return UserID;
//            }
//        }
//    }

//}
