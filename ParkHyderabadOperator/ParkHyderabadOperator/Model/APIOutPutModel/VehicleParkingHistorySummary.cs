using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class VehicleParkingHistorySummary
    {
        
        public CustomerVehicle CustomerVehicleID { get; set; }
        public Customer CustomerID { get; set; }
        public Location LocationID { get; set; }
        public decimal ParkingFees { get; set; }
        public string ParkingFrom { get; set; }
        public string ParkingTo { get; set; }
        public User OperatorID { get; set; }
        public string PaymentType { get; set; }
        public string ParkingTimmingsTextColor { get; set; }
    }
}
