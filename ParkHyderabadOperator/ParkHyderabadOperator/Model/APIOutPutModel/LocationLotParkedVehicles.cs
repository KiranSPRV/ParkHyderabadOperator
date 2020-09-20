using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class LocationLotParkedVehicles
    {
        public int CustomerParkingSlotID { get; set; }
        public string VehicleImage { get; set; }
        public string RegistrationNumber { get; set; }
        public string ParkingBayName { get; set; }
        public string ParkingBayRange { get; set; }
        public string BayNumberColor { get; set; }
        public string ApplicationTypeCode { get; set; }
        public int StatusID { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string VehicleStatusColor { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleClampImage { get; set; }
        public bool IsClamp { get; set; }
    }
}
