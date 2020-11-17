using Firebase.Database;

using Firebase.Database.Query;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
namespace ParkHyderabadOperator.Model
{
    public class FirebaseHelper
    {
        FirebaseClient firebase = null;
        public FirebaseHelper()
        {
            firebase = new FirebaseClient("https://instaoperatorformsapp.firebaseio.com/");
        }
        public async void AddVehicleViolation(ViolationAndClamp objViolationVehicle)
        {
            try
            {
                await firebase.Child("CustomerParkingSlot")
                              .PostAsync(objViolationVehicle);
                await firebase.Child("CustomerParkingSlotHistory")
                              .PostAsync(objViolationVehicle);
            }
            catch (Exception ex)
            {
                string exmsg = ex.Message;
            }
            
        }
        public async void AddCustomerparkingSlot(CustomerParkingSlot objParkngSlots)
        {
            try
            {
                await firebase.Child("CustomerParkingSlot")
                              .PostAsync(objParkngSlots);
                await firebase.Child("CustomerParkingSlotHistory")
                              .PostAsync(objParkngSlots);
            }
            catch (Exception ex)
            {
                string exmsg = ex.Message;
            }

        }
    }
}
