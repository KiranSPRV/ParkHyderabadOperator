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
        private async void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                decimal nfcAmount = (objCustomerPassNewNFC.PassPriceID.NFCCardPrice == null || objCustomerPassNewNFC.PassPriceID.NFCCardPrice == 0) ? 0 : objCustomerPassNewNFC.PassPriceID.NFCCardPrice;
                if (entryCashReceived.Text != null && entryCashReceived.Text != "0")
                {
                    if (Convert.ToDecimal(entryCashReceived.Text) >= nfcAmount)
                    {

                        stlayoutYESNO.IsVisible = false;
                        stLayoutNFCCard.IsVisible = true;

                    }
                    else
                    {
                        ShowLoading(false);
                        await DisplayAlert("Alert", "Please enter valid Amount", "Ok");
                    }

                }
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
                    btnGenerateNFCCard.IsVisible = false;
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
                            btnGenerateNFCCard.IsVisible = true;
                        }
                        else
                        {
                            ShowLoading(false);
                            btnGenerateNFCCard.IsVisible = true;
                            await DisplayAlert("Alert", "NFC Card creation failed,Please contact Admin", "Ok");
                        }


                    }

                    else
                    {
                        ShowLoading(false);
                        btnGenerateNFCCard.IsVisible = true;
                        await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                btnGenerateNFCCard.IsVisible = true;
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