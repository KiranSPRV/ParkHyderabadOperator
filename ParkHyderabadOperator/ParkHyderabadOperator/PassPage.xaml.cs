using ParkHyderabadOperator.Model;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassPage : ContentPage
    {
        public PassPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ShowLoading(false);
        }

        private async void BtnNewPass_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    NewPassPage newPassPage = null;
                    ShowLoading(true);
                    await Task.Run(() =>
                    {
                         newPassPage = new NewPassPage("New Pass");
                    });
                    await Navigation.PushAsync(newPassPage);
                    ShowLoading(false);
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        private async void BtnRenewPass_Clicked(object sender, EventArgs e)
        {
            try
            {
                ReNewPassPage reNewPassPage = null;
                ShowLoading(true);
                await Task.Run(() =>
                {
                     reNewPassPage = new ReNewPassPage();
                });
                await Navigation.PushAsync(reNewPassPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        private async void BtnActivatePass_Clicked(object sender, EventArgs e)
        {
            try
            {
                ActivatePassPage activatePassPage = null;
                ShowLoading(true);
                await Task.Run(() =>
                {
                    activatePassPage = new ActivatePassPage();

                });
                await Navigation.PushAsync(activatePassPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        private async void BtnValidatePass_Clicked(object sender, EventArgs e)
        {
            try
            {
                ValidatePassPage validatePassPage = null;
                ShowLoading(true);
                await Task.Run(() =>
                {
                    validatePassPage = new ValidatePassPage();
                });
                await Navigation.PushAsync(validatePassPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }

        }


        #region Navigation BackButton

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        private async void SlBackbuttonClick_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        #endregion

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            activity.IsEnabled = show;
            if (show)
            {
                absLayoutpasspage.Opacity = 0.5;
            }
            else
            {
                absLayoutpasspage.Opacity = 1;
            }

        }
    }
}