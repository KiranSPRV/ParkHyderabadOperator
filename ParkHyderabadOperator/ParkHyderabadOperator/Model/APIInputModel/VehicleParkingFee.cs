using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIInputModel
{
  public  class VehicleParkingFee
    {
        public int UserID { get; set; }
        public int VehicleTypeID { get; set; }
        public int LocationParkingLotID { get; set; }
        public int ParkingHours { get; set; }
        public int Duration { get; set; }
        public string VehicleTypeCode { get; set; }
        public decimal Fees { get; set; }
        public decimal PaidAmount { get; set; }
        public string SpotExpireTime { get; set; }
        public string ParkingStartTime { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsFullDay { get; set; }
    }
}
    