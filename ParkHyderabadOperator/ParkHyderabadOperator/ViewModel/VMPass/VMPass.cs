
using ParkHyderabadOperator.Model.Pass;

namespace ParkHyderabadOperator.ViewModel.VMPass
{
    public class VMPass
    {

        public VMPass()
        {
            PassTypeID = new PassTypes();
            PassPriceID = new PassPrice();
        }

        public PassTypes PassTypeID { get; set; }
        public PassPrice PassPriceID { get; set; }
    }
}
