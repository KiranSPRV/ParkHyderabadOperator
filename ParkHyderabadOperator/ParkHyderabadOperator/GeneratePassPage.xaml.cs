using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneratePassPage : ContentPage
    {
        string SelectedPass = string.Empty;
         
        public GeneratePassPage(string pass)
        {
            InitializeComponent();
            if (pass == "New Pass")
            {
                labelGeneratePassPageTitle.Text = "GENERATE PASS";
                slAddNFCCard.IsVisible = true;
            }
            else if (pass == "ReNew Pass")
            {

                labelGeneratePassPageTitle.Text = "RENEW PASS";
                slAddNFCCard.IsVisible =false;
            }
            LoadPassAndPersonDetails();
        }

        private void LoadPassAndPersonDetails()
        {
            try
            {

                labelValidFrom.Text = DateTime.Now.ToString("dd MMM yyyy");
                labelValidTo.Text = DateTime.Now.ToString("dd MMM yyyy");
            }
            catch(Exception ex)
            {

            }
        }

        private async void BtnCash_Clicked(object sender, EventArgs e)
        {
            try
            {

                await Navigation.PushAsync(new CashPayment());
            }
            catch (Exception ex) { }
        }

        private async void BtnElectronic_Clicked(object sender, EventArgs e)
        {
            try
            {

                //await Navigation.PushAsync(new PassGenerationEPayPaymentConfirmationPage(""));
            }
            catch (Exception ex) { }
        }
    }
}