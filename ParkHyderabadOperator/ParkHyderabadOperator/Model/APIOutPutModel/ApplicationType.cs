using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class ApplicationType
    {
        public int ApplicationTypeID { get; set; }
        public string ApplicationTypeCode { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ApplicationTypeDesc { get; set; }
        public string ApplicationTypeColor { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}