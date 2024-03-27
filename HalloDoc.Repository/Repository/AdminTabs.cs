using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Drawing;
using static HalloDoc.Entity.Models.Constant;
using Region = HalloDoc.Entity.DataModels.Region;

namespace HalloDoc.Repository.Repository
{
    public class AdminTabs : IAdminTabs
    {
        private readonly HelloDocContext _context;
        private readonly EmailConfiguration _emailConfig;
        public AdminTabs(HelloDocContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }
        public AdminProfile ViewAdminProfile(string UserId)
        {

            AdminProfile? v = (from a in _context.Admins
                               join Aspnetuser in _context.AspNetUsers
                               on a.AspNetUserId equals Aspnetuser.Id into aspGroup
                               from asp in aspGroup.DefaultIfEmpty()
                               where a.AdminId.ToString() == UserId
                               select new AdminProfile
                               {
                                   AdminId = a.AdminId,
                                   UserName = asp.UserName,
                                   LastName = a.LastName,
                                   FirstName = a.FirstName,
                                   Email = a.Email,
                                   ConformEmail = a.Email,
                                   Address1 = a.Address1,
                                   Address2 = a.Address2,
                                   Mobile = a.Mobile,
                                   City = a.City,
                                   ZipCode = a.Zip,
                               }).FirstOrDefault();
            List<Region> regions = new List<Region>();
            regions = _context.AdminRegions
                  .Where(r => r.AdminId.ToString() == UserId)
                  .Select(req => new Region()
                  {
                      RegionId = req.RegionId
                  })
                  .ToList();
            v.RegionIds = regions;
            return v;

        }
        public bool EditPassword(string Password, int UserId)
        {
            var hasher = new PasswordHasher<string>();
            var Admin = _context.Admins.Where(A => A.AdminId == UserId).FirstOrDefault();
            AspNetUser? U = _context.AspNetUsers.FirstOrDefault(m => m.Id == Admin.AspNetUserId);
            if (U != null)
            {
                U.PasswordHash = Password;
                _context.AspNetUsers.Update(U);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool EditAdministratorInfo(AdminProfile AdminProfile)
        {
            try
            {
                var Data = _context.Admins.Where(W => W.AdminId == AdminProfile.AdminId).FirstOrDefault();
                if (Data != null)
                {
                    Data.FirstName = AdminProfile.FirstName;
                    Data.LastName = AdminProfile.LastName;
                    Data.Mobile = AdminProfile.Mobile;
                    Data.Email = AdminProfile.Email;
                    _context.Admins.Update(Data);
                    _context.SaveChanges();
                    List<int> regions = _context.AdminRegions.Where(r => r.AdminId == AdminProfile.AdminId).Select(req => req.RegionId).ToList();
                    List<int> Regionsid = AdminProfile.RegionIdList.Split(',').Select(int.Parse).ToList();

                    if (regions.Count > 0)
                    {
                        foreach (var item in regions)
                        {
                            AdminRegion ar = _context.AdminRegions.Where(r => r.AdminId == AdminProfile.AdminId && r.RegionId == item).First();
                            _context.AdminRegions.Remove(ar);
                            _context.SaveChanges();
                        }
                    }
                    foreach (var item in Regionsid)
                    {
                        AdminRegion ar = new()
                        {
                            RegionId = item,
                            AdminId = AdminProfile.AdminId
                        };
                        _context.AdminRegions.Update(ar);
                        _context.SaveChanges();
                        regions.Remove(item);
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool EditBillingInfo(AdminProfile AdminProfile)
        {
            var Data = _context.Admins.Where(W => W.AdminId == AdminProfile.AdminId).FirstOrDefault();
            if (Data != null)
            {
                Data.Address1 = AdminProfile.Address1;
                Data.Address2 = AdminProfile.Address2;
                Data.City = AdminProfile.City;
                Data.Mobile = AdminProfile.Mobile;
                _context.Admins.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<PhysiciansData> PhysicianAll(int region)
        {
            List<PhysiciansData> data = (from r in _context.Physicians
                                         join role in _context.Roles
                                         on r.RoleId equals role.RoleId into roleGroup
                                         from roles in roleGroup.DefaultIfEmpty()
                                         join Notifications in _context.PhysicianNotifications
                                         on r.PhysicianId equals Notifications.PhysicianId into aspGroup
                                         from nof in aspGroup.DefaultIfEmpty()
                                         where r.IsDeleted == new BitArray(1)
                                           && (region == -1 || r.RegionId == region)
                                         select new PhysiciansData
                                         {
                                             Email = r.Email,
                                             notificationid = nof.PhysicianId,
                                             FirstName = r.FirstName,
                                             LastName = r.LastName,
                                             Role = roles.Name,
                                             Status = (state)r.Status,
                                             IsNonDisclosureDoc = r.IsNonDisclosureDoc,
                                             IsNotificationStopped = nof.IsNotificationStopped
                                         }).ToList();
            return data;
        }
        public bool changeNoti(int[] files, int region)
        {
            List<PhysicianNotification> PhysicianNotification = (from noti in _context.PhysicianNotifications join 
                                                                 phy in _context.Physicians on noti.PhysicianId equals phy.PhysicianId 
                                                                 where (region == -1 || phy.RegionId == region)
                                                                 select noti)
                                                                 .ToList();
            foreach (var item in PhysicianNotification) 
            {
                if (files.Contains(item.PhysicianId))
                {
                    item.IsNotificationStopped = true;
                    _context.PhysicianNotifications.Update(item);
                    _context.SaveChanges();
                }
                else
                {
                    item.IsNotificationStopped = false;
                    _context.PhysicianNotifications.Update(item);
                    _context.SaveChanges();
                }
            }
            return true;
        }
        public bool ContactProviderMail(string Email, string Message)
        {
            _emailConfig.SendMail(Email, "Message from admin", Message);
            return true;
        }
    }
}
