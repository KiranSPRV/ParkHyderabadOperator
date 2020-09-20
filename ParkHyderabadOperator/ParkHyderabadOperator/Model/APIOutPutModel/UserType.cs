using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class UserType
    {
        public int UserTypeID { get; set; }
        public string UserTypeCode { get; set; }
        public string UserTypeName { get; set; }
        public string UserTypeDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}