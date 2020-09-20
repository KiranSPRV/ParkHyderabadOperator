using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIInputModel
{
   public class ViolationAndClamp
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int BayNumberID { get; set; }
        public string BayNumber { get; set; }
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }
        public string RegistrationNumber { get; set; }
        public byte[] ViolationImage { get; set; }
        public bool IsClamp { get; set; }
        public bool IsWarning { get; set; }
        public int ReasonID { get; set; }
        public string ReasonName { get; set; }
        public DateTime ViolationStartTime { get; set; }
        public string ViolationTime { get; set; }

        public decimal VehicleImageLottitude { get; set; }

        public decimal VehicleImageLongitude { get; set; }
    }
}
