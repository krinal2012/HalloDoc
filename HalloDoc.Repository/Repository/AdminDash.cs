using HalloDoc.Entity.DataContext;
using HalloDoc.Repository.Repository.Interface;
using HalloDoc.Entity.Models.ViewModel;
using HalloDoc.Entity.DataModels;

namespace HalloDoc.Repository.Repository
{
    public class AdminDash : IAdminDash
    {
        private readonly HelloDocContext _context;
        public AdminDash(HelloDocContext context)
        {
            _context = context;
        }
        public List<AdminList> NewRequestData()
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
        public ViewCaseModel ViewCaseData(int? RequestID)
        {
            ViewCaseModel list = _context.Requests.Join
                        (_context.RequestClients,
                        requestclients => requestclients.RequestId, requests => requests.RequestId,
                        (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                        ).Where(req => req.Request.RequestId== RequestID)
                        .Select(req=> new ViewCaseModel()
                        {
                            RequestTypeId = req.Request.RequestTypeId,
                            ConfNo = req.Requestclient.City.Substring(0, 2) + req.Requestclient.IntDate.ToString() + req.Requestclient.StrMonth + req.Requestclient.IntYear.ToString() + req.Requestclient.LastName.Substring(0, 2) + req.Requestclient.FirstName.Substring(0, 2) + "002",
                            Symptoms = req.Requestclient.Notes,
                            FirstName = req.Requestclient.FirstName,
                            LastName = req.Requestclient.LastName,
                            DOB = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
                            Mobile = req.Requestclient.PhoneNumber,
                            Email = req.Requestclient.Email,
                            Address = req.Requestclient.Address
                        }).FirstOrDefault();
            return list;
        }
    }
}
