using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class LocationPassMapper
    {
        public LocationPassMapper()
        {
            LocationID = new Location();
        }
        public int LocationPassMapperID { get; set; }
        public Location LocationID { get; set; }
        public int PassID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
    }
}
