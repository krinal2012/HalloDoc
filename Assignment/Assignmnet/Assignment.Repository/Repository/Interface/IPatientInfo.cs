using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Repository.Interface
{
    public interface IPatientInfo
    {
        public PaginatedViewModel<Patient> RequestData(PaginatedViewModel<Patient> ls);
        public bool AddPatient(PaginatedViewModel<Patient> ls);
        public Patient editPatientModal(int id);
        public bool editPatientPost(Patient formData);
        public bool DeletePatient(int id);
    }
}
