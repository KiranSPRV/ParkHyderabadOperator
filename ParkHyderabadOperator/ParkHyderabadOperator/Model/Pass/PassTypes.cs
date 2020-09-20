using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.Pass
{
   public class PassTypes
    {
        public int PassTypeID { get; set; }
        public string PassTypeCode { get; set; }
        public string PassTypeName { get; set; }
        public string PassTypeDesc { get; set; }
        public int StartRange { get; set; }
        public int EndRange { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

       //Remove below columns after db changes
        public string PassType { get; set; }
        public decimal PassPrice { get; set; }
        public string PassValidationDuration { get; set; }
        public DateTime PassValidateFrom { get; set; }
        public DateTime PassValidateTo { get; set; }

    }
}
