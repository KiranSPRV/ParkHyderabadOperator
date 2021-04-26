

using ParkHyderabadOperator.Model.Pass;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model.APIOutPutModel
{
   public class CustomerVehiclePass
    {
        public CustomerVehiclePass()
        {
            CustomerVehicleID = new CustomerVehicle();
            PrimaryLocationParkingLotID = new LocationParkingLot();
            PassCardTypeMapperID = new PassCardTypeMapper();
            PaymentTypeID = new PaymentType();
            CreatedBy = new User();
            PassPriceID = new PassPrice();
            PassTypeID = new PassType();
            LocationID = new Location();
            PassPurchaseLocationID = new LocationParkingLot();
            SuperVisorID = new User();
            NFCCardSoldByID = new User();
            NFCCardSoldFromID = new ApplicationType();
            NFCCardPaymentID = new PaymentType();
            NFCCardActivatedByID = new User();
            NFCSoldLotID = new LocationParkingLot();
            CardTypeID = new CardType();
        }
        public int CustomerVehiclePassID { get; set; }
        public CustomerVehicle CustomerVehicleID { get; set; }
        public LocationParkingLot PrimaryLocationParkingLotID { get; set; }
        public PassCardTypeMapper PassCardTypeMapperID { get; set; }
        public PassPrice PassPriceID { get; set; }
        public Location LocationID { get; set; }
        public LocationParkingLot PassPurchaseLocationID { get; set; }
        public CardType CardTypeID { get; set; }
        public bool IsMultiLot { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IssuedCard { get; set; }
        public string CardNumber { get; set; }
        public string BarCode { get; set; }
        public decimal Amount { get; set; }
        public decimal CardAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal DueAmount { get; set; }
        public string TransactionID { get; set; }
        public PaymentType PaymentTypeID { get; set; }
        public PassType PassTypeID { get; set; }
        public int StatusID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public User CreatedBy { get; set; }
        public User SuperVisorID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public User NFCCardSoldByID { get; set; }
        public ApplicationType NFCCardSoldFromID { get; set; }
        public PaymentType NFCCardPaymentID { get; set; }
        public DateTime NFCCardSoldDate { get; set; }
        public DateTime NFCCardActivateDate { get; set; }
        public User NFCCardActivatedByID { get; set; }
        public LocationParkingLot NFCSoldLotID { get; set; }
        public string GSTNumber { get; set; }

       

    }
}
