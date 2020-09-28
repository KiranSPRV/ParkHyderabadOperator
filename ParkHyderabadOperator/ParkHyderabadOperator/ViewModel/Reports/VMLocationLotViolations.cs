using ParkHyderabadOperator.Model.Report;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.Reports
{
   public class VMLocationLotViolations
    {
        public VMLocationLotViolations()
        {
        }
       
        public int TotalClamp { get; set; }
        public int TotalWarningClamps { get; set; }
        public int TotalUnPaidClamps { get; set; }
        public int TotalPaidClamps { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalEPay { get; set; }
        public string Currency { get; set; }
        public List<StationClampedReport> LocationLotViolationReport { get; set; }
    }
}
