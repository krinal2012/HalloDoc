                                                                                                                                                                                                                                                                                                        using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDoc.Entity.Models.Constant;


namespace HalloDoc.Entity.Models.ViewModel
{
    public class PatientDashList
    {       
            public DateTime createdDate { get; set; }
            public status Status { get; set; }
            public int RequestId { get; set; }
            public int Fcount { get; set; }
    }
}
