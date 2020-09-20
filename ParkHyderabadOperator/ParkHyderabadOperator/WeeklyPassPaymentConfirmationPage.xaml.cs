using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeeklyPassPaymentConfirmationPage : ContentPage
    {
        string IsNewOrReNew = string.Empty;
        CustomerVehiclePass objCustomerweeklyPass;
        DALPass dal_CustomerPass;
        DALExceptionManagment dal_Exceptionlog;
        public WeeklyPassPaymentConfirmationPage()
        {
            InitializeComponent();
            stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();

        }
        public WeeklyPassPaymentConfirmationPage(string NewOrReNew, CustomerVehiclePass objCustomerPass)
        {
            InitializeComponent();
            dal_CustomerPass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
            IsNewOrReNew = NewOrReNew;
            try
            {
                stLayoutDailyPassGeneratePassReceipt.IsVisible = false;
                objCustomerweeklyPass = objCustomerPass;
                if (objCustomerweeklyPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("bike_black.png");
                }
                else if (objCustomerweeklyPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("car_black.png");
                }

                labelVehicleRegNumber.Text = objCustomerweeklyPass.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objCustomerweeklyPass.LocationID.LocationName + "-" + "Single Station";
                labelPassAmount.Text = objCustomerweeklyPass.Amount.ToString("N2") + "/-";
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPaymentConfirmationPage.xaml.cs", "", "WeeklyPassPaymentConfirmationPage");
            }
        }
        private async void BtnGeneratePassReceipt_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {

                        CustomerVehiclePass resultPass = dal_CustomerPass.CreateCustomerPass(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerweeklyPass);
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPaymentConfirmationPage.xaml.cs", "", "BtnGeneratePassReceipt_Clicked");
            }
        }
        private void BtnYes_Clicked(object sender, EventArgs e)
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
                // await Navigation.PushAsync(new WeeklyPassPage(IsNewOrReNew));
                var passPage = new PassPage();
                await Navigation.PushAsync(passPage);
            }
            catch (Exception ex) { }
        }

        private void LoadPasseTypesAndPriceDetails(string SelectedVehicle)
        {
            try
            {
                
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    
                    DALPass dal_Pass = new DALPass();


                    VehicleLotPassPrice objVehicleLotPassPrice = new VehicleLotPassPrice();
                    objVehicleLotPassPrice.VehicleTypeCode = SelectedVehicle;
                    objVehicleLotPassPrice.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    var lstVMPass = dal_Pass.GetPassPriceDetails(Convert.ToString(App.Current.Properties["apitoken"]), objVehicleLotPassPrice);
                  

                }
               
            }
            catch (Exception ex)
            {
                
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "LoadPasseTypesAndPriceDetails");
            }
        }

    }
}