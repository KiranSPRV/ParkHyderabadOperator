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
	public partial class NFCCardEPaymentPage : ContentPage
	{
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objCustomerPassNewNFC;
        public NFCCardEPaymentPage ()
		{
			InitializeComponent ();
		}
        public NFCCardEPaymentPage(CustomerVehiclePass objNewNFC)
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
            objCustomerPassNewNFC = objNewNFC;
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NFCCardEPaymentPage.xaml.cs", "", "BtnGenerateNFCCard_Clicked");
            }
        }
        
    }
}