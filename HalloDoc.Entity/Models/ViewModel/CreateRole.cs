using HalloDoc.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HalloDoc.Entity.Models.Constant;

namespace HalloDoc.Entity.Models.ViewModel
{
    public class CreateRole
    {
        public int RoleId { get; set; }
        public string Role { get; set; }
        public AccountType AccountType { get; set; }
        public string files { get; set; }
        public List<Menu> menus { get; set; }
        public List<RoleMenu> rolemenus { get; set; }
    }
}
