﻿using ParkHyderabadOperator.Model.APIOutPutModel;
using System;

namespace ParkHyderabadOperator.ViewModel
{
    public class VMRecentCheckOuts
    {

        public VMRecentCheckOuts()
        {
            StatusID = new Status();
            Operator = new User();
            ApplicationTypeID = new ApplicationType();
            VehicleTypeID = new VehicleType();
        }
        public int CustomerParkingSlotID { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string Duration { get; set; }
        public decimal CashAmount { get; set; }
        public decimal EpayAmount { get; set; }
        public Status StatusID { get; set; }
        public ApplicationType ApplicationTypeID { get; set; }
        public VehicleType VehicleTypeID { get; set; }
        public User Operator { get; set; }
        public string VehilceStatusColor { get; set; }
       

    }
}
