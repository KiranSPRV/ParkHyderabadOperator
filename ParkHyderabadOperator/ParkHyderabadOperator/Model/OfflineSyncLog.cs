using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabadOperator.Model
{
    public class OfflineSyncLog
    {
        public int OfflineSyncLogID { get; set; }
        public string RegistrationNumber { get; set; }
        public string ExceptionMessage { get; set; }
        public bool IsSync { get; set; }
        public int CustomerParkingSlotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public int LocationParkingLotID { get; set; }
        public DateTime? ExpectedStartTime { get; set; }
        public DateTime? ExpectedEndTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
