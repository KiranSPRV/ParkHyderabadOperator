using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeeklyPassCashPaymentPage : ContentPage
    {
        string IsNewOrReNew = string.Empty;
        CustomerVehiclePass objCustomerweeklyPass;
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;
        public WeeklyPassCashPaymentPage()
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
        }
        public WeeklyPassCashPaymentPage(string NewOrReNew, CustomerVehiclePass objCustomerPass)
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
            IsNewOrReNew = NewOrReNew;
            try
            {
                stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
                objCustomerweeklyPass = objCustomerPass;
                ImgVehicleType.Source = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleIcon;
                labelVehicleRegNumber.Text = objCustomerweeklyPass.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objCustomerweeklyPass.LocationID.LocationName + "-" + "Single Station";
                labelPassAmount.Text = objCustomerweeklyPass.Amount.ToString("N2") + "/-";
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassCashPaymentPage.xaml.cs", "", "WeeklyPassCashPaymentPage");
            }
        }
        private async void BtnYes_Clicked(object sender, EventArgs e)
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

        private async void BtnGeneratePassReceipt_Clicked(object sender, EventArgs e)
        {
            try
            {
                btnGeneratePassReceipt.IsVisible = false;
                CustomerVehiclePass resultPass = null;
                PassPaymentReceiptPage PassPaymentReceiptPage = null;
                ShowLoading(true);
                if (DeviceInternet.InternetConnected())
                {
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        decimal passAmount = (objCustomerweeklyPass.TotalAmount == null || objCustomerweeklyPass.TotalAmount == 0) ? objCustomerweeklyPass.Amount : objCustomerweeklyPass.TotalAmount;
                        if (Convert.ToDecimal(entryCashReceived.Text) >= passAmount)
                        {
                            await Task.Run(() =>
                            {
                                resultPass = dal_CustomerPass.CreateCustomerPass(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerweeklyPass);
                                if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                                {
                                    PassPaymentReceiptPage = new PassPaymentReceiptPage(resultPass);
                                }
                            });
                            if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                            {
                                await DisplayAlert("Alert", "Vehicle Pass created successfully", "Ok");
                                await Navigation.PushAsync(PassPaymentReceiptPage);

                            }
                            else
                            {
                                await DisplayAlert("Alert", "Payment Failed,Please contact Admin", "Ok");
                            }
                        }
                        else
                        {

                            btnGeneratePassReceipt.IsVisible = true;
                            await DisplayAlert("Alert", "Please enter valid pass amount.", "Ok");
                        }
                        ShowLoading(false);

                    }
                }
                else
                {
                    ShowLoading(false);
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPaymentConfirmationPage.xaml.cs", "", "BtnGeneratePassReceipt_Clicked");
            }
        }

        #region Payment Calculation
        private async void EntryCashReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entryCashReceived.Text.Length > 0 && entryCashReceived.Text != null)
                {
                    decimal passAmount = (objCustomerweeklyPass.TotalAmount == null || objCustomerweeklyPass.TotalAmount == 0) ? objCustomerweeklyPass.Amount : objCustomerweeklyPass.TotalAmount;
                    decimal returnAmount = Math.Abs((Convert.ToDecimal(entryCashReceived.Text) - passAmount));
                    entryCashReturn.Text = returnAmount.ToString("N2");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "EntryCashReceived_TextChanged");
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