using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.Reports
{
   public class VMUserDailyLogin
    {
        public VMUserDailyLogin()
        {
            UserDailyLoginID = new List<UserDailyLogin>();
        }
        public List<UserDailyLogin> UserDailyLoginID { get; set; }

        public string SuperVisorName { get; set; }
        public int WorkedDays { get; set; }
        public int AbsentDays { get; set; }
        public string TotalHours { get; set; }
    }
}
