using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationPage : ContentPage
    {
        VehicleCheckIn objNewCheckIn;
        DALCheckIn dal_DALCheckIn = null;
        DALExceptionManagment dal_Exceptionlog = null;
        public ConfirmationPage()
        {
            InitializeComponent();
            stLayoutConfirmCheckIn.IsVisible = false;
        }
        public ConfirmationPage(VehicleCheckIn obj)
        {
            InitializeComponent();
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            stLayoutConfirmCheckIn.IsVisible = false;
            LoadVehicleChekInDetails(obj);


        }
        public async void LoadVehicleChekInDetails(VehicleCheckIn obj)
        {
            string vehicleTypeName = string.Empty;
            try
            {
                if (obj != null)
                {
                    objNewCheckIn = obj;
                    if (obj.VehicleTypeCode == "2W")
                    {
                        vehicleTypeName = "Two Wheeler";
                        ImgVehicleType.Source = ImageSource.FromFile("bike_black.jpg");
                    }
                    else if (obj.VehicleTypeCode == "4W")
                    {
                        vehicleTypeName = "Four Wheeler";
                        ImgVehicleType.Source = ImageSource.FromFile("car_black.jpg");
                    }
                    labelVehicleRegNumber.Text = obj.RegistrationNumber;
                    labelParkingLocation.Text = obj.LocationName + "-" + obj.LocationParkingLotName + "," + obj.BayNumber + "-" + obj.BayRange;
                    labelChekInAmount.Text = Convert.ToDecimal(obj.ParkingFees + obj.ClampFees).ToString("N2");
                    labelClampAmount.Text = "( Clamp Fees " + obj.ClampFees.ToString("N2") + " )";
                    if (obj.ClampFees > 0)
                    {
                        labelChekInAmountDetails.Text = "( " + vehicleTypeName + " - For " + obj.ParkingHours + " hrs )";
                    }
                    else
                    {
                        labelChekInAmountDetails.Text = "( " + vehicleTypeName + " - For " + obj.ParkingHours + " hrs )";
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "CheckIn vehicle details are not avialable.", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "LoadVehicleChekInDetails");
            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutConfirmCheckIn.IsVisible = true;
            }
            catch (Exception ex)
            {

            }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stLayoutConfirmCheckIn.IsVisible = false;
                await Navigation.PushAsync(new CheckIn());
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnNo_Clicked");
            }
        }
        private async void BtnCheckIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                btnCheckIn.IsVisible = false;
                ShowLoading(true);
                CustomerParkingSlot objResultCustomerParkingSlot = new CustomerParkingSlot();
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken") && objNewCheckIn != null)
                {
                    if (DeviceInternet.InternetConnected())
                    {
                     await Task.Run(() =>
                     {
                         objNewCheckIn.PaymentReceived = true;
                         objResultCustomerParkingSlot = dal_DALCheckIn.SaveVehicleNewCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objNewCheckIn);

                     });
                        // Vehicle New Check In
                        if (objResultCustomerParkingSlot.CustomerParkingSlotID != 0)
                        {
                            await Navigation.PushAsync(new ReceiptPage(objResultCustomerParkingSlot));
                        }
                        else
                        {

                            await DisplayAlert("Alert", "Check-In Fail,Please contact admin.", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please check your internet.", "Cancel");
                        ShowLoading(false);
                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Check-In Fail,Please contact admin.", "Ok");
                }

                ShowLoading(false);
                btnCheckIn.IsVisible = true;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnCheckIn_Clicked");
                ShowLoading(false);
                btnCheckIn.IsVisible = true;
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutCheckInConfirmationPage.Opacity = 0.5;
            }
            else
            {
                absLayoutCheckInConfirmationPage.Opacity = 1;
            }

        }


    }
}