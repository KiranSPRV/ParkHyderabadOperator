using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DailyPass : ContentPage
    {
        public string IsNewORReNew = string.Empty;
        DALExceptionManagment dal_Exceptionlog;
        PassPrice objResultVMPass = null;
        DALPass dal_Pass = null;
        CustomerVehiclePass objCustomerPass;
        public DailyPass()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            objCustomerPass = new CustomerVehiclePass();
        }
        public DailyPass(string NewOrReNew, PassPrice objvmPass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            IsNewORReNew = NewOrReNew;
            objResultVMPass = objvmPass;
            objCustomerPass = new CustomerVehiclePass();
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
                labelPassType.Text = objResultVMPass.PassTypeID.PassTypeName;
                labelPassStationAccess.Text = objResultVMPass.StationAccess;
                labelPassAmount.Text = objResultVMPass.Price.ToString("N2");

                if (objResultVMPass.PassTypeID.PassTypeCode == "EP")// Event Pass
                {
                    objResultVMPass.Duration = (objResultVMPass.Duration == null || objResultVMPass.Duration == "0" || objResultVMPass.Duration == "") ? "0" : objResultVMPass.Duration;
                    objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(Convert.ToInt32(objResultVMPass.Duration));
                    labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy");
                    labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy");
                }
                else if (objResultVMPass.PassTypeID.PassTypeCode == "DP")
                {
                    labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy, hh:mm tt");
                    labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy, hh:mm tt");
                }

               
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "DailyPass");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            bool IsVehicleAlreayhasPass = false;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {
                    if (entryPhoneNumber.Text != null && entryPhoneNumber.Text != "")
                    {
                        if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                        {
                            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                IsVehicleAlreayhasPass = IsVehiclePassVerification(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser.LocationParkingLotID.LocationID.LocationID, entryRegistrationNumber.Text, objResultVMPass.PassTypeID.PassTypeCode, objResultVMPass.PassTypeID.PassTypeID, objResultVMPass.VehicleTypeID.VehicleTypeCode, DateTime.Now);
                                if (!IsVehicleAlreayhasPass)
                                {

                                    
                                    objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode = objResultVMPass.VehicleTypeID.VehicleTypeCode;
                                    objCustomerPass.CustomerVehicleID.RegistrationNumber = entryRegistrationNumber.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.Name = entryName.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.PhoneNumber = entryPhoneNumber.Text;
                                    objCustomerPass.PaymentTypeID.PaymentTypeCode = "Cash";
                                    objCustomerPass.IsMultiLot = false;
                                    objCustomerPass.PassPriceID.StationAccess = objResultVMPass.StationAccess;
                                    objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeName = objResultVMPass.PassTypeID.PassTypeName;
                                    objCustomerPass.Amount = objResultVMPass.Price;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                    objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                    objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                    objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                    objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                    await Navigation.PushAsync(new DayPassPaymentConfirmationPage(IsNewORReNew, objCustomerPass));
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Please check registration number,vehicle already has day pass", "Ok");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter registration number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please phone number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter name", "Ok");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "SlCashPayment_Tapped");
            }
        }
        private async void SlEpayment_Tapped(object sender, EventArgs e)
        {
            bool IsVehicleAlreayhasPass = false;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {
                    if (entryPhoneNumber.Text != null && entryPhoneNumber.Text != "")
                    {
                        if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                        {
                            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                IsVehicleAlreayhasPass = IsVehiclePassVerification(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser.LocationParkingLotID.LocationID.LocationID, entryRegistrationNumber.Text, objResultVMPass.PassTypeID.PassTypeCode, objResultVMPass.PassTypeID.PassTypeID, objResultVMPass.VehicleTypeID.VehicleTypeCode, DateTime.Now);
                                if (!IsVehicleAlreayhasPass)
                                {
                                    objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode = objResultVMPass.VehicleTypeID.VehicleTypeCode;
                                    objCustomerPass.CustomerVehicleID.RegistrationNumber = entryRegistrationNumber.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.Name = entryName.Text;
                                    objCustomerPass.CustomerVehicleID.CustomerID.PhoneNumber = entryPhoneNumber.Text;
                                    objCustomerPass.PaymentTypeID.PaymentTypeCode = "EPay";
                                    objCustomerPass.IsMultiLot = false;
                                    objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                    objCustomerPass.PassPriceID.StationAccess = objResultVMPass.StationAccess;
                                    objCustomerPass.PassPriceID.PassTypeID.PassTypeName = objResultVMPass.PassTypeID.PassTypeName;
                                    objCustomerPass.Amount = objResultVMPass.Price;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                    objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                    objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                    objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                    objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                    await Navigation.PushAsync(new DayPassPaymentConfirmationPage(IsNewORReNew, objCustomerPass));
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Please check registration number,vehicle already has day pass", "Ok");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter registration number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter phone number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter name", "Ok");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "SlEpayment_Tapped");
            }


        }
        public bool IsVehiclePassVerification(string apitoken,int LocationID, string RegistrationNumber, string PassType, int PassTypeID, string VehicleTypeCode, DateTime startDate)
        {
            bool IsVehiclehasPass = false;
            try
            {
                User objloginuser = (User)App.Current.Properties["LoginUser"];
                CustomerVehiclePass objVerifyCustomerVehicle = new CustomerVehiclePass();
                objVerifyCustomerVehicle.PassPriceID.PassTypeID.PassTypeID = PassTypeID;
                objVerifyCustomerVehicle.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                objVerifyCustomerVehicle.CustomerVehicleID.RegistrationNumber = RegistrationNumber;
                objVerifyCustomerVehicle.LocationID.LocationID = LocationID;
                objVerifyCustomerVehicle.StartDate = startDate;
                objVerifyCustomerVehicle.ExpiryDate = startDate;
                objVerifyCustomerVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode = VehicleTypeCode;
                //Result of PassDetails
                objVerifyCustomerVehicle = dal_Pass.GetCustomerVehiclePassDetails(apitoken, objVerifyCustomerVehicle);
                if (objVerifyCustomerVehicle.StartDate == startDate && objVerifyCustomerVehicle.ExpiryDate == startDate)
                {
                    IsVehiclehasPass = true;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "DailyPass");
            }
            return IsVehiclehasPass;
        }
    }
}