using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashPayment : ContentPage
    {
        string selectedPassStationType = string.Empty;
        string NewOrReNew = string.Empty;
        public CashPayment()
        {
            InitializeComponent();
            slNFCCard.IsVisible = false;
            slCashPaymentGeneratePass.IsVisible = false;
            GetPassPaymentDetails();

        }
        public CashPayment(string PassStationType)
        {
            InitializeComponent();
            slNFCCard.IsVisible = false;
            slCashPaymentGeneratePass.IsVisible = false;
            selectedPassStationType = PassStationType;
            GetPassPaymentDetails();

        }
        public CashPayment(string PassStationType,string GenerateOrReNew)
        {
            InitializeComponent();
            slNFCCard.IsVisible = false;
            slCashPaymentGeneratePass.IsVisible = false;
            selectedPassStationType = PassStationType;
            NewOrReNew = GenerateOrReNew;
            GetPassPaymentDetails();

        }
        public void GetPassPaymentDetails()
        {
            try
            {
                ImgVehicleType.Source = "bike_black.png";
                labelVehicleRegNumber.Text = "TS 05 08 FL 060";
                labelParkingLocation.Text = "KPHB Colony - Single Station";
                labelPassAmount.Text = "450.00";
                labelAmountDetails.Text = "( 400.00 Rs Pass + 50.00 Rs Card )";
            }
            catch (Exception ex)
            {

            }
        }
        private async void BtnGeneratePass_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new PassPaymentReceiptPage("Monthly Pass", selectedPassStationType));
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
                slNFCCard.IsVisible = true;
                slCashPaymentGeneratePass.IsVisible = true;
               
            }
            catch (Exception ex)
            {

            }

        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                slNFCCard.IsVisible = false;
                slCashPaymentGeneratePass.IsVisible = false;
               // await Navigation.PushAsync(new MonthlyPassPage(selectedPassStationType, NewOrReNew));
            }
            catch (Exception ex)
            {

            }
        }

        private void ImgBtnNFCCard_Clicked(object sender, EventArgs e)
        {
            try
            {
                labelNFCCard.Text = "#12345";
                labelNFCSuccessMsg.Text = "NFC card added successfully";
            }
            catch (Exception ex)
            {
            }
        }
    }
}