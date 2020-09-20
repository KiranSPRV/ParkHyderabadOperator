using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class Status
    {
        public int StatusID { get; set; }
        public string StatusCode { get; set; }
        public bool ShowStatus { get; set; }
        public string StatusName { get; set; }
        public string StatusDesc { get; set; }
        public string StatusColor { get; set; }
        public string StatusImage { get; set; }
        public bool ShowStatusImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}