using System;

namespace ParkHyderabadOperator.Model.APIInputModel
{
    public class ViolationVehicleFees
    {
        public int CustomerParkingSlotId { get; set; }
        public DateTime ParkingStartTime { get; set; }
        public DateTime ParkingEndTime { get; set; }
        public string VehicleTypeCode { get; set; }
        public int VehicleTypeID { get; set; }
        public int LocationParkingLotID { get; set; }
        public decimal ParkingFee { get; set; }
        public decimal ClampFee { get; set; }
        public decimal TotalFee { get; set; }
        public int TotalHours { get; set; }
    }
}