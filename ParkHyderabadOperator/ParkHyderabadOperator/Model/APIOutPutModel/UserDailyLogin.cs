using System;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class UserDailyLogin
    {
        public UserDailyLogin()
        {
            UserID = new User();
            LocationParkingLotID = new LocationParkingLot();
        }
        public int UserDailyLoginID { get; set; }
        public User UserID { get; set; }
        public LocationParkingLot LocationParkingLotID { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime? TimeSheetDate { get; set; }
        public string LoginTime { get; set; }
        public string LogoutTime { get; set; }
        public string NoofHours { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? HistoryFromDate { get; set; }
        public DateTime? HistoryToDate { get; set; }
    }
}
