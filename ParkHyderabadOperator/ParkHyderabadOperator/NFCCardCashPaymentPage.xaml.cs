using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
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
            objCustomerPassNewNFC = objNewNFC;
        }
        private  void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutDailyPassGeneratePassReceipt.IsVisible = true;
            }
            catch (Exception ex) { }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                slCashPaymentGeneratePass.IsVisible = false;
                var passPage = new PassPage();
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
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {

                        CustomerVehiclePass resultPass = dal_CustomerPass.SaveCustomerVehiclePassNewNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerPassNewNFC);
                        if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                        {
                            await DisplayAlert("Alert", "Customer vehicle pass created successfully", "Ok");
                            var PassPaymentReceiptPage = new PassPaymentReceiptPage(resultPass);
                            await Navigation.PushAsync(PassPaymentReceiptPage);
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Fail,Please contact admin", "Ok");
                        }


                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardCashPaymentPage.xaml.cs", "", "BtnGenerateNFCCard_Clicked");
            }
        }
        
        #region Payment Calculation
        private async void EntryCashReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entryCashReceived.Text.Length > 0 && entryCashReceived.Text != null)
                {
                    decimal cardAmount = (objCustomerPassNewNFC.CardAmount);
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
    }
}