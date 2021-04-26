using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.VMHome
{
    public class VMLocationLotParkedVehicles
    {
        public VMLocationLotParkedVehicles()
        {
            CustomerParkingSlotID = new List<LocationLotParkedVehicles>();
        }
        public  List<LocationLotParkedVehicles> CustomerParkingSlotID { get; set; }
        public int TotalTwoWheeler { get; set; }
        public int TotalFourWheeler { get; set; }
        public int TotalOutTwoWheeler { get; set; }
        public int TotalOutFourWheeler { get; set; }
        public int TotalHVWheeler { get; set; }
        public int TotalOutHVWheeler { get; set; }
        public int TotalThreeWheeler { get; set; }
        public int TotalOutThreeWheeler { get; set; }
    }
}
