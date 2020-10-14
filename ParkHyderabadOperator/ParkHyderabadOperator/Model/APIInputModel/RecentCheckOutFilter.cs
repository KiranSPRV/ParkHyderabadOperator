using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIInputModel
{
   public class RecentCheckOutFilter
    {

        public RecentCheckOutFilter()
        {

        }
        public int LocationID { get; set; }
        public int LocationParkingLotID { get; set; }
        public int UserID { get; set; }
        public int DayID { get; set; }
        public DateTime SelectedDay { get; set; }
        public bool Ins { get; set; }
        public bool Outs { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeCode { get; set; }
    }
}
