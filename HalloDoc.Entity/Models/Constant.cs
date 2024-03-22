using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Entity.Models
{
    public class Constant
    {
        public enum RequestTypes
        {
            Business = 1,
            Patient=2,
            Family = 3,
            Concierge=4
        }
        public enum status
        {
            Unassigned = 1,
            Accepted = 2,
            Cancelled = 3,
            MDEnRoute = 4,
            MDOnSite = 5,
            Conclude = 6,
            CancelledByPatient = 7,
            Closed =8,
            Unpaid =9,
            Clear=10,
            Block=11
        }
    }
}
