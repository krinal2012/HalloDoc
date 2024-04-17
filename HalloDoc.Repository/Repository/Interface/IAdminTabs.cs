using HalloDoc.Entity.DataModels;
using HalloDoc.Entity.Models.ViewModel;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminTabs
    {
        public List<Role> RolePhyscian();
        public List<Role> RoleAdmin();
        public AdminProfile ViewAdminProfile(string UserId);
        public bool AddAdminAccount(AdminProfile admindata);
        public bool ProfilePassword(string Password, int UserId);
        public bool EditAdministratorInfo(AdminProfile AdminProfile);
        public bool EditBillingInfo(AdminProfile AdminProfile);
        public PaginatedViewModel<PhysiciansData> PhysicianAll(int region, int page);
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
        public PaginatedViewModel<Role> AccessAccount(int page);
        public List<Menu> RolebyAccountType(AccountType Account);
        public bool SaveCreateRole(CreateRole roles, string UserId);
        public CreateRole ViewEditRole(int RoleId);
        public bool SaveEditRole(CreateRole roles);
        public bool DeleteRole(int RoleId);
        public PaginatedViewModel<UserAccessData> UserAccessData(string AccountType, int page);
        public List<PhysicianLocation> FindPhysicianLocation();
        public PaginatedViewModel<Partners> PartnersData(string searchValue, int Profession, int page);
        public HealthProfessional EditPartners(int VendorId);
        public bool EditPartnersData(HealthProfessional hp);
        public bool DeleteBusiness(int VendorId);
        public SearchInputs PatientHistory(SearchInputs search);
        public List<PatientDashList> RecordsPatientExplore(int UserId);
        public BlockHistory RecordsBlock(BlockHistory Formdata);
        public bool UnBlock(int reqId);
        public SearchInputs RecordsSearch(SearchInputs search);
        public bool RecordsDelete(int reqId);
        public SearchInputs RecordsEmailLog(SearchInputs search);
        public SearchInputs RecordsSMSLog(SearchInputs search);
        public bool ContactAdmin(int ProviderId, string Notes);
    }
}
