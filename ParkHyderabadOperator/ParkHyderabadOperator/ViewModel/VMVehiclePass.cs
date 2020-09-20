using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;

namespace ParkHyderabadOperator.ViewModel
{
    public class VMVehiclePass
    {
        public VMVehiclePass()
        {
            PassType = new PassTypes();
            Customer = new Customer();
            ParkingLotTypes = new ParkingLotTypes();
        }
        public Vehicle VehicleID { get; set; }
        public PassTypes PassType { get; set; }
        public Customer Customer { get; set; }
        public ParkingLotTypes ParkingLotTypes { get; set; }


    }
}
