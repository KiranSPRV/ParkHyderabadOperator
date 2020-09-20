namespace ParkHyderabadOperator.Model.APIInputModel
{
    public class CheckInVehiclePass
    {
        public string RegistrationNumber { get; set; }
        public int LocationID { get; set; }
        public int UserID { get; set; }
        public int LocationParkingLotID { get; set; }
        public string NFCCardNumber { get; set; }
    }
}