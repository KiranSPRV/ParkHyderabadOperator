using ParkHyderabadOperator.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.RecentCheckOuts
{
   public class RecentCheckOutReport
    {
        public RecentCheckOutReport()
        {
            RecentCheckOutID = new List<VMRecentCheckOuts>();
        }
        public List<VMRecentCheckOuts> RecentCheckOutID { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalEpay { get; set; }
    }
}
