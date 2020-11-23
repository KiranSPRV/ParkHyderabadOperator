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
    public partial class NFCCardCashPaymentPage : ContentPage
    {
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objCustomerPassNewNFC;
        public NFCCardCashPaymentPage()
        {
            InitializeComponent();
        }
        public NFCCardCashPaymentPage(CustomerVehiclePass objNewNFC)
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
            stLayoutNFCCard.IsVisible = false;
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
                labelNFCCardAmount.Text = objCustomerPassNewNFC.PassPriceID.NFCCardPrice.ToString("N2");
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardCashPaymentPage.xaml.cs", "", "NFCCardCashPaymentPage");
            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutNFCCard.IsVisible = true;
            }
            catch (Exception ex) { }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                slCashPaymentGeneratePass.IsVisible = false;
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
                    PassPaymentReceiptPage PassPaymentReceiptPage = null;
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        await Task.Run(() =>
                        {
                            resultPass = dal_CustomerPass.SaveCustomerVehiclePassNewNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerPassNewNFC);
                            if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                            {
                                PassPaymentReceiptPage = new PassPaymentReceiptPage(resultPass);
                            }
                        });
                        if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                        {
                            await DisplayAlert("Alert", "Customer vehicle pass created successfully", "Ok");
                            await Navigation.PushAsync(PassPaymentReceiptPage);
                            ShowLoading(false);
                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("Alert", "Fail,Please contact admin", "Ok");
                        }


                    }
                }
                else
                {
                    ShowLoading(false);
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardCashPaymentPage.xaml.cs", "", "BtnGenerateNFCCard_Clicked");
            }
        }

        #region Payment Calculation
        private void EntryCashReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entryCashReceived.Text.Length > 0 && entryCashReceived.Text != null)
                {
                    decimal cardAmount = (objCustomerPassNewNFC.PassPriceID.NFCCardPrice);
                    decimal returnAmount = Math.Abs((Convert.ToDecimal(entryCashReceived.Text) - cardAmount));
                    entryCashReturn.Text = returnAmount.ToString("N2");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardCashPaymentPage.xaml.cs", "", "EntryCashReceived_TextChanged");
            }
        }
        #endregion
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
        }
    }
}