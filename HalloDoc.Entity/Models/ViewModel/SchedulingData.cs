using HalloDoc.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Entity.Models.ViewModel
{

    public class SchedulingData
    {
        [Required(ErrorMessage = "Please select any region.")]
        public int regionid { get; set; }
        [Required(ErrorMessage = "Please select one Physician.")]
        public int physicianid { get; set; }
        public string RegionName { get; set; }
        [Required(ErrorMessage = "Please select appropriate date.")]
        public DateTime shiftdate { get; set; }
        public short status { get; set; }
        public TimeOnly starttime { get; set; }
        [GreaterThan("starttime", ErrorMessage = "End time must be later than start time.")]
        public TimeOnly endtime { get; set; }
        public int repeatcount { get; set; }
        public string physicianname { get; set; }
        public string modaldate { get; set; }
        public int shiftdetailid { get; set; }
    }
    public class DayWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
    public class MonthWiseScheduling
    {
        public DateTime date { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
    public class WeekWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
    public class GreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public GreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (TimeOnly)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (TimeOnly)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
