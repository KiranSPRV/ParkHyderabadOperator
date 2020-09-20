using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;

namespace ParkHyderabadOperator.ViewModel
{
   public class VMVehicleHistory
    {
        public VMVehicleHistory()
        {

            OperatorID = new Operator();
            LocationID = new Location();
            VehicleID = new Vehicle();
            CustomerID = new Customer();
        } 
        public int HistoryID { get; set; }
        public Vehicle VehicleID { get; set; }
        public Customer CustomerID { get; set; }
        public Location LocationID { get; set; }
        public decimal ParkingFees { get; set; }
        public string ParkingFrom { get; set; }
        public string ParkingTo { get; set; }
        public Operator OperatorID { get; set; }
        public string PaymentType { get; set; }

        public string ParkingTimmingsTextColor { get; set; }
    }
}
