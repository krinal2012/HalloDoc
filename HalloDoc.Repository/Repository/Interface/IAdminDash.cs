using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc.Entity.Models;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IAdminDash
    {
        public List<AdminList> NewRequestData();
    }
}
