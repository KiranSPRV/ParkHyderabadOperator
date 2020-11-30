using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALLogin;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePasswordPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        DALUserLogin dal_Userlogin;
        User objloginuser;
        public ChangePasswordPage()
        {
            InitializeComponent();
            entryCurrentPassword.TextColor = Color.Red;
            entryNewPassword.TextColor = Color.Red;
            entryConfirmPassword.TextColor = Color.Red;

            dal_Exceptionlog = new DALExceptionManagment();
            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
            {
                objloginuser = (User)App.Current.Properties["LoginUser"];
                entryUserID.Text = objloginuser.UserCode;
            }
        }
        private async void BtnUpdatePassword_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);
                    dal_Userlogin = new DALUserLogin();
                    objloginuser.Password = entryNewPassword.Text;
                    string currentPassword = await SecureStorage.GetAsync("Password");
                    if (currentPassword != null && currentPassword != "")
                    {
                        if ((entryCurrentPassword.Text != null && entryCurrentPassword.Text != "" && entryCurrentPassword.Text.Length >= 8 && entryCurrentPassword.Text == currentPassword))
                        {
                            if ((entryNewPassword.Text != null && entryNewPassword.Text != "" && entryNewPassword.Text.Length >= 8))
                            {
                                if ((entryConfirmPassword.Text != null && entryConfirmPassword.Text != "" && entryConfirmPassword.Text.Length >= 8))
                                {
                                    if (entryNewPassword.Text == entryConfirmPassword.Text)
                                    {
                                        string msg = dal_Userlogin.UpdateUserPassword(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser);
                                        if (msg == "Success")
                                        {
                                            await SecureStorage.SetAsync("Password", entryNewPassword.Text.Trim());
                                            await DisplayAlert("Alert", "Your password updated successfully", "Ok");
                                            var masterPage = new MasterHomePage();
                                            await Navigation.PushAsync(masterPage);
                                        }
                                        else
                                        {
                                            await DisplayAlert("Alert", "Unable to update Password,Please contact Admin", "Ok");
                                            var masterPage = new MasterHomePage();
                                            await Navigation.PushAsync(masterPage);
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Password and Confirm Password do not match", "Cancel");
                                        ShowLoading(false);
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Please enter valid Confirm Password", "Cancel");
                                    ShowLoading(false);
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please enter valid New Password", "Cancel");
                                ShowLoading(false);
                            }

                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter valid Current Password", "Cancel");
                            ShowLoading(false);
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Unable to get Password details", "Cancel");
                        ShowLoading(false);
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                    ShowLoading(false);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ChangePasswordPage.xaml.cs", "", "BtnUpdatePassword_Clicked");
            }
        }


        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                abschangePasswordPagepage.Opacity = 0.5;
            }
            else
            {
                abschangePasswordPagepage.Opacity = 1;
            }

        }
    }
}