using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
