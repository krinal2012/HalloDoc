using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Web.Helpers;

namespace HalloDoc.Repository.Repository
{
    public class Login : ILogin
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HelloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public Login(HelloDocContext context, IHttpContextAccessor httpContextAccessor, EmailConfiguration emailConfig)
        {
            this.httpContextAccessor = httpContextAccessor;
            _context = context;
            _emailConfig = emailConfig;
        }
        public async Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email && u.PasswordHash == aspNetUser.PasswordHash);
            UserInfo admin = new UserInfo();
            if (user != null)
            {
                var data = _context.AspNetUserRoles.FirstOrDefault(E => E.UserId == user.Id);
                var datarole = _context.AspNetRoles.FirstOrDefault(e => e.Id == data.RoleId);
                admin.Username = user.UserName;
                admin.FirstName = admin.FirstName ?? string.Empty;
                admin.LastName = admin.LastName ?? string.Empty;
                admin.Role = datarole.Name;
                admin.AspNetUserId = user.Id;

                if (admin.Role == "Admin")
                {
                    var admindata = _context.Admins.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.AdminId;
                    admin.RoleID = (int)admindata.RoleId;
                }
                else if (admin.Role == "Patient")
                {
                    var admindata = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.UserId;

                }
                else
                {
                    var admindata = _context.Physicians.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.RoleID = (int)admindata.RoleId;
                    admin.UserId = admindata.PhysicianId;
                }
                return admin;
            }
            else
            {
                return null;
            }
        }
        public bool isAccessGranted(int roleId, string menuName)
        {
            // Get the list of menu IDs associated with the role
            IQueryable<int> menuIds = _context.RoleMenus
                                            .Where(e => e.RoleId == roleId)
                                            .Select(e => e.MenuId);

            // Check if any menu with the given name exists in the list of menu IDs
            bool accessGranted = _context.Menus
                                         .Any(e => menuIds.Contains(e.MenuId) && e.Name == menuName);

            return accessGranted;
        }
        public bool SendResetLink(String Email)
        {
            var agreementUrl = "https://localhost:7151/Home/ResetPass?Email=" + Email;
            var subject = "Reset your password";
            var EmailTemplate = $"To reset your password <a href='{agreementUrl}'>Click here..</a>";
            bool sent = _emailConfig.SendMail(Email, subject, EmailTemplate).Result;
            EmailLog em = new EmailLog
            {

                EmailTemplate = EmailTemplate,
                SubjectName = subject,
                EmailId = Email,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1),
                SentTries = 1,
                Action = 5, // action 5 for send link of reset request
                RoleId = 2, // role 2 for admin
            };

            if (sent)
            {
                em.IsEmailSent[0] = true;
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;

        }
        public bool CreateAccount(viewPatientReq viewPatientReq)
        {
            var isexist = _context.Users.Any(req => req.Email == viewPatientReq.Email);
            if (isexist)
            {
                return false;
            }
                var Aspnetuser = new AspNetUser();
                var role = new AspNetUserRole();
                var User = new User();
                var Request = new Request();
                var Requestclient = new RequestClient();
                var U = _context.RequestClients.FirstOrDefault(m => m.Email == viewPatientReq.Email);
                Guid g = Guid.NewGuid();
                Aspnetuser.Id = g.ToString();
                Aspnetuser.UserName = U.FirstName;
                Aspnetuser.PasswordHash = viewPatientReq.Pass;
                Aspnetuser.Email = viewPatientReq.Email;
                Aspnetuser.PhoneNumber = U.PhoneNumber;
                Aspnetuser.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(Aspnetuser);
                _context.SaveChanges();
                role.UserId = Aspnetuser.Id;
                role.RoleId = "1"; //For Patient Role
                _context.AspNetUserRoles.Add(role);
                _context.SaveChanges();


                User.AspNetUserId = Aspnetuser.Id;
                User.FirstName = U.FirstName;
                User.LastName = U.LastName;
                User.Email = viewPatientReq.Email;
                User.Mobile = U.PhoneNumber;
                User.Street = U.Street;
                User.City = U.City;
                User.State = U.State;
                User.ZipCode = U.ZipCode;
                User.StrMonth = U.StrMonth;
                User.IntDate = U.IntDate;
                User.IntYear = U.IntYear;
                User.Status = 1; //for new request
                User.CreatedBy = Aspnetuser.Id;
                User.CreatedDate = DateTime.Now;
                _context.Users.Add(User);
                _context.SaveChanges();

                var res = (from req in _context.Requests
                           join rc in _context.RequestClients
                           on req.RequestId equals rc.RequestId
                           where rc.Email == viewPatientReq.Email
                           select req.RequestId).ToList();

                foreach (var r in res)
                {
                    var req = _context.Requests.FirstOrDefault(req => req.RequestId == r);
                    req.UserId = User.UserId;
                    _context.Requests.Update(req);
                    _context.SaveChanges();
                }
                return true;
        }
    }
}

