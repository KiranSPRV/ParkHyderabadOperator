using System;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class CustomerVehicle
    {
        public CustomerVehicle()
        {
            CustomerID = new Customer();
            VehicleTypeID = new VehicleType();
        }
        public int CustomerVehicleID { get; set; }
        public Customer CustomerID { get; set; }
        public VehicleType VehicleTypeID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Status StatusID { get; set; }
    }
}
