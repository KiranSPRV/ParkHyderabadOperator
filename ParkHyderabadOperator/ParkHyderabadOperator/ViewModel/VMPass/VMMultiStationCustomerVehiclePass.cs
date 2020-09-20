using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.VMPass
{
   public class VMMultiStationCustomerVehiclePass
    {
        public VMMultiStationCustomerVehiclePass()
        {
            CustomerVehiclePassID = new CustomerVehiclePass();
            LocationID = new List<Location>();
        }
        public CustomerVehiclePass CustomerVehiclePassID { get; set; }
        public List<Location> LocationID { get; set; }
    }
}
