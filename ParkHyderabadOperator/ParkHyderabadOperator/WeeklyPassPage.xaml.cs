using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeeklyPassPage : ContentPage
    {
        string IsNewORReNew = string.Empty;
        PassPrice objResultVMPass = null;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objCustomerPass = null;
        DALPass dal_Pass = null;
        DALCheckIn dal_DALCheckIn;
        public WeeklyPassPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            objCustomerPass = new CustomerVehiclePass();
            dal_DALCheckIn = new DALCheckIn();
        }
        public WeeklyPassPage(string NewOrReNew, PassPrice objvmPass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            objCustomerPass = new CustomerVehiclePass();
            dal_DALCheckIn = new DALCheckIn();
            IsNewORReNew = NewOrReNew;
            objResultVMPass = objvmPass;
            try
            {
                if (NewOrReNew == "New Pass")
                {
                    labelGeneratePassPageTitle.Text = "GENERATE PASS";

                }
                else if (NewOrReNew == "ReNew Pass")
                {
                    labelGeneratePassPageTitle.Text = "RENEW PASS";
                    //Get ReNew Customer Details From APP Properties
                    if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                    {
                        CustomerVehiclePass objReNewVehicle = (CustomerVehiclePass)App.Current.Properties["ReNewPassCustomerVehicle"];
                        objCustomerPass.CustomerVehiclePassID = objReNewVehicle.CustomerVehiclePassID;
                        objCustomerPass.CustomerVehicleID = objReNewVehicle.CustomerVehicleID;
                        objCustomerPass.CustomerVehicleID.CustomerID.CustomerID = objReNewVehicle.CustomerVehicleID.CustomerID.CustomerID;

                        entryRegistrationNumber.Text = objReNewVehicle.CustomerVehicleID.RegistrationNumber;
                        entryPhoneNumber.Text = objReNewVehicle.CustomerVehicleID.CustomerID.PhoneNumber;
                        entryName.Text = objReNewVehicle.CustomerVehicleID.CustomerID.Name;

                        entryRegistrationNumber.IsReadOnly = true;
                        entryPhoneNumber.IsReadOnly = true;
                        entryName.IsReadOnly = true;
                    }
                }

                labelPassAmount.Text = objResultVMPass.Price.ToString("N2");
                labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy");
                int passduration = objResultVMPass.Duration == "" || objResultVMPass.Duration == null ? 0 : (Convert.ToInt32(objResultVMPass.Duration) - 1);
                objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                labelValidTo.Text = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration).ToString("dd MMM yyyy");
                Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                if (objResultVMPass.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    imgCustomerVehcileType.Source = ImageSource.FromFile("bike_black.png");
                }
                else if (objResultVMPass.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    imgCustomerVehcileType.Source = ImageSource.FromFile("car_black.png");
                }
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    labelParkingLocation.Text = objloginuser.LocationParkingLotID.LocationID.LocationName + " Station Only";
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPage.xaml.cs", "", "WeeklyPassPage");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            int number = 0;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {
                    if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                    {
                        if (!IsVehiclehasPass())
                        {
                            string regNumber = entryRegistrationNumber.Text;
                            string regFormat = regNumber.Substring(regNumber.Length - 4);
                            if (int.TryParse(regFormat, out number))
                            {
                                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                {

                                    User objloginuser = (User)App.Current.Properties["LoginUser"];

                                    objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode = objResultVMPass.VehicleTypeID.VehicleTypeCode;
                                    objCustomerPass.CustomerVehicleID.RegistrationNumber = entryRegistrationNumber.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.Name = entryName.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.PhoneNumber = entryPhoneNumber.Text;
                                    objCustomerPass.PaymentTypeID.PaymentTypeCode = "Cash";
                                    objCustomerPass.IsMultiLot = false;
                                    objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                    objCustomerPass.Amount = objResultVMPass.Price;
                                    objCustomerPass.TotalAmount = objResultVMPass.Price;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                    objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                    objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                    int daystoexpire = (Convert.ToDateTime(objCustomerPass.ExpiryDate).Date - DateTime.Now.Date).Days;
                                    if (daystoexpire > 0)
                                    {
                                        objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate).AddDays(daystoexpire);
                                    }
                                    else
                                    {
                                        objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                    }

                                    objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                    objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    await Navigation.PushAsync(new WeeklyPassCashPaymentPage(IsNewORReNew, objCustomerPass));
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Enter valid registration number", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", entryRegistrationNumber.Text.ToUpper() + ": This vehicle already has pass.", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter registration name", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter name", "Ok");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPage.xaml.cs", "", "SlCashPayment_Tapped");
            }
        }
        private async void SlEpayment_Tapped(object sender, EventArgs e)
        {
            int number = 0;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {

                    if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                    {
                        string regNumber = entryRegistrationNumber.Text;
                        string regFormat = regNumber.Substring(regNumber.Length - 4);
                        if (int.TryParse(regFormat, out number))
                        {
                            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                            {

                                if (!IsVehiclehasPass())
                                {
                                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                                    CustomerVehiclePass objCustomerPass = new CustomerVehiclePass();
                                    objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode = objResultVMPass.VehicleTypeID.VehicleTypeCode;
                                    objCustomerPass.CustomerVehicleID.RegistrationNumber = entryRegistrationNumber.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.Name = entryName.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.PhoneNumber = entryPhoneNumber.Text;
                                    objCustomerPass.PaymentTypeID.PaymentTypeCode = "EPay";
                                    objCustomerPass.IsMultiLot = false;
                                    objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                    objCustomerPass.Amount = objResultVMPass.Price;
                                    objCustomerPass.TotalAmount = objResultVMPass.Price;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                    objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                    objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                    objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                    int daystoexpire = (Convert.ToDateTime(objCustomerPass.ExpiryDate).Date - DateTime.Now.Date).Days;
                                    if (daystoexpire > 0)
                                    {
                                        objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate).AddDays(daystoexpire);
                                    }
                                    else
                                    {
                                        objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                    }
                                    objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                    objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    var weeklyPassPaymentConfirmationPage = new WeeklyPassPaymentConfirmationPage(IsNewORReNew, objCustomerPass);
                                    await Navigation.PushAsync(weeklyPassPaymentConfirmationPage);
                                }
                                else
                                {
                                    await DisplayAlert("Alert", entryRegistrationNumber.Text.ToUpper() + ": This vehicle already has pass.", "Ok");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Enter valid registration number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter registration name", "Ok");
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "Please enter name", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "WeeklyPassPage.xaml.cs", "", "SlEpayment_Tapped");
            }
        }
        public bool IsVehiclehasPass()
        {
            bool IsVehiclehasPass = false;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    Location objLoginUserLocation = objloginuser.LocationParkingLotID.LocationID;
                    objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, objloginuser.LocationParkingLotID.LocationID.LocationID, objloginuser.LocationParkingLotID.LocationParkingLotID, objloginuser.UserID, "");
                    if (objCustomerPass.CustomerVehiclePassID != 0)
                    {
                        int daystoexpire = ((Convert.ToDateTime(objCustomerPass.ExpiryDate).Date) - (DateTime.Now.Date)).Days;
                        if (daystoexpire >= 3)
                        {
                            IsVehiclehasPass = true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "EntryRegistrationNumber_TextChanged");
            }
            return IsVehiclehasPass;
        }
    }
}