using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterHomePage : MasterDetailPage
    {
        DALExceptionManagment dal_Exceptionlog;
        DALMenubar dal_Menubar = null;
        private float Latitude;
        private float Longitude;


        public MasterHomePage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Menubar = new DALMenubar();

            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser"))
                {
                    User loginUser = (User)App.Current.Properties["LoginUser"];
                    labelUserName.Text = loginUser.UserName;
                    labelUserID.Text = "ID: " + Convert.ToString(loginUser.UserCode);
                    MasterDetailHomePage homePage = new MasterDetailHomePage();
                    Detail = new NavigationPage(homePage);
                    IsPresented = false;
                }
            }
            catch (Exception ex)
            {

            }

        }
        public MasterHomePage(ParkedVehiclesFilter selectedFilters)
        {
            InitializeComponent();
            dal_Menubar = new DALMenubar();
            MasterDetailHomePage homefilterPage = new MasterDetailHomePage(selectedFilters);
            Detail = new NavigationPage(homefilterPage);
            IsPresented = false;
        }
        private async void SlHistory_Tapped(object sender, EventArgs e)
        {
            HistoryPage historyPage = null;
            try
            {

                StklauoutactivityIndicator.IsVisible = true;
                await Task.Run(() =>
                {
                    historyPage = new HistoryPage();
                });
                await Navigation.PushAsync(historyPage);
                StklauoutactivityIndicator.IsVisible = false;
            }
            catch (Exception ex) { }
        }
        private async void SlReports_Tapped(object sender, EventArgs e)
        {
            try
            {
                var repPage = new ReportPage();
                await Navigation.PushAsync(repPage);
            }
            catch (Exception ex)
            {
            }
        }
        private async void slChangePassword_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ChangePasswordPage());
            }
            catch (Exception ex)
            {
            }
        }
        private async void SlLogout_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    bool answer = await DisplayAlert("Exit", "Do you want to logout the app", "Yes", "No");
                    if (answer)
                    {
                        await GetCurrentLocation();
                        //MasterDetailHomePage masterDetailHomePage = new MasterDetailHomePage();
                        //masterDetailHomePage.StopNFCListening();

                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                        {
                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                            objloginuser.LogoutTime = DateTime.Now;
                            objloginuser.LocationParkingLotID.Lattitude = Latitude;
                            objloginuser.LocationParkingLotID.Longitude = Longitude;
                            string resultmsg = dal_Menubar.UpdateUserDailyLogOut(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser);
                            SecureStorage.RemoveAll();
                            if (App.Current.Properties.ContainsKey("apitoken"))
                            {
                                App.Current.Properties.Remove("apitoken");
                            }
                            if (App.Current.Properties.ContainsKey("LoginUser"))
                            {
                                App.Current.Properties.Remove("LoginUser");
                            }
                            if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                            {
                                App.Current.Properties.Remove("ReNewPassCustomerVehicle");
                            }

                            await Navigation.PushAsync(new LoginPage());
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "ListVehicles");
            }
        }
        private async void SlTimeSheet_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new TimeSheet());
            }
            catch (Exception ex) { }
        }
        public async Task GetCurrentLocation()
        {
            try
            {

                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Latitude = (float)location.Latitude;
                    Longitude = (float)location.Longitude;
                }
                else
                {
                    DisplayAlert("Alert", "Please enable your device location.", "Ok");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                DisplayAlert("Alert", "Please enable your device location.", "Ok");
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                DisplayAlert("Alert", "Please enable your device location.", "Ok");
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                DisplayAlert("Alert", "Please enable your device location.", "Ok");
                // Handle permission exception
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Please enable your device location.", "Ok");
                // Unable to get location
            }
        }

        private async void SlRecentCheckOuts_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RecentCheckOuts());
            }
            catch (Exception ex) { }
        }
    }
}