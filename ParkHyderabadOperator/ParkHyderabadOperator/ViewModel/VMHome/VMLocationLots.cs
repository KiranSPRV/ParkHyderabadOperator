using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.VMHome
{
   public class VMLocationLots
    {
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public string LotName { get; set; }
        public string LocationName { get; set; }
        public int  LocationID { get; set; }
        public bool IsActive { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
    }
}
