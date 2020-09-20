using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.Report;

namespace ParkHyderabadOperator.ViewModel.Reports
{
   public class VMStationVehicles
    {
        public VMStationVehicles()
        {
            StationVehiclesID = new List<StationVehicles>();
        }
        public List<StationVehicles> StationVehiclesID { get; set; }
        public int TotalVehicles { get; set; }
        public decimal TotalVehiclesCash { get; set; }
        public decimal TotalVehiclesEPay { get; set; }
        public string Currency { get; set; }
    }
}
