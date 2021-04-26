

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class User
    {
        public User()
        {
            UserTypeID = new UserType();
            LocationParkingLotID = new LocationParkingLot();
        }
        public int UserID { get; set; }
        public UserType UserTypeID { get; set; }
        public LocationParkingLot LocationParkingLotID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int? SupervisorID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string LoginDeviceID { get; set; }
        public byte[] Photo { get; set; }
    }
}