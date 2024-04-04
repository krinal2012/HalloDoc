using HalloDoc.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Repository.Interface
{
    public interface IScheduling
    {
        public void AddShift(SchedulingData model, List<string?>? chk, string adminId);
        public void ViewShift(int shiftdetailid);
        public void ViewShiftreturn(SchedulingData modal);
    }
}
