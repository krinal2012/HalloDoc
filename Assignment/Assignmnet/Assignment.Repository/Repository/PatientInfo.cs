using Assignment.Entity.DataContext;
using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using Assignment.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Repository
{
    public class PatientInfo : IPatientInfo
    {
        private readonly HospitalDbContext _context;
        public PatientInfo(HospitalDbContext context)
        {
            _context = context;
        }
        public PaginatedViewModel<Patient> RequestData(PaginatedViewModel<Patient> ls)
        {
            var searchValue = "";
            if (ls.SearchValue != null)
            {
                searchValue = ls.SearchValue.ToLower();
            }

            var list = (from p in _context.Patients
                        join d in _context.Doctors
                        on p.DoctortId equals d.DoctorId into Group
                        from pd in Group.DefaultIfEmpty()
                        where (searchValue == null ||
                               p.FirstName.ToLower().Contains(searchValue) || p.LastName.ToLower().Contains(searchValue) ||
                               p.Email.ToLower().Contains(searchValue) || p.PhoneNo.Contains(searchValue) ||
                               p.Gender.ToLower().Contains(searchValue) || p.Disease.ToLower().Contains(searchValue) ||
                               p.Specialist.ToLower().Contains(searchValue))
                        select new Patient
                        {
                            Id = p.Id,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Email = p.Email,
                            Age = p.Age,
                            PhoneNo = p.PhoneNo,
                            Gender = p.Gender,
                            Disease = p.Disease,
                            Specialist = p.Specialist,

                        }).ToList();

            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)ls.PageSize);
            List<Patient> list1 = list.Skip((ls.CurrentPage - 1) * ls.PageSize).Take(ls.PageSize).ToList();

            PaginatedViewModel<Patient> viewModel = new PaginatedViewModel<Patient>()
            {
                List = list1,
                CurrentPage = ls.CurrentPage,
                TotalPages = totalPages,
                TotalCount = totalItemCount,
            };
            return viewModel;
        }
        public bool AddPatient(PaginatedViewModel<Patient> ls)
        {
            if (ls == null)
            {
                return false;
            }
            var patient = new Patient();
            patient.FirstName = ls.FirstName;
            patient.LastName = ls.LastName;
            patient.Email = ls.Email;
            patient.Age = ls.Age;
            patient.PhoneNo = ls.PhoneNo;
            patient.Gender = ls.Gender;
            patient.Disease = ls.Disease;
            patient.Specialist = ls.Specialist;
            patient.DoctortId = _context.Doctors.Where(res=> res.Specialist == ls.Specialist).Select(res=>res.DoctorId).FirstOrDefault();
            if(patient.DoctortId == 0)
            {
                var doctor = new Doctor();
                doctor.Specialist = ls.Specialist;
                _context.Doctors.Add(doctor);
                _context.SaveChanges();
                patient.DoctortId = doctor.DoctorId;
            }
            _context.Patients.Add(patient);
            _context.SaveChanges();
            return true;
        }
        public Patient editPatientModal(int id)
        {
            var res = _context.Patients.FirstOrDefault(req => req.Id == id);
            return res;
        }
        public bool editPatientPost(Patient formData)
        {
            try
            {
                var Data = _context.Patients.First(W => W.Id == formData.Id);
                if (Data != null)
                {
                    Data.FirstName = formData.FirstName;
                    Data.LastName = formData.LastName;
                    Data.PhoneNo = formData.PhoneNo;
                    Data.Email = formData.Email;
                    Data.Age = formData.Age;
                    Data.Disease = formData.Disease;
                    Data.Gender = formData.Gender;
                    Data.Specialist = formData.Specialist;
                    Data.DoctortId = _context.Doctors.Where(res => res.Specialist == formData.Specialist).Select(res => res.DoctorId).FirstOrDefault();
                    if (Data.DoctortId == 0)
                    {
                        var doctor = new Doctor();
                        doctor.Specialist = formData.Specialist;
                        _context.Doctors.Add(doctor);
                        _context.SaveChanges();
                        Data.DoctortId = doctor.DoctorId;
                    }
                    _context.Patients.Update(Data);
                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool DeletePatient(int id)
        {
            var Data = _context.Patients.Where(W => W.Id == id).FirstOrDefault();
            if (Data != null)
            {
                _context.Patients.Remove(Data);
                _context.SaveChanges();
                return true;
            }
            else
                return false;
        }

    }
}
