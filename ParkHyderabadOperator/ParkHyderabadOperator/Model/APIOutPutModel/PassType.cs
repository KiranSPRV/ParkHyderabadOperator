using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class PassType
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
    }
}