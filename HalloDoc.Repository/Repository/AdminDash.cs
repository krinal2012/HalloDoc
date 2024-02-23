using HalloDoc.Entity.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.Entity.Models;
using HalloDoc.Repository.Repository.Interface;
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
    }
}
