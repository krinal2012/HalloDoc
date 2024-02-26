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
            New = 1,
            Pending = 2,
            Active = 3,
            Conclude = 4,
            Close = 5,
            Unpaid = 6

        }
    }
}
