﻿using HalloDoc.Entity.DataModels;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class viewDocument
    {
        public string FileName { get; set; }
        public DateTime? uploaddate { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RequestId { get; set; }
        public string ConfirmationNumber { get; set; }
        //public string Filename { get; set; }
        public string isDeleted { get; set; }
        public List<RequestWiseFile> Files { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string sortColumn { get; set; } = "RequestedDate";
        public bool sortOrder { get; set; } = false;

    }
}
