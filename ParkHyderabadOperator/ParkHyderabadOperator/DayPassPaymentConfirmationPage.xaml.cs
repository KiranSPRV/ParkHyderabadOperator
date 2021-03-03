using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.DAL.DALExceptionLog;

using ParkHyderabadOperator.Model.APIOutPutModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayPassPaymentConfirmationPage : ContentPage
    {
        string IsNewOrReNew = string.Empty;
        CustomerVehiclePass objCustomerDayPass;
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;

        public DayPassPaymentConfirmationPage()
        {
            InitializeComponent();
            stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
        }
        public DayPassPaymentConfirmationPage(string NewOrReNew)
        {
            InitializeComponent();
            IsNewOrReNew = NewOrReNew;
            stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();

        }
        public DayPassPaymentConfirmationPage(string NewOrReNew, CustomerVehiclePass objCustomerPass)
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            IsNewOrReNew = NewOrReNew;
            try
            {
                stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
                objCustomerDayPass = objCustomerPass;
                ImgVehicleType.Source =objCustomerDayPass.CustomerVehicleID.VehicleTypeID.VehicleIcon;
                labelPassType.Text = "( "+objCustomerDayPass.PassPriceID.PassTypeID.PassTypeName+" )";
                labelVehicleRegNumber.Text = objCustomerDayPass.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objCustomerDayPass.LocationID.LocationName + "-" + objCustomerDayPass.PassPriceID.StationAccess;
                labelPassAmount.Text = objCustomerDayPass.Amount.ToString("N2") + "/-";
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DayPassPaymentConfirmationPage.xaml.cs", "", "DayPassPaymentConfirmationPage");
            }
        }
        private async void BtnGeneratePassReceipt_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    CustomerVehiclePass resultPass = dal_CustomerPass.CreateCustomerPass(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerDayPass);
                    if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                    {
                        await DisplayAlert("Alert", "Vehicle Pass created successfully", "Ok");
                        await Navigation.PushAsync(new PassPaymentReceiptPage(resultPass));
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Fail to crated pass ,Please contact Admin", "Ok");
                    }

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DayPassPaymentConfirmationPage.xaml.cs", "", "BtnGeneratePassReceipt_Clicked");
            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                stLayoutDailyPassGeneratePassReceipt.IsVisible = true;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DayPassPaymentConfirmationPage.xaml.cs", "", "BtnYes_Clicked");
            }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new PassPage());
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DayPassPaymentConfirmationPage.xaml.cs", "", "BtnNo_Clicked");
            }
        }
    }
}