using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel
{
   public class VMVehiclePassWithDueAmount
    {
        public CustomerVehiclePass CustomerVehiclePassID { get;set;}
        public decimal VehicleDueAmount { get; set; }
    }
}
