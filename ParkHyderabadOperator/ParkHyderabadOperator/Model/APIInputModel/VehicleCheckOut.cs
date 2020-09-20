using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIInputModel
{
   public class VehicleCheckOut
    {
        public int CustomerParkingSlotID { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public int LocationID { get; set; }
        public int BayNumberID { get; set; }
        public int LocationParkingLotID { get; set; }
        public string VehicleTypeID { get; set; }
        public string VehicleTypeCode { get; set; }
        public int CustomerID { get; set; }
        public int CustomerVehicleID { get; set; }
        public string ParkingStartTime { get; set; }
        public string ParkingEndTime { get; set; }
        public string ParkingActualStartTime { get; set; }
        public string ParkingActualEndTime { get; set; }
        public int ParkingHours { get; set; }
        public decimal Amount { get; set; }
        public decimal ExtendAmount { get; set; }
        public decimal ClampFees { get; set; }
        public decimal PaidAmount { get; set; }
        public bool IsClamp { get; set; }
        public int ViolationWarningCount { get; set; }
        public string PaymentType { get; set; }
        public int StatusID { get; set; }
        public int ApplicationID { get; set; }

    }
}
