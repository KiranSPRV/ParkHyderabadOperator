using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIInputModel
{
    public class UserLogin
    {
        public string Password { get; set; }
        public string UserName { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogOutTime { get; set; }
        public string LoginDeviceID { get; set; }
    }
}