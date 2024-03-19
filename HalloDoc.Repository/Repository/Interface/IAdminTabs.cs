using HalloDoc.Entity.Models.ViewModel;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminTabs
    {
        public AdminProfile ViewAdminProfile(string UserId);
        public bool EditPassword(string Password, int UserId);
        public bool EditAdministratorInfo(AdminProfile AdminProfile);
        public bool EditBillingInfo(AdminProfile AdminProfile);
    }
}
