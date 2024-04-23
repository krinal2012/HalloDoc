using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IJWTInterface
    {
        //public string GenerateToken(AspNetUser user);
        string GenerateJWTAuthetication(UserInfo userinfo);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
        public List<string> getManuByID(string RoleID);
    }
}
