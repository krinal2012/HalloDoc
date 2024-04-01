using Hallodoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Drawing;
using static HalloDoc.Entity.Models.Constant;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public List<Role> Role()
        {
            var role = _context.Roles.ToList();
            return (role);
        }
        public bool ProfilePassword(string Password, int UserId)
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
        public PhysiciansData ViewProviderProfile(int PhysicianId)
        {
            PhysiciansData? v = (from p in _context.Physicians
                               join Aspnetuser in _context.AspNetUsers
                               on p.AspNetUserId equals Aspnetuser.Id into aspGroup
                               from asp in aspGroup.DefaultIfEmpty()
                               where p.PhysicianId == PhysicianId
                               select new PhysiciansData
                               {
                                   Physicianid = PhysicianId,
                                   UserName = asp.UserName,
                                   Status = (state)p.Status,
                                   LastName = p.LastName,
                                   FirstName = p.FirstName,
                                   Email = p.Email,
                                   Mobile = p.Mobile,
                                   Medicallicense = p.MedicalLicense,
                                   NpiNumber = p.Npinumber,
                                   SyncEmailaddress= p.SyncEmailAddress,
                                   Address1 = p.Address1,
                                   Address2 = p.Address2,
                                   City = p.City,
                                   ZipCode = p.Zip,
                                   BusinessName = p.BusinessName,
                                   BusinessWebsite = p.BusinessWebsite,
                                   AdminNotes = p.AdminNotes,
                                   IsNonDisclosureDoc=p.IsNonDisclosureDoc,
                                   Isbackgrounddoc = p.IsBackgroundDoc[0],
                                   Isagreementdoc = p.IsAgreementDoc[0],
                                   Iscredentialdoc = p.IsCredentialDoc[0],
                                   Islicensedoc = p.IsLicenseDoc[0]
        }).FirstOrDefault();
            List<Region> regions = new List<Region>();
            regions = _context.PhysicianRegions
                  .Where(r => r.PhysicianId == PhysicianId)
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

            var Admin = _context.Physicians.Where(A => A.PhysicianId == UserId).FirstOrDefault();
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
        public bool EditAdministrator(PhysiciansData PhysiciansData)
        {
            try
            {
                var Data = _context.Physicians.Where(W => W.PhysicianId == PhysiciansData.Physicianid).FirstOrDefault();
                if (Data != null)
                {
                    Data.FirstName = PhysiciansData.FirstName;
                    Data.LastName = PhysiciansData.LastName;
                    Data.Mobile = PhysiciansData.Mobile;
                    Data.Email = PhysiciansData.Email;
                    Data.MedicalLicense = PhysiciansData.Medicallicense;
                    Data.Npinumber = PhysiciansData.NpiNumber;
                    Data.SyncEmailAddress = PhysiciansData.SyncEmailaddress;
                    _context.Physicians.Update(Data);
                    _context.SaveChanges();
                    List<int> regions = _context.PhysicianRegions.Where(r => r.PhysicianId == PhysiciansData.Physicianid).Select(req => req.RegionId).ToList();
                    List<int> Regionsid = PhysiciansData.RegionIdList.Split(',').Select(int.Parse).ToList();

                    if (regions.Count > 0)
                    {
                        foreach (var item in regions)
                        {
                            PhysicianRegion ar = _context.PhysicianRegions.Where(r => r.PhysicianId == PhysiciansData.Physicianid && r.RegionId == item).First();
                            _context.PhysicianRegions.Remove(ar);
                            _context.SaveChanges();
                        }
                    }
                    foreach (var item in Regionsid)
                    {
                        PhysicianRegion ar = new()
                        {
                            RegionId = item,
                            PhysicianId = PhysiciansData.Physicianid
                        };
                        _context.PhysicianRegions.Update(ar);
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
        public bool EditBilling(PhysiciansData AdminProfile)
        {
            var Data = _context.Physicians.Where(W => W.PhysicianId == AdminProfile.Physicianid).FirstOrDefault();
            if (Data != null)
            {
                Data.Address1 = AdminProfile.Address1;
                Data.Address2 = AdminProfile.Address2;
                Data.City = AdminProfile.City;
                Data.Zip = AdminProfile.ZipCode;
                Data.Mobile = AdminProfile.Mobile;
                _context.Physicians.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool EditProviderProfile(PhysiciansData PhysiciansData)
        {
            var Data = _context.Physicians.Where(W => W.PhysicianId == PhysiciansData.Physicianid).FirstOrDefault();
            if (Data != null)
            {
                Data.BusinessName = PhysiciansData.BusinessName;
                Data.BusinessWebsite = PhysiciansData.BusinessWebsite;
                Data.AdminNotes = PhysiciansData.AdminNotes;
                if (PhysiciansData.SignatureFile != null)
                {
                    string FilePath = "wwwroot\\Upload";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    string fileNameWithPath = Path.Combine(path, PhysiciansData.SignatureFile.FileName);
                 
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        PhysiciansData.SignatureFile.CopyTo(stream);
                    }

                    Data.Signature = PhysiciansData.SignatureFile.FileName;
                   
                }
                if (PhysiciansData.PhotoFile != null)
                {
                    string FilePath = "wwwroot\\Upload";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                    string fileNameWithPath = Path.Combine(path, PhysiciansData.PhotoFile.FileName);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        PhysiciansData.PhotoFile.CopyTo(stream);
                    }

                    Data.Photo = PhysiciansData.PhotoFile.FileName;

                }
                _context.Physicians.Update(Data);
                _context.SaveChanges();
                return true;

                
            }
            else
            {
                return false;
            }
        }
        public bool SaveProvider(int[] checkboxes, int physicianid)
        {
            Physician data = _context.Physicians.Where(x=>x.PhysicianId == physicianid).FirstOrDefault();
            foreach(var i in checkboxes)
            {
                switch(i)
                {
                    case 1:
                        data.IsAgreementDoc = new BitArray(1);
                        data.IsAgreementDoc[0] = true; break;
                    case 2:
                        data.IsBackgroundDoc = new BitArray(1);
                        data.IsBackgroundDoc[0] = true; break;
                    case 3:
                        data.IsCredentialDoc = new BitArray(1);
                        data.IsCredentialDoc[0] = true; break;
                    case 4:
                        data.IsNonDisclosureDoc = true; break;
                    case 5:
                        data.IsLicenseDoc = new BitArray(1);
                        data.IsLicenseDoc[0] = true; break;
                }
                
            }
            _context.Physicians.Update(data);
            _context.SaveChanges();
            return true;
        }
        public bool AddProviderAccount(PhysiciansData PhysiciansData, int[] checkboxes, string UserId)
        {
            var Data = new Physician();
            var Aspnetuser = new AspNetUser();
            var AspNetUserRoles = new AspNetUserRole();
            var phyNoti = new PhysicianNotification();
            Guid g = Guid.NewGuid();
            Aspnetuser.Id = g.ToString();
            Aspnetuser.UserName = PhysiciansData.FirstName;
            Aspnetuser.PasswordHash = PhysiciansData.Password;
            Aspnetuser.Email = PhysiciansData.Email;
            Aspnetuser.PhoneNumber = PhysiciansData.Mobile;
            Aspnetuser.CreatedDate = DateTime.Now;
            _context.AspNetUsers.Add(Aspnetuser);
            _context.SaveChanges();

            AspNetUserRoles.UserId = Aspnetuser.Id;
            AspNetUserRoles.RoleId = "3";
            _context.AspNetUserRoles.Add(AspNetUserRoles);
            _context.SaveChanges();

            Data.AspNetUserId = Aspnetuser.Id;
            Data.FirstName = PhysiciansData.FirstName;
            Data.LastName = PhysiciansData.LastName;
            Data.Mobile = PhysiciansData.Mobile;
            Data.Email = PhysiciansData.Email;
            Data.MedicalLicense = PhysiciansData.Medicallicense;
            Data.Npinumber = PhysiciansData.NpiNumber;
            Data.SyncEmailAddress = PhysiciansData.SyncEmailaddress;
            Data.Address1 = PhysiciansData.Address1;
            Data.Address2 = PhysiciansData.Address2;
            Data.City = PhysiciansData.City;
            Data.Zip = PhysiciansData.ZipCode;
            Data.Mobile = PhysiciansData.Mobile;
            Data.BusinessName = PhysiciansData.BusinessName;
            Data.BusinessWebsite = PhysiciansData.BusinessWebsite;
            Data.AdminNotes = PhysiciansData.AdminNotes;
            Data.RoleId = Convert.ToInt32(PhysiciansData.Role);
            Data.IsDeleted=new BitArray(1);
            Data.Status = (short?)(state)PhysiciansData.Status;
            Data.CreatedBy = UserId;
            if (PhysiciansData.SignatureFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, PhysiciansData.SignatureFile.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    PhysiciansData.SignatureFile.CopyTo(stream);
                }

                Data.Signature = PhysiciansData.SignatureFile.FileName;

            }
            if (PhysiciansData.PhotoFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, PhysiciansData.PhotoFile.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    PhysiciansData.PhotoFile.CopyTo(stream);
                }

                Data.Photo = PhysiciansData.PhotoFile.FileName;
            }
            foreach (var i in checkboxes)
            {
                switch (i)
                {
                    case 1:
                        Data.IsAgreementDoc = new BitArray(1);
                        Data.IsAgreementDoc[0] = true; break;
                    case 2:
                        Data.IsBackgroundDoc = new BitArray(1);
                        Data.IsBackgroundDoc[0] = true; break;
                    case 3:
                        Data.IsCredentialDoc = new BitArray(1);
                        Data.IsCredentialDoc[0] = true; break;
                    case 4:
                        Data.IsNonDisclosureDoc = true; break;
                    case 5:
                        Data.IsLicenseDoc = new BitArray(1);
                        Data.IsLicenseDoc[0] = true; break;
                }
            }
            _context.Physicians.Add(Data);
            _context.SaveChanges();

            
            if(PhysiciansData.RegionIdList != null)
            {
                List<int> Regionsid = PhysiciansData.RegionIdList.Split(',').Select(int.Parse).ToList();
                foreach (var item in Regionsid)
                {
                    PhysicianRegion ar = new()
                    {
                        RegionId = item,
                        PhysicianId = Data.PhysicianId
                    };
                    _context.PhysicianRegions.Add(ar);
                    _context.SaveChanges();
                }
            }

            phyNoti.IsNotificationStopped = false;
            phyNoti.PhysicianId= Data.PhysicianId;
            _context.PhysicianNotifications.Add(phyNoti);
            _context.SaveChanges();
            
            return true;
        }
        public bool DeleteProvider(int PhysicianId)
        {
            Physician phy = _context.Physicians.Where(x => x.PhysicianId == PhysicianId).FirstOrDefault();
            phy.IsDeleted[0] = true;
            _context.Physicians.Update(phy);
            _context.SaveChanges();
            return true;

        }
        public List<Menu> RolebyAccountType(int Account)
        {
            var result = _context.Menus
                      .Where(req => req.AccountType == Account)
                      .ToList();
            return result;
        }

    }
}
