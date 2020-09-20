using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model
{
   public class LocationParkingReport
    {
        public string VehicleType { get; set; }
        public string ParkingHours { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string Cash { get; set; }
        public string Epay { get; set; }
        public string FOC { get; set; }
    }
}
