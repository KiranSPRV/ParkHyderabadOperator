using ParkHyderabadOperator.DAL.DALCheckOut;
using ParkHyderabadOperator.DAL.DALExceptionLog;
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
    public partial class CheckOutPaymentConfirmationPage : ContentPage
    {
        string PageCalledBy = string.Empty;
        CustomerParkingSlot objviolationVehicleChekOut;
        DALExceptionManagment dal_Exceptionlog;
        BlueToothDevicePrinting ObjblueToothDevicePrinting = null;
        string[] receiptlines = new string[16]; // Receipt Lines
        public CheckOutPaymentConfirmationPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            stLayoutPaymentConfirmCheckOut.IsVisible = false;

        }
        public CheckOutPaymentConfirmationPage(string redirectdFrom, CustomerParkingSlot objInput)
        {
            InitializeComponent();
            stLayoutPaymentConfirmCheckOut.IsVisible = false;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            PageCalledBy = redirectdFrom;
            objviolationVehicleChekOut = objInput;
            ImgVehicleType.Source = objInput.VehicleTypeID.VehicleIcon;
            labelChekInAmount.Text     = "("+objInput.VehicleTypeID.VehicleTypeName +" - For " + objInput.Duration + " Hours)";
            labelVehicleRegNumber.Text = objInput.CustomerVehicleID.RegistrationNumber;
            labelParkingLocation.Text  = objInput.LocationParkingLotID.LocationID.LocationName + "-" + objInput.LocationParkingLotID.LocationParkingLotName + "," + objInput.LocationParkingLotID.ParkingBayID.ParkingBayName + " " + objInput.LocationParkingLotID.ParkingBayID.ParkingBayRange;

            if (redirectdFrom == "ViolationVehicleInformation")
            {
                labelChekInAmount.Text = "(Parking ₹" + objInput.Amount.ToString("N2") + " Clamp ₹" + objInput.ClampFees.ToString("N2") + " Due Amount ₹" + objInput.DueAmount.ToString("N2") + ")";
                lableAmount.Text = (objInput.Amount + objInput.ClampFees + objInput.DueAmount).ToString("N2");
            }
            else if (redirectdFrom == "PassCheckInVehicleInformation")
            {
                labelChekInAmount.Text = "( Clamp ₹" + objInput.ClampFees.ToString("N2") + ")";
                lableAmount.Text = (objInput.ClampFees).ToString("N2");
            }
            else if (redirectdFrom == "OverstayVehicleInformation")
            {
                lableAmount.Text = (objInput.ExtendAmount + objInput.ClampFees).ToString("N2");
            }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {

                ShowLoading(true);
                if (PageCalledBy == "ViolationVehicleInformation")
                { await Navigation.PushAsync(new ViolationVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                else if (PageCalledBy == "OverstayVehicleInformation")
                { await Navigation.PushAsync(new OverstayVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                else if (PageCalledBy == "PassCheckInVehicleInformation")
                { await Navigation.PushAsync(new PassCheckInVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckOutPaymentConfirmationPage.xaml.cs", "", "BtnNo_Clicked");
                ShowLoading(false);
            }
        }
        private  void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                slYESNO.IsVisible = false;
                stLayoutPaymentConfirmCheckOut.IsVisible = true;

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckOutPaymentConfirmationPage.xaml.cs", "", "BtnYes_Clicked");
            }
        }
        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                btnCheckOut.IsVisible = false;
                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);
                    slYESNO.IsVisible = false;
                    DALVehicleCheckOut dal_CheckOut = new DALVehicleCheckOut();
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        CustomerParkingSlot VehicleCheckOut = null;
                        User objCheckoutBy = (User)App.Current.Properties["LoginUser"];
                        objviolationVehicleChekOut.CreatedBy = objCheckoutBy.UserID;
                        await Task.Run(() =>
                        {
                            VehicleCheckOut = dal_CheckOut.VehicleCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), objviolationVehicleChekOut);
                        });
                        if (VehicleCheckOut != null)
                        {
                            await Navigation.PushAsync(new CheckOutReceiptPage(PageCalledBy, VehicleCheckOut));
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Check-Out Failed,Please contact Admin.", "Ok");
                        }

                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                }
                ShowLoading(false);
                btnCheckOut.IsVisible = true;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckOutPaymentConfirmationPage.xaml.cs", "", "BtnCheckOut_Clicked");
                ShowLoading(false);
                btnCheckOut.IsVisible = true;
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutConfirmationpage.Opacity = 0.5;
            }
            else
            {
                absLayoutConfirmationpage.Opacity = 1;
            }

        }
    }
}