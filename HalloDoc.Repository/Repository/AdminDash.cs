using HalloDoc.Entity.DataContext;
using HalloDoc.Repository.Repository.Interface;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataModels;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Repository.Repository
{
    public class AdminDash : IAdminDash
    {
        private readonly HelloDocContext _context;
        public AdminDash(HelloDocContext context)
        {
            _context = context;
        }
        public List<AdminList> NewRequestData(int statusid)
        {

            var list = _context.Requests.Join
                        (_context.RequestClients,
                        requestclients => requestclients.RequestId, requests => requests.RequestId,
                        (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                        )
                        .Where(req => req.Request.Status == 1)
                        .Select(req => new AdminList()
                        
                         {
                            RequestId = req.Request.RequestId,
                            PatientName = req.Requestclient.FirstName + " " + req.Requestclient.LastName,
                            Email = req.Requestclient.Email,
                            DateOfBirth = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
                            RequestTypeId = req.Request.RequestTypeId,
                            Requestor = req.Request.FirstName + " " + req.Request.LastName,
                            RequestedDate = req.Request.CreatedDate,
                            PatientPhoneNumber = req.Requestclient.PhoneNumber,
                            RequestorPhoneNumber = req.Request.PhoneNumber,
                            Notes = req.Requestclient.Notes,
                            Address =  req.Requestclient.Street + ", " + req.Requestclient.City + ", " + req.Requestclient.State + " " + req.Requestclient.ZipCode
                        })
                        .OrderByDescending(req => req.RequestedDate)
                        .ToList();
            return list;                                   
        }
        public ViewCaseModel ViewCaseData(int RequestID,int RequestTypeId)
        {
            ViewCaseModel list = 
                        _context.RequestClients
                       .Where(req => req.Request.RequestId== RequestID)
                        .Select(req=> new ViewCaseModel()
                        {
                            RequestTypeId = RequestTypeId,
                            ConfNo = req.Address.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                            Symptoms = req.Notes,
                            FirstName = req.FirstName,
                            LastName = req.LastName,
                            DOB = new DateTime((int)req.IntYear, Convert.ToInt32(req.StrMonth.Trim()), (int)req.IntDate),
                            Mobile = req.PhoneNumber,
                            Email = req.Email,
                            Address = req.Address
                        }).FirstOrDefault();
            return list;
        }
    }
}
