
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class PassCardTypeMapper
    {
        public PassCardTypeMapper()
        {

            PassTypeID = new PassType();
        }
        public int PassCardTypeMapperID { get; set; }
        public PassType PassTypeID { get; set; }
        public int CardTypeID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}