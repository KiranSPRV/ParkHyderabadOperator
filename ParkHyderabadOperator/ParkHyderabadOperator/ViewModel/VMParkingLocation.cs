using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;

namespace ParkHyderabadOperator.ViewModel
{
   public class VMParkingLocation
    {
        public VMParkingLocation()
        {

            Location = new Location();
            ParkingLotTypes = new ParkingLotTypes();
        }
        public Location Location { get; set; }
        public ParkingLotTypes ParkingLotTypes { get; set; }

    }
}
