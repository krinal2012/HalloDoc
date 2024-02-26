using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class viewDocument
    {
        public string uploader { get; set; }
        public DateTime? uploaddate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RequestId { get; set; }
        public string Filename { get; set; }

    }
}
