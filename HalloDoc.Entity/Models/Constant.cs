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
    }
}
