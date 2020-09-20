using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.Reports
{
   public class VMReportSummary
    {
        public VMReportSummary()
        {
            VMLocationLotParkingReportID = new VMLocationLotParkingReport();
            VMLocationLotPassReportID = new VMLocationLotPassReport();
            VMLocationLotViolationsID = new VMLocationLotViolations();
        }

        public decimal Cash { get; set; }
        public decimal EPay { get; set; }
        public VMLocationLotParkingReport VMLocationLotParkingReportID { get; set; }
        public VMLocationLotPassReport VMLocationLotPassReportID { get; set; }
        public VMLocationLotViolations VMLocationLotViolationsID { get; set; }

       
    }
}
