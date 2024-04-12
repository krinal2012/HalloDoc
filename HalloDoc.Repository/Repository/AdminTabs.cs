using HalloDoc.Entity.DataContext;
using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using static HalloDoc.Entity.Models.Constant;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
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
        public bool AddAdminAccount(AdminProfile admindata, int[] checkboxes)
        {
            var Aspnetuser = new AspNetUser();
            var AspNetUserRoles = new AspNetUserRole();
            Guid g = Guid.NewGuid();
            Aspnetuser.Id = g.ToString();
            Aspnetuser.UserName = admindata.UserName;
            Aspnetuser.PasswordHash = admindata.Password;
            Aspnetuser.Email = admindata.Email;
            Aspnetuser.PhoneNumber = admindata.Mobile;
            Aspnetuser.CreatedDate = DateTime.Now;
            _context.AspNetUsers.Add(Aspnetuser);
            _context.SaveChanges();

            AspNetUserRoles.UserId = Aspnetuser.Id;
            AspNetUserRoles.RoleId = "2";
            _context.AspNetUserRoles.Add(AspNetUserRoles);
            _context.SaveChanges();

            var Admin = new Admin();
            Admin.AspNetUserId = Aspnetuser.Id;
            Admin.FirstName = admindata.FirstName;
            Admin.LastName = admindata.LastName;
            Admin.Status = 1;
            //Admin.RoleId = admindata.Role;
            Admin.Email = admindata.Email;
            Admin.Mobile = admindata.Mobile;
            Admin.IsDeleted = new BitArray(1);
            Admin.IsDeleted[0] = false;
            Admin.Address1 = admindata.Address1;
            Admin.Address2 = admindata.Address2;
            Admin.City = admindata.City;
            Admin.Zip = admindata.ZipCode;
            Admin.CreatedDate = DateTime.Now;
            Admin.CreatedBy = Aspnetuser.Id;
            _context.Admins.Add(Admin);
            _context.SaveChanges();

            if (admindata.RegionIds != null)
            {
                List<int> Regionsid = admindata.RegionIdList.Split(',').Select(int.Parse).ToList();
                foreach (var item in Regionsid)
                {
                    AdminRegion ar = new()
                    {
                        RegionId = item,
                        AdminId = Admin.AdminId
                    };
                    _context.AdminRegions.Add(ar);
                    _context.SaveChanges();
                }
            }
            return true;
        }
        public List<Role> RolePhyscian()
        {
            var role = _context.Roles.Where(r => r.AccountType == 3).ToList();
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
                U.ModifiedDate = DateTime.Now;
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
                    Data.ModifiedDate = DateTime.Now;
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
                Data.Zip = AdminProfile.ZipCode;
                Data.ModifiedDate = DateTime.Now;
                _context.Admins.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public PaginatedViewModel<PhysiciansData> PhysicianAll(int region, int page)
        {
            var pagesize = 5;
            List<PhysiciansData> list = (from r in _context.Physicians
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
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<PhysiciansData> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<PhysiciansData> viewModel = new PaginatedViewModel<PhysiciansData>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;
        }
        public bool changeNoti(int[] files, int region)
        {
            List<PhysicianNotification> PhysicianNotification = (from noti in _context.PhysicianNotifications
                                                                 join
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
            var subject = "Message from admin";
            bool sent = _emailConfig.SendMail(Email, subject, Message).Result;
            EmailLog em = new EmailLog
            {
                EmailTemplate = Message,
                SubjectName = subject,
                EmailId = Email,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1),
                SentTries = 1,
                Action = 7, // action 7 for Message from admin
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
                                     SyncEmailaddress = p.SyncEmailAddress,
                                     Address1 = p.Address1,
                                     Address2 = p.Address2,
                                     City = p.City,
                                     ZipCode = p.Zip,
                                     BusinessName = p.BusinessName,
                                     BusinessWebsite = p.BusinessWebsite,
                                     AdminNotes = p.AdminNotes,
                                     IsNonDisclosureDoc = p.IsNonDisclosureDoc,
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
            Physician data = _context.Physicians.Where(x => x.PhysicianId == physicianid).FirstOrDefault();
            foreach (var i in checkboxes)
            {
                switch (i)
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
            Data.IsDeleted = new BitArray(1);
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


            if (PhysiciansData.RegionIdList != null)
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
            phyNoti.PhysicianId = Data.PhysicianId;
            _context.PhysicianNotifications.Add(phyNoti);
            _context.SaveChanges();

            return true;
        }
        public bool DeleteProvider(int PhysicianId)
        {
            Physician phy = _context.Physicians.Where(x => x.PhysicianId == PhysicianId).FirstOrDefault();
            phy.IsDeleted[0] = true;
            phy.ModifiedDate = DateTime.Now;
            _context.Physicians.Update(phy);
            _context.SaveChanges();
            return true;

        }
        public PaginatedViewModel<Role> AccessAccount(int page)
        {
            var pagesize = 5;
            var list = _context.Roles.Where(r => r.IsDeleted == new BitArray(1)).ToList();
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<Role> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<Role> viewModel = new PaginatedViewModel<Role>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;
        }
        public List<Menu> RolebyAccountType(AccountType Account)
        {
            int accounttype = (int)Account;
            var result = _context.Menus
                      .Where(req => accounttype == 0 || req.AccountType == accounttype)
                      .ToList();
            return result;
        }
        public bool SaveCreateRole(CreateRole roles, string UserId)
        {
            var data = new Role();
            data.Name = roles.Role;
            data.AccountType = (short)roles.AccountType;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = UserId;
            _context.Roles.Add(data);
            _context.SaveChanges();

            List<int> menus = roles.files.Split(',').Select(int.Parse).ToList();

            foreach (var item in menus)
            {
                var obj = new RoleMenu();
                obj.RoleId = data.RoleId;
                obj.MenuId = item;
                _context.RoleMenus.Add(obj);
                _context.SaveChanges();
            }
            return true;
        }
        public CreateRole ViewEditRole(int RoleId)
        {
            CreateRole? v = (from p in _context.Roles

                             where p.RoleId == RoleId
                             select new CreateRole
                             {
                                 Role = p.Name,
                                 AccountType = (AccountType)p.AccountType,
                             }).FirstOrDefault();
            List<Menu> Menu = _context.Menus
                .Where(req => req.AccountType == (short)v.AccountType).ToList();
            v.menus = Menu;
            List<RoleMenu> rm = _context.RoleMenus
                                .Where(obj => obj.RoleId == RoleId).ToList();
            v.rolemenus = rm;
            return v;
        }
        public bool SaveEditRole(CreateRole roles)
        {
            List<int> selectedmenus = roles.files.Split(',').Select(int.Parse).ToList();
            List<int> rolemenus = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).Select(req => req.RoleMenuId).ToList();

            if (rolemenus.Count > 0)
            {
                foreach (var item in rolemenus)
                {
                    RoleMenu ar = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).First();
                    _context.RoleMenus.Remove(ar);
                    _context.SaveChanges();
                }
            }
            foreach (var item in selectedmenus)
            {
                RoleMenu ar = new()
                {
                    RoleId = roles.RoleId,
                    MenuId = item
                };
                _context.RoleMenus.Update(ar);
                _context.SaveChanges();
                //regions.Remove(item);
            }
            return true;
        }
        public bool DeleteRole(int RoleId)
        {
            Role r = _context.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();
            r.IsDeleted[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.Roles.Update(r);
            _context.SaveChanges();
            return true;
        }
        public PaginatedViewModel<UserAccessData> UserAccessData(string AccountType, int page)
        {
            var pagesize = 5;
            var list = (from aspuser in _context.AspNetUsers
                        join admin in _context.Admins
                        on aspuser.Id equals admin.AspNetUserId into AdminGroup
                        from admin in AdminGroup.DefaultIfEmpty()
                        join physician in _context.Physicians
                        on aspuser.Id equals physician.AspNetUserId into PhyGroup
                        from physician in PhyGroup.DefaultIfEmpty()
                        where (admin != null || physician != null) && (admin.IsDeleted == new BitArray(1) || physician.IsDeleted == new BitArray(1))
                        select new UserAccessData
                        {
                            Id = admin != null ? admin.AdminId : (physician != null ? physician.PhysicianId : 0),
                            AccountType = admin != null ? "Admin" : (physician != null ? "Physician" : null),
                            AccountPOC = admin != null ? admin.FirstName + " " + admin.LastName : (physician != null ? physician.FirstName + " " + physician.LastName : null),
                            Status = (int)(admin != null ? admin.Status : (physician != null ? physician.Status : null)),
                            Phone = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                            OpenReq = _context.Requests.Count(r => r.PhysicianId == physician.PhysicianId),
                            isAdmin = admin != null
                        }).ToList();
            if (AccountType != null)
            {
                list = list.Where(r => r.AccountType == "All" || r.AccountType == AccountType).ToList();
            }
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<UserAccessData> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<UserAccessData> viewModel = new PaginatedViewModel<UserAccessData>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;

        }
        public List<PhysicianLocation> FindPhysicianLocation()
        {
            List<PhysicianLocation> pl = _context.PhysicianLocations
                                    .OrderByDescending(x => x.PhysicianName)
                        .Select(r => new PhysicianLocation
                        {
                            LocationId = r.LocationId,
                            Longitude = r.Longitude,
                            Latitude = r.Latitude,
                            PhysicianName = r.PhysicianName
                        }).ToList();
            return pl;

        }
        public PaginatedViewModel<Partners> PartnersData(string searchValue, int Profession, int page)
        {
            var pagesize = 5;
            var list = (from Hp in _context.HealthProfessionals
                        join Hpt in _context.HealthProfessionalTypes
                        on Hp.Profession equals Hpt.HealthProfessionalId into AdminGroup
                        from asp in AdminGroup.DefaultIfEmpty()
                        where (searchValue == null || Hp.VendorName.Contains(searchValue))
                           && (Profession == 0 || Hp.Profession == Profession)
                           && (Hp.IsDeleted == new BitArray(1))
                        select new Partners
                        {
                            VendorId = Hp.VendorId,
                            Profession = asp.ProfessionName,
                            Business = Hp.VendorName,
                            Email = Hp.Email,
                            FaxNumber = Hp.FaxNumber,
                            PhoneNumber = Hp.PhoneNumber,
                            BusinessNumber = Hp.BusinessContact
                        }).ToList();
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<Partners> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<Partners> viewModel = new PaginatedViewModel<Partners>()
            {
                AdminList = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };
            return viewModel;
        }
        public HealthProfessional EditPartners(int VendorId)
        {
            var result = _context.HealthProfessionals.Where(Req => Req.VendorId == VendorId).FirstOrDefault();
            return result;
        }
        public bool EditPartnersData(HealthProfessional hp)
        {
            var Data = _context.HealthProfessionals.Where(req => req.VendorId == hp.VendorId).FirstOrDefault();
            if (Data != null)
            {
                Data.Profession = hp.Profession;
                Data.VendorName = hp.VendorName;
                Data.Email = hp.Email;
                Data.FaxNumber = hp.FaxNumber;
                Data.PhoneNumber = hp.PhoneNumber;
                Data.BusinessContact = hp.BusinessContact;
                Data.Address = hp.Address;
                Data.City = hp.City;
                Data.Zip = hp.Zip;
                Data.State = hp.State;
                _context.HealthProfessionals.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                var data = new HealthProfessional();
                data.Profession = hp.Profession;
                data.VendorName = hp.VendorName;
                data.Email = hp.Email;
                data.FaxNumber = hp.FaxNumber;
                data.PhoneNumber = hp.PhoneNumber;
                data.BusinessContact = hp.BusinessContact;
                data.Address = hp.Address;
                data.City = hp.City;
                data.Zip = hp.Zip;
                data.State = hp.State;
                _context.HealthProfessionals.Add(data);
                _context.SaveChanges();
                return true;
            }
        }
        public bool DeleteBusiness(int VendorId)
        {
            HealthProfessional r = _context.HealthProfessionals.Where(x => x.VendorId == VendorId).FirstOrDefault();
            r.IsDeleted[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.HealthProfessionals.Update(r);
            _context.SaveChanges();
            return true;
        }
        public SearchInputs PatientHistory(SearchInputs search)
        {
            var His = _context.Users
                     .Where(pp => (string.IsNullOrEmpty(search.FirstName) || pp.FirstName.Contains(search.FirstName))
                               && (string.IsNullOrEmpty(search.LastName) || pp.LastName.Contains(search.LastName))
                               && (string.IsNullOrEmpty(search.Email) || pp.Email.Contains(search.Email))
                               && (string.IsNullOrEmpty(search.Mobile) || pp.Mobile.Contains(search.Mobile)))
                     .ToList();
            SearchInputs data = new SearchInputs();

            int totalItemCount = His.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)search.PageSize);
            List<User> list1 = His.Skip((search.CurrentPage - 1) * search.PageSize).Take(search.PageSize).ToList();

            data.CurrentPage = search.CurrentPage;
            data.TotalPages = totalPages;
            data.User = list1;
            return data;
        }
        public List<PatientDashList> RecordsPatientExplore(int UserId)
        {
            List<PatientDashList> allData = (from req in _context.Requests
                                             join reqClient in _context.RequestClients
                                             on req.RequestId equals reqClient.RequestId into reqClientGroup
                                             from rc in reqClientGroup.DefaultIfEmpty()
                                             join phys in _context.Physicians
                                             on req.PhysicianId equals phys.PhysicianId into physGroup
                                             from p in physGroup.DefaultIfEmpty()
                                             where req.UserId == UserId
                                             select new PatientDashList
                                             {
                                                 PatientName = rc.FirstName + " " + rc.LastName,
                                                 RequestedDate = (req.CreatedDate),
                                                 Confirmation = req.ConfirmationNumber,
                                                 Physician = p.FirstName + " " + p.LastName,
                                                 ConcludedDate = req.CreatedDate,
                                                 Status = (status)req.Status,
                                                 RequestTypeId = req.RequestTypeId,
                                                 RequestId = req.RequestId
                                             }).ToList();
            return allData;
        }
        public BlockHistory RecordsBlock(BlockHistory formData)
        {
            var bh = (from req in _context.BlockRequests
                      join r in _context.Requests on req.RequestId equals r.RequestId
                      where (string.IsNullOrEmpty(formData.PatientName) || r.FirstName.Contains(formData.PatientName))
                         && (formData.createdDate == null || req.CreatedDate.Value.Date == formData.createdDate)
                         && (string.IsNullOrEmpty(formData.Email) || req.Email.Contains(formData.Email))
                         && (string.IsNullOrEmpty(formData.Mobile) || req.PhoneNumber.Contains(formData.Mobile))
                      select new PatientDashList
                      {
                          PatientName = r.FirstName,
                          Email = req.Email,
                          createdDate = (DateTime)req.CreatedDate,
                          IsActive = req.IsActive[0],
                          RequestId = Convert.ToInt32(req.RequestId),
                          Mobile = req.PhoneNumber,
                          Notes = req.Reason
                      }).ToList();
            BlockHistory data = new BlockHistory();
            int totalItemCount = bh.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)formData.PageSize);
            List<PatientDashList> list1 = bh.Skip((formData.CurrentPage - 1) * formData.PageSize).Take(formData.PageSize).ToList();

            data.CurrentPage = formData.CurrentPage;
            data.TotalPages = totalPages;
            data.pd = list1;
            return data;
        }
        public bool UnBlock(int reqId)
        {
            BlockRequest r = _context.BlockRequests.Where(x => x.RequestId == reqId).FirstOrDefault();
            r.IsActive[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.BlockRequests.Update(r);
            _context.SaveChanges();

            Request req = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            req.Status = 1;
            req.ModifiedDate = DateTime.Now;
            _context.Requests.Update(req);
            _context.SaveChanges();

            return true;

        }
        public SearchInputs RecordsSearch(SearchInputs rm)
        {
            List<SearchRecords> allData = (from req in _context.Requests
                                           join reqClient in _context.RequestClients
                                           on req.RequestId equals reqClient.RequestId into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _context.Physicians
                                           on req.PhysicianId equals phys.PhysicianId into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join nts in _context.RequestNotes
                                           on req.RequestId equals nts.RequestId into ntsgrp
                                           from nt in ntsgrp.DefaultIfEmpty()
                                           where req.IsDeleted == new BitArray(1) && (rm.ReqStatus == 0 || req.Status == rm.ReqStatus) &&
                                                    (rm.RequestTypeID == 0 || req.RequestTypeId == rm.RequestTypeID) &&
                                                    (!rm.StartDOS.HasValue || req.CreatedDate.Date >= rm.StartDOS.Value.Date) &&
                                                    (!rm.EndDOS.HasValue || req.CreatedDate.Date <= rm.EndDOS.Value.Date) &&
                                                    (rm.PatientName.IsNullOrEmpty() || (req.FirstName + " " + req.LastName).ToLower().Contains(rm.PatientName.ToLower())) &&
                                                    (rm.PhyName.IsNullOrEmpty() || (p.FirstName + " " + p.LastName).ToLower().Contains(rm.PhyName.ToLower())) &&
                                                    (rm.Email.IsNullOrEmpty() || rc.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                                    (rm.Mobile.IsNullOrEmpty() || rc.PhoneNumber.ToLower().Contains(rm.Mobile.ToLower()))
                                           orderby req.CreatedDate
                                           select new SearchRecords
                                           {
                                               //Modifieddate = req.Modifieddate,
                                               PatientName = req.FirstName + " " + req.LastName,
                                               RequestTypeID = req.RequestTypeId,
                                               DateOfService = req.CreatedDate,
                                               Email = rc.Email ?? "-",
                                               Mobile = rc.PhoneNumber ?? "-",
                                               Address = rc.Address + "," + rc.City,
                                               Zip = rc.ZipCode,
                                               Status = (status)req.Status,
                                               Physician = p.FirstName + " " + p.LastName ?? "-",
                                               PhyNotes = nt != null ? nt.PhysicianNotes ?? "-" : "-",
                                               AdminNotes = nt != null ? nt.AdminNotes ?? "-" : "-",
                                               PatientNotes = rc.Notes ?? "-",
                                               RequestID = req.RequestId,
                                               Modifieddate = req.ModifiedDate
                                           }).ToList();

            SearchInputs data = new SearchInputs();

            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SearchRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            data.CurrentPage = rm.CurrentPage;
            data.TotalPages = totalPages;
            data.sr = list1;

            for (int i = 0; i < data.sr.Count; i++)
            {
                if (data.sr[i].Status == (status)9)
                {
                    allData[i].CloseCaseDate = allData[i].Modifieddate;
                }
                else
                {
                    allData[i].CloseCaseDate = null;
                }
                if (allData[i].Status == (status)3 && allData[i].Physician != null)
                {
                    var res = _context.RequestStatusLogs.FirstOrDefault(x => (x.Status == 3) && (x.RequestId == allData[i].RequestID));
                    allData[i].CancelByPhyNotes = res.Notes;
                }
            }

            return data;
        }
        public bool RecordsDelete(int reqId)
        {
            Request hp = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            hp.IsDeleted[0] = true;
            _context.Requests.Update(hp);
            _context.SaveChanges();
            return true;
        }
        public SearchInputs RecordsEmailLog(SearchInputs rm)
        {
            List<EmailLogRecords> allData = (from em in _context.EmailLogs
                                             join req in _context.Requests
                                             on em.RequestId equals req.RequestId into Group
                                             from rc in Group.DefaultIfEmpty()
                                             where (rm.Role == 0 || em.RoleId == rm.Role) &&
                                                   (!rm.StartDOS.HasValue || em.CreateDate.Date == rm.StartDOS.Value.Date) &&
                                                   (!rm.EndDOS.HasValue || em.SentDate == rm.EndDOS.Value.Date) &&
                                                   (rm.FirstName.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(rm.FirstName.ToLower())) &&
                                                   (rm.Email.IsNullOrEmpty() || em.EmailId.ToLower().Contains(rm.Email.ToLower()))
                                             select new EmailLogRecords
                                             {
                                                 
                                                 Recipient = _context.AspNetUsers.Where(req=>req.Email==em.EmailId).Select(req=>req.UserName).FirstOrDefault(),
                                                 ConfirmationNumber = em.ConfirmationNumber,
                                                 CreateDate = em.CreateDate,
                                                 SentDate = (DateTime)em.SentDate,
                                                 RoleId = (AccountType)em.RoleId,
                                                 EmailId = em.EmailId,
                                                 IsEmailSent = (em.IsEmailSent == new BitArray(0) ? "No" : "Yes"),
                                                 SentTries = em.SentTries,
                                                 Action = (EmailAction)em.Action,

                                             }).ToList();
            SearchInputs data = new SearchInputs();
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();
            data.CurrentPage = rm.CurrentPage;
            data.TotalPages = totalPages;
            data.el = list1;
            return data;
        }
        public SearchInputs RecordsSMSLog(SearchInputs rm)
        {
            List<EmailLogRecords> allData = (from em in _context.Smslogs
                                             join req in _context.Requests
                                             on em.RequestId equals req.RequestId into Group
                                             from rc in Group.DefaultIfEmpty()
                                             where (rm.Role == 0 || em.RoleId == rm.Role) &&
                                                   (!rm.StartDOS.HasValue || em.CreateDate.Date == rm.StartDOS.Value.Date) &&
                                                   (!rm.EndDOS.HasValue || em.SentDate == rm.EndDOS.Value.Date) &&
                                                   (rm.FirstName.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(rm.FirstName.ToLower())) &&
                                                   (rm.Mobile.IsNullOrEmpty() || em.MobileNumber.ToLower().Contains(rm.Mobile.ToLower()))
                                             select new EmailLogRecords
                                             {
                                                 Recipient = rc.FirstName,
                                                 ConfirmationNumber = em.ConfirmationNumber,
                                                 CreateDate = em.CreateDate,
                                                 SentDate = (DateTime)em.SentDate,
                                                 RoleId = (AccountType)em.RoleId,
                                                 Mobile = em.MobileNumber,
                                                 IsEmailSent = (em.IsSmssent == new BitArray(0) ? "No" : "Yes"),
                                                 SentTries = em.SentTries,
                                                 Action = (EmailAction)em.Action,

                                             }).ToList();
            SearchInputs data = new SearchInputs();
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogRecords> list1 = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();
            data.CurrentPage = rm.CurrentPage;
            data.TotalPages = totalPages;
            data.el = list1;
            return data;
        }

    }
}
