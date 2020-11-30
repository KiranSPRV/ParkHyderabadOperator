using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NFCCardEPaymentPage : ContentPage
    {
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objCustomerPassNewNFC;
        public NFCCardEPaymentPage()
        {
            InitializeComponent();
        }
        public NFCCardEPaymentPage(CustomerVehiclePass objNewNFC)
        {
            InitializeComponent();
            stLayoutEpaymentConfirm.IsVisible = false;
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();

            try
            {
                objCustomerPassNewNFC = objNewNFC;
                if (objCustomerPassNewNFC.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("bike_black.png");
                }
                else if (objCustomerPassNewNFC.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("car_black.png");
                }
                labelVehicleRegNumber.Text = objCustomerPassNewNFC.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objCustomerPassNewNFC.CreatedBy.LocationParkingLotID.LocationID.LocationName + "-" + objCustomerPassNewNFC.PassPriceID.StationAccess;
                labelEpaymentAmount.Text = objCustomerPassNewNFC.PassPriceID.NFCCardPrice.ToString("N2");
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "MonthlyPassCashPaymentPage");
            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutEpaymentConfirm.IsVisible = true;
            }
            catch (Exception ex) { }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutEpaymentConfirm.IsVisible = false;
                var passPage = new ActivatePassPage();
                await Navigation.PushAsync(passPage);
            }
            catch (Exception ex)
            {

            }
        }
        private async void BtnGenerateNFCCard_Clicked(object sender, EventArgs e)
        {
            try
            {

                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);
                    CustomerVehiclePass resultPass = null;
                    NFCCardPaymentReceiptPagae PassPaymentReceiptPage = null;
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        await Task.Run(() =>
                        {
                             resultPass = dal_CustomerPass.SaveCustomerVehiclePassNewNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerPassNewNFC);
                            if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                            {
                                PassPaymentReceiptPage = new NFCCardPaymentReceiptPagae(resultPass);
                            }
                        });
                        if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                        {
                            await DisplayAlert("Alert", "Vehicle Pass created successfully", "Ok");
                            await Navigation.PushAsync(PassPaymentReceiptPage);
                            ShowLoading(false);
                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("Alert", "NFC Card creation failed,Please contact Admin", "Ok");
                        }


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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardEPaymentPage.xaml.cs", "", "BtnGenerateNFCCard_Clicked");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
        }
    }
}