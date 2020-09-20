using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.Report;
namespace ParkHyderabadOperator.ViewModel
{
  public  class VMVehiclePassReport
    {
        
        public VMVehiclePassReport()
        {
            StationPasses = new List<StationPassesReport>();
        }
        public List<StationPassesReport> StationPasses { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalEPay { get; set; }
        public string Currency { get; set; }
    }
}
