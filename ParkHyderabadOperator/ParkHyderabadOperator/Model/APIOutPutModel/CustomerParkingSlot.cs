using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
    public class CustomerParkingSlot
    {
        public CustomerParkingSlot()
        {
            ApplicationTypeID = new ApplicationType();
            CustomerID = new Customer();
            VehicleTypeID = new VehicleType();
            PaymentTypeID = new PaymentType();
            LocationParkingLotID = new LocationParkingLot();
            CustomerVehicleID = new CustomerVehicle();
            ViolationReasonID = new ViolationReason();
            FOCReasonID = new ViolationReason();
            StatusID = new Status();
            ParkingBayID = new ParkingBay();
            SuperVisorID = new User();
        }
        public int CustomerParkingSlotID { get; set; }
        public Customer CustomerID { get; set; }
        public VehicleType VehicleTypeID { get; set; }
        public PaymentType PaymentTypeID { get; set; }
        public LocationParkingLot LocationParkingLotID { get; set; }
        public CustomerVehicle CustomerVehicleID { get; set; }
        public ApplicationType ApplicationTypeID { get; set; }
        public ViolationReason ViolationReasonID { get; set; }
        public ViolationReason FOCReasonID { get; set; }
        public int ParentCustomerParkingslotID { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string Duration { get; set; }
       
        public ParkingBay ParkingBayID { get; set; }
        public byte[] VehicleParkingImage { get; set; }
        public byte[] GovernmentVehicleImage { get; set; }
        public string TransactionID { get; set; }
        public Status StatusID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UserCode { get; set; }


        public User SuperVisorID { get; set; }

        public DateTime UpdatedOn { get; set; }

        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }

        public decimal PaidDueAmount { get; set; }
        public bool IsDueAmountPaid { get; set; }
        public DateTime? DueAmountPaidOn { get; set; }

        public decimal ViolationFees { get; set; }
        public decimal ExtendAmount { get; set; }
        public decimal ClampFees { get; set; }
        public bool IsClamp { get; set; }
        public bool IsWarning { get; set; }
        public int ViolationWarningCount { get; set; }
        public decimal VehicleImageLottitude { get; set; }
        public decimal VehicleImageLongitude { get; set; }

        public string GSTNumber { get; set; }
    }
}