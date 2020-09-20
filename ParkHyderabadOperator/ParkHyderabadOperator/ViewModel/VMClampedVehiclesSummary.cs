using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.Report;
using System;
using System.Collections.Generic;
namespace ParkHyderabadOperator.ViewModel
{
    public class VMClampedVehiclesSummary
    {

        public VMClampedVehiclesSummary()
        {
           
        }
        public List<StationClampedReport> StationClampedReport { get; set; }
        public int TotalClamp { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalEPay { get; set; }
        public string Currency { get; set; }
    }
}
