using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class Location
    {
        public Location()
        {
            LocationCardTypeID = new CardType();
        }
        public int LocationID { get; set; }
        public CardType LocationCardTypeID { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationDesc { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}