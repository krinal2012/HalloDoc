using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminTabs
    {

        public List<Role> Role();
        public AdminProfile ViewAdminProfile(string UserId);
        public bool ProfilePassword(string Password, int UserId);
        public bool EditAdministratorInfo(AdminProfile AdminProfile);
        public bool EditBillingInfo(AdminProfile AdminProfile);
        public List<PhysiciansData> PhysicianAll(int region);
        public bool changeNoti(int[] files, int region);
        public bool ContactProviderMail(string Email, string Message);
        public PhysiciansData ViewProviderProfile(int PhysicianId);
        public bool EditPassword(string Password, int UserId);
        public bool EditAdministrator(PhysiciansData PhysiciansData);
        public bool EditBilling(PhysiciansData AdminProfile);
        public bool EditProviderProfile(PhysiciansData PhysiciansData);
        public bool SaveProvider(int[] checkboxes, int physicianid);
        public bool AddProviderAccount(PhysiciansData PhysiciansData, int[] checkboxes, string UserId);
        public bool DeleteProvider(int PhysicianId);
        public List<Menu> RolebyAccountType(AccountType Account);
        public bool SaveCreateRole(CreateRole roles, string UserId);
        public CreateRole ViewEditRole(int RoleId);
        public bool SaveEditRole(CreateRole roles);
        public bool DeleteRole(int RoleId);
    }
}
