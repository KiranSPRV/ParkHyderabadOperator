using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class ParkingBay
    {
        public ParkingBay()
        {
            VehicleTypeID = new VehicleType();
            
        }
        public int ParkingBayID { get; set; }
        public string ParkingBayCode { get; set; }
        public string ParkingBayName { get; set; }
        public string ParkingBayRange { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public VehicleType VehicleTypeID { get; set; }
       
    }
}
