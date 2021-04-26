using ParkHyderabadOperator.DAL.DALCheckOut;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FOCConfirmationPage : ContentPage
    {
        CustomerParkingSlot objFOCVehicle;
        DALViolationandClamp dal_ViolationClamp;
        string RedirectPage = string.Empty;

        public FOCConfirmationPage(CustomerParkingSlot objresult, string RediretFrom)
        {
            InitializeComponent();
            stLayoutConfirmCheckOut.IsVisible = false;
            RedirectPage = RediretFrom;
            dal_ViolationClamp = new DALViolationandClamp();
            LoadGetViolationReasons();
            GetPassPaymentDetails(objresult);

        }
        private void LoadGetViolationReasons()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    pickerChckOutReason.ItemsSource = dal_ViolationClamp.GetViolationReasons(Convert.ToString(App.Current.Properties["apitoken"]), "FOC");
                }
            }
            catch (Exception ex) { }
        }
        public void GetPassPaymentDetails(CustomerParkingSlot objfocvehicle)
        {
            try
            {
                objFOCVehicle = objfocvehicle;
                ImgVehicleType.Source = objfocvehicle.CustomerVehicleID.VehicleTypeID.VehicleIcon;
                labelVehicleRegNumber.Text = objfocvehicle.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objfocvehicle.LocationParkingLotID.LocationID.LocationName + "-" + objfocvehicle.LocationParkingLotID.LocationParkingLotName + "," + objfocvehicle.LocationParkingLotID.ParkingBayID.ParkingBayName + " " + objfocvehicle.LocationParkingLotID.ParkingBayID.ParkingBayRange;

            }
            catch (Exception ex)
            {

            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutConfirmCheckOut.IsVisible = true;
            }
            catch (Exception ex)
            {

            }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stLayoutConfirmCheckOut.IsVisible = false;
                if (RedirectPage == "ViolationVehicleInformation")
                {
                    await Navigation.PushAsync(new ViolationVehicleInformation(objFOCVehicle.CustomerParkingSlotID));
                }
                else if (RedirectPage == "PassCheckInVehicleInformation")
                {
                    await Navigation.PushAsync(new PassCheckInVehicleInformation(objFOCVehicle.CustomerParkingSlotID));
                }
                else if (RedirectPage == "OverstayVehicleInformation")
                {
                    await Navigation.PushAsync(new OverstayVehicleInformation(objFOCVehicle.CustomerParkingSlotID));
                }
            }
            catch (Exception ex) { }
        }
        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            string resultmsg = string.Empty;
            MasterDetailHomePage masterPage = null;
            btnCheckOut.IsVisible = false;
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);
                    if (pickerChckOutReason.SelectedItem != null)
                    {
                        DALVehicleCheckOut dal_CheckOut = new DALVehicleCheckOut();
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                        {
                            ViolationReason objselectedreason = (ViolationReason)pickerChckOutReason.SelectedItem;
                            objFOCVehicle.StatusID.StatusName = "FOC";
                            objFOCVehicle.FOCReasonID.ViolationReasonID = objselectedreason.ViolationReasonID;
                            User objFOCBy = (User)App.Current.Properties["LoginUser"];
                            objFOCVehicle.CreatedBy = objFOCBy.UserID;
                            await Task.Run(() =>
                            {
                                resultmsg = dal_CheckOut.FOCVehicleCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), objFOCVehicle);
                                if (resultmsg != null && resultmsg == "Success")
                                {
                                    masterPage = new MasterDetailHomePage();
                                }
                            });
                            if (resultmsg != null && resultmsg == "Success")
                            {
                                await Navigation.PushAsync(masterPage);
                            }
                            else
                            {
                                await DisplayAlert("Alert", "FOC Failed,Please contact Admin.", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Lost API Token,Please login agin", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please select Reason", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                btnCheckOut.IsVisible = true;
            }
            ShowLoading(false);
            btnCheckOut.IsVisible = true;
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutFOCpage.Opacity = 0.5;
            }
            else
            {
                absLayoutFOCpage.Opacity = 1;
            }

        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}