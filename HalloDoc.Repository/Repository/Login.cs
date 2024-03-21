﻿using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
            _emailConfig.SendMail(Email, "Reset your password", $"To reset your password <a href='{agreementUrl}'>Click here..</a>");
            return true;
        }
    }
}
