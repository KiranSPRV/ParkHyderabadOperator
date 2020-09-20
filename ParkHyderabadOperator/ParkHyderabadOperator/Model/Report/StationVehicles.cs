using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.Report
{
   public class StationVehicles
    {
        public StationVehicles()
        {
            VehicleID = new Vehicle();
            LocationID = new Location();
            
        }
        public Vehicle VehicleID { get; set; }
        public Location LocationID { get; set; }
        public int StationVehiclesCount { get; set; }
        public decimal StationVehicleCash { get; set; }
        public decimal StationVehicleEPay { get; set; }
        public string Currency { get; set; }
    
    }
}
