using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.Pass
{
   public class PassPrice
    {
        public PassPrice()
        {
            PassTypeID = new PassTypes();
            VehicleTypeID = new VehicleType();
        }
        public int PassPriceID { get; set; }
        public PassTypes PassTypeID { get; set; }
        public string PassCode { get; set; }
        public string PassName { get; set; }
        public string StationAccess { get; set; }
        public string Duration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool NFCApplicable { get; set; }
        public decimal NFCCardPrice { get; set; }
        public VehicleType VehicleTypeID { get; set; }
        public decimal Price { get; set; }
        public string PassDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal TotalPassPrice { get; set; }

    }
}
