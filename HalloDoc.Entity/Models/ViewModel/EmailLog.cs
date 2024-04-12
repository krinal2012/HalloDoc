using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class EmailLogRecords
    {
        public string Recipient { get; set; }
        public string EmailId { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string? ConfirmationNumber { get; set; }
        public AccountType RoleId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SentDate { get; set; }
        public string IsEmailSent { get; set; }
        public int? SentTries { get; set; }
        public EmailAction Action { get; set; }
    }
}
