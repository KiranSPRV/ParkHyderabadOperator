
using Newtonsoft.Json;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALLogin;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private float Latitude;
        private float Longitude;
        private bool IsOnline = false;
        DALExceptionManagment dal_Exceptionlog;
        string LoginDeviceID = string.Empty;
        string cookieUserName, cookiePassword = string.Empty;
        public LoginPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            App.Current.Properties["BaseURL"] = "http://35.202.198.25:81/InstaParkingOperatorAPI/";
        }
        protected async override void OnAppearing()
        {
            try
            {
                await GetCurrentLocation();
                cookieUserName = await SecureStorage.GetAsync("UserName");
                cookiePassword = await SecureStorage.GetAsync("Password");
                if (!string.IsNullOrEmpty(cookieUserName) && !string.IsNullOrEmpty(cookiePassword))
                {
                    entryUserID.Text = cookieUserName;
                    entryPassword.Text = cookiePassword;
                    UserLoginVerification();
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
        }
        protected async override void OnDisappearing()
        {
            if (App.Current.Properties.ContainsKey("LoginUser"))
            {
                if (App.Current.Properties["LoginUser"] == null)
                {
                    await DisplayAlert("Alert", "User details are not avilabel,Please login", "Cancel");
                }
            }

        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;

        }
        private async Task GetAPIToken()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Convert.ToString(App.Current.Properties["BaseURL"]));
                    // We want the response to be JSON.
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    List<System.Collections.Generic.KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                    postData.Add(new KeyValuePair<string, string>("username", "123"));
                    postData.Add(new KeyValuePair<string, string>("password", "123"));
                    FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                    try
                    {
                        // Post to the Server and parse the response.
                        var response = await client.PostAsync("Token", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = response.Content.ReadAsStringAsync();
                            OAuthToken responseData = JsonConvert.DeserializeObject<OAuthToken>(jsonString.Result);
                            var Access_Token = responseData.access_token;
                            if (!App.Current.Properties.ContainsKey("apitoken"))
                            {
                                App.Current.Properties["apitoken"] = Access_Token;
                            }
                            else
                            {
                                App.Current.Properties["apitoken"] = Access_Token;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Alert", "Unable to connect api.", "Ok");
                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LoginPage.xaml.cs", "", "GetAPIToken");
                        string exmsg = ex.Message;
                    }
                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to connect api.", "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LoginPage.xaml.cs", "", "GetAPIToken");

            }
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
                    await DisplayAlert("Alert", "Please enable your device location.", "Ok");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Alert", "Please enable your device location." + fnsEx.Message, "Ok");
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Alert", "Please enable your device location." + fneEx.Message, "Ok");
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Alert", "Please enable your device location." + pEx.Message, "Ok");
                // Handle permission exception
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Please enable your device location." + ex.Message, "Ok");
                // Unable to get location
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutHomepage.Opacity = 0.5;
            }
            else
            {
                absLayoutHomepage.Opacity = 1;
            }

        }
        private void BtnSignIn_Clicked(object sender, EventArgs e)
        {
            UserLoginVerification();
        }
        public async void UserLoginVerification()
        {
            User resultObj = null;
            ShowLoading(true);
            IsOnline = VerifyInternet();
            try
            {
                if ((entryUserID.Text != null && entryUserID.Text.Length >= 4) && (entryPassword.Text != null && entryPassword.Text.Length >= 8))
                {
                    if (DeviceInternet.InternetConnected())
                    {
                        await GetAPIToken();
                        await GetCurrentLocation();
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            DALUserLogin objdalLogin = new DALUserLogin();
                            UserLogin objinputuser = new UserLogin();
                            objinputuser.UserName = entryUserID.Text.Trim();
                            objinputuser.Password = entryPassword.Text.Trim();
                            objinputuser.Latitude = Latitude;
                            objinputuser.Longitude = Longitude;
                            objinputuser.LoginDeviceID = GetDeviceUniqueID();
                            APIResponse objAPIResponse;
                            objAPIResponse = objdalLogin.LoginVerification(Convert.ToString(App.Current.Properties["apitoken"]), objinputuser);
                            if (objAPIResponse != null)
                            {
                                if (objAPIResponse.Result)
                                {
                                    resultObj = JsonConvert.DeserializeObject<User>(Convert.ToString(objAPIResponse.Object));
                                    App.Current.Properties["LoginUser"] = resultObj;
                                    resultObj.LoginDeviceID = LoginDeviceID;
                                    MasterHomePage masterPage = null;
                                    await DisplayAlert("Alert", "Your location and lot details are:" + resultObj.LocationParkingLotID.LocationID.LocationName + "-" + resultObj.LocationParkingLotID.LocationParkingLotName, "Ok");
                                    DateTime toDay = DateTime.Now;
                                    TimeSpan lotClosingTime = new TimeSpan(22, 30, 0);
                                    toDay = toDay.Date + lotClosingTime;
                                    if ((resultObj.UserTypeID.UserTypeName.ToUpper()) == ("Operator".ToUpper()) )
                                    {
                                        if(DateTime.Now < toDay)
                                        {
                                            await Task.Run(() =>
                                            {
                                                if (string.IsNullOrEmpty(cookieUserName) && string.IsNullOrEmpty(cookiePassword))
                                                {
                                                    SaveUserLogin(Convert.ToString(App.Current.Properties["apitoken"]), resultObj);
                                                }
                                                SecureStorage.SetAsync("apitoken", Convert.ToString(App.Current.Properties["apitoken"]));
                                                SecureStorage.SetAsync("UserName", entryUserID.Text.Trim());
                                                SecureStorage.SetAsync("Password", entryPassword.Text.Trim());
                                                masterPage = new MasterHomePage();
                                            });
                                            await Navigation.PushAsync(masterPage);
                                        }
                                        else
                                        {
                                            await DisplayAlert("Alert", "Please contact admin,lot time ("+ toDay + ") closed", "Cancel");
                                            ShowLoading(false);
                                        }
                                    }
                                    else
                                    {
                                        await Task.Run(() =>
                                        {
                                            if (string.IsNullOrEmpty(cookieUserName) && string.IsNullOrEmpty(cookiePassword))
                                            {
                                                SaveUserLogin(Convert.ToString(App.Current.Properties["apitoken"]), resultObj);
                                            }
                                            SecureStorage.SetAsync("apitoken", Convert.ToString(App.Current.Properties["apitoken"]));
                                            SecureStorage.SetAsync("UserName", entryUserID.Text.Trim());
                                            SecureStorage.SetAsync("Password", entryPassword.Text.Trim());
                                            masterPage = new MasterHomePage();
                                        });
                                        await Navigation.PushAsync(masterPage);
                                    }
                                    
                                }
                                else
                                {
                                    
                                    await DisplayAlert("Alert", objAPIResponse.Message, "Cancel");
                                    ShowLoading(false);
                                }

                            }
                            else
                            {
                                await DisplayAlert("Alert", "Invalid Credentials", "Cancel");
                                ShowLoading(false);
                            }
                        }
                        else
                        {
                            DisplayAlert("Alert", "Unable to connect api.", "Ok");
                            ShowLoading(false);
                        }

                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please check your internet.", "Ok");
                        ShowLoading(false);
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "Please enter Valid UserID and Password.", "Ok");
                    ShowLoading(false);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LoginPage.xaml.cs", "", "BtnSignIn_Clicked");
                ShowLoading(false);
                await DisplayAlert("Alert", "Unable to connect api" + ex.Message, "Ok");
            }

        }
        public void SaveUserLogin(string accesstoken, User objLoginUser)
        {
            try
            {
                DALUserLogin objdalLogin = new DALUserLogin();
                objLoginUser.LoginTime = DateTime.Now;
                string resultmsg = objdalLogin.SaveUserDailyLogin(accesstoken, objLoginUser);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LoginPage.xaml.cs", "", "SaveUserLogin");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public string GetDeviceUniqueID()
        {
            try
            {
                LoginDeviceID = Preferences.Get("my_id", string.Empty);
                if (string.IsNullOrWhiteSpace(LoginDeviceID))
                {
                    LoginDeviceID = System.Guid.NewGuid().ToString();
                    Preferences.Set("my_id", LoginDeviceID);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LoginPage.xaml.cs", "", "GetDeviceUniqueID");
            }
            return LoginDeviceID;
        }
    }
}