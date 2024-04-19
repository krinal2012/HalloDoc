namespace HalloDoc.Entity.Models
{
    public class Constant
    {
        public enum AccountType
        {
            All=0,
            Patient =1,
            Admin=2,
            Physician=3,
        }
        public enum RequestTypes
        {
            Business = 1,
            Patient=2,
            Family = 3,
            Concierge=4
        }
        public enum status
        {
            Unassigned = 1,
            Accepted = 2,
            Cancelled = 3,
            MDEnRoute = 4,
            MDOnSite = 5,
            Conclude = 6,
            CancelledByPatient = 7,
            Closed =8,
            Unpaid =9,
            Clear=10,
            Block=11
        }
        public enum RegionList
        {
            Gujarat=1,
            Rajsthan =2,
            Maharastra=3,
            Delhi=4
        }
        public enum state
        {
            Active = 1,
            Pending = 2,
        }
        public enum EmailAction
        {
            SendOrder = 1,
            Request,
            SendLink,
            SendAgreement,
            Forgot,
            NewRegistration,
            contact
        }
    }
}
