using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;

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
                }
                else if (admin.Role == "Patient")
                {
                    var admindata = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    admin.UserId = admindata.UserId;
                }
                else
                {
                    var admindata = _context.Physicians.FirstOrDefault(u => u.AspNetUserId == user.Id);
                    //admin.UserId = admindata.Physicianid;
                }
                return admin;
            }
            else
            {
                return null;
            }
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
    }
}
