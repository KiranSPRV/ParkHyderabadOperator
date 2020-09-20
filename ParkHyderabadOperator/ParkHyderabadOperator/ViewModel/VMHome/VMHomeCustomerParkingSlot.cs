using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.ViewModel.VMHome
{
  public  class VMHomeCustomerParkingSlot
    {
        public VMHomeCustomerParkingSlot()
        {
            CustomerParkingSlotID = new List<CustomerParkingSlot>();
        }
        public List<CustomerParkingSlot> CustomerParkingSlotID { get; set; }
        public int TotalTwoWheeler { get; set; }
        public int TotalFourWheeler { get; set; }
    }
}
