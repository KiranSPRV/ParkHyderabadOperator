using System;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class VehicleType
    {

        public int VehicleTypeID { get; set; }
        public string VehicleTypeCode { get; set; }
        public string VehicleTypeName { get; set; }
        public string VehicleTypeDesc { get; set; }
        public string VehicleTypeIcon { get; set; }
        public string VehicleImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string VehicleTypeDisplayName { get; set; }
       
        public string VehicleInActiveImage { get; set; }
        public string VehicleActiveImage { get; set; }
        public string VehicleDisplayImage { get; set; }
        public string VehicleIcon { get; set; }
    }
}
