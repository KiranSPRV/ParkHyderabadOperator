using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;

namespace ParkHyderabadOperator.ViewModel
{
   public class VMPassPayment
    {
        public VMPassPayment()
        {
            PassType = new PassTypes();
            Customer = new Model.APIOutPutModel.Customer();
            ParkingLotTypes = new ParkingLotTypes();
            PaymentReceivedBy = new Operator();
            Location = new Location();
            VehicleID = new Vehicle();
        }
        public int PaymentID { get; set; }

        public string PaymentType { get; set; }
        public Vehicle VehicleID { get; set; }
        public PassTypes PassType { get; set; }
        public Model.APIOutPutModel.Customer Customer { get; set; }
        public ParkingLotTypes ParkingLotTypes { get; set; }
        public DateTime PaymentDate { get; set; }
        public Operator PaymentReceivedBy { get; set; }
        public Location Location { get; set; }
    }
}
