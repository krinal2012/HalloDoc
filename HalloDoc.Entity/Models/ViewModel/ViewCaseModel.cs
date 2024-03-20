using HalloDoc.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class ViewCaseModel
    {
        public int RequestTypeId { get; set; }
        public int Status { get; set; }
        public int RequestId { get; set; }
        public string ConfNo { get; set; }
        public string Symptoms { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string? Mobile { get; set; }
        public string Email { get; set; }
        public string? Region { get; set; }
        public string Address { get; set; }
        public string? Room { get; set; }
        public List<RequestWiseFile> documents { get; set; }

    }
}
