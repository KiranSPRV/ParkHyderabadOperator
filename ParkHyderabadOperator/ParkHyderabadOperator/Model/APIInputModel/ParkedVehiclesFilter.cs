using ParkHyderabadOperator.Model.APIOutPutModel;
using System.Collections.Generic;

namespace ParkHyderabadOperator.Model.APIInputModel
{
    public class ParkedVehiclesFilter
    {
        public ParkedVehiclesFilter()
        {

            StatusCode = new List<Status>();
            ApplicationTypeCode = new List<ApplicationType>();
        }

        public string VehicleTypeCode { get; set; }
        public int LocationID { get; set; }
        public int LocationParkingLotID { get; set; }
        public List<Status> StatusCode { get; set; }
        public List<ApplicationType> ApplicationTypeCode { get; set; }

        public bool IsClamped { get; set; }
    }
}