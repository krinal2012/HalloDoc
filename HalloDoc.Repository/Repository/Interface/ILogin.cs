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
        public bool SendResetLink(String Email);
        public bool isAccessGranted(int roleId, string menuName);
        public bool CreateAccount(viewPatientReq viewPatientReq);
    }
}
