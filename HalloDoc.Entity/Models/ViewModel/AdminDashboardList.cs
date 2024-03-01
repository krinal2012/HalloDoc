namespace HalloDoc.Entity.Models.ViewModel
{
    public class AdminList
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }
        public string ProviderName { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
        public int RequestTypeId { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string RequestorPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
    }

    public class CountStatusWiseRequestModel
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
          
    }
}


