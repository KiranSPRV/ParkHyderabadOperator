using ParkHyderabadOperator.Model.Report;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.Reports
{
    public class VMLocationLotPassReport
    {
        public VMLocationLotPassReport()
        {

        }
        public List<StationPassesReport> StationPasses { get; set; }
        public int TotalSold { get; set; }
        public int TotalNFC { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalEPay { get; set; }
        public string Currency { get; set; }
    }
}
