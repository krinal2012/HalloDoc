using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface ILogin
    {
        Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser);
    }
}
