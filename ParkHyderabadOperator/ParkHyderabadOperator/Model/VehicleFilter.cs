using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model
{
   public class VehicleFilter
    {
        public int FilterID { get; set; }
        public string FilterName { get; set; }
        public bool IsSelected { get; set; }
        public string ApplicationTypeCode { get; set; }
        public string StatusCode { get; set; }
    }
}
