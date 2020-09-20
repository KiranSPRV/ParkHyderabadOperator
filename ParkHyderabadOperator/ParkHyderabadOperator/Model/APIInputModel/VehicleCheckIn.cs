using System;

namespace ParkHyderabadOperator.Model.APIInputModel
{
    public class VehicleCheckIn
    {
        public int CustomerParkingSlotID { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int BayNumberID { get; set; }
        public string BayNumber { get; set; }
        public string BayRange { get; set; }
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }
        public string RegistrationNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string NFCCardNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int ParkingHours { get; set; }
        public int ParkingMinutes { get; set; }
        public decimal ParkingFees { get; set; }
        public decimal ClampFees { get; set; }
        public string PaymentType { get; set; }
        public bool PaymentReceived { get; set; }
        public string ParkingStartTime { get; set; }
        public string ParkingEndTime { get; set; }
        public string StatusName { get; set; }
        public byte[] GovernmentVehicleImage { get; set; }
        public decimal VehicleImageLottitude { get; set; }
        public decimal VehicleImageLongitude { get; set; }
    }
}
