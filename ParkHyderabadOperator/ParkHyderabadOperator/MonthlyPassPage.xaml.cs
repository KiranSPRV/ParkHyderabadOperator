using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParkHyderabadOperator.Model.APIInputModel;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonthlyPassPage : ContentPage
    {

        public string IsNewORReNew = string.Empty;
        DALExceptionManagment dal_Exceptionlog;
        PassPrice objResultVMPass = null;
        DALPass dal_Pass = null;
        CustomerVehiclePass objCustomerPass = null;
        DALCheckIn dal_DALCheckIn;
        public MonthlyPassPage(string monthlyPassStationType)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            dal_DALCheckIn = new DALCheckIn();
            slAllStationMessage.IsVisible = false;
            objCustomerPass = new CustomerVehiclePass();
        }
        public MonthlyPassPage(string NewOrReNew, PassPrice objvmPass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Pass = new DALPass();
            objCustomerPass = new CustomerVehiclePass();
            dal_DALCheckIn = new DALCheckIn();
            objResultVMPass = objvmPass;
            IsNewORReNew = NewOrReNew;
            int passduration = 0;
            passduration = objResultVMPass.Duration == "" || objResultVMPass.Duration == null ? 0 : (Convert.ToInt32(objResultVMPass.Duration) - 1);

            if (NewOrReNew == "New Pass")
            {
                labelGeneratePassPageTitle.Text = "GENERATE PASS";
                slADDNFC.IsVisible = true;
                objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
            }
            else if (NewOrReNew == "ReNew Pass")
            {
                labelGeneratePassPageTitle.Text = "RENEW PASS";
                labelInclude.Text = "(Including Card)";
                slADDNFC.IsVisible = false;
                //Get ReNew Customer Details From APP Properties
                if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                {
                    CustomerVehiclePass objReNewVehicle = (CustomerVehiclePass)App.Current.Properties["ReNewPassCustomerVehicle"];
                    objCustomerPass.CustomerVehiclePassID = objReNewVehicle.CustomerVehiclePassID;
                    objCustomerPass.CustomerVehicleID = objReNewVehicle.CustomerVehicleID;
                    objCustomerPass.CustomerVehicleID.CustomerID.CustomerID = objReNewVehicle.CustomerVehicleID.CustomerID.CustomerID;
                    objCustomerPass.IssuedCard = objReNewVehicle.IssuedCard;
                    entryRegistrationNumber.Text = objReNewVehicle.CustomerVehicleID.RegistrationNumber;
                    entryPhoneNumber.Text = objReNewVehicle.CustomerVehicleID.CustomerID.PhoneNumber;
                    entryName.Text = objReNewVehicle.CustomerVehicleID.CustomerID.Name;

                    entryRegistrationNumber.IsReadOnly = true;
                    entryPhoneNumber.IsReadOnly = true;
                    entryName.IsReadOnly = true;
                    if (objReNewVehicle.IssuedCard)
                    {
                        checkAddNFCCard.IsChecked = true;
                        labelInclude.Text = "(Including Tag)";
                        labelPassAmount.Text = (objReNewVehicle.PassPriceID.Price + objReNewVehicle.PassPriceID.NFCCardPrice).ToString("N2");

                    }
                    else
                    {
                        labelInclude.Text = "";
                        labelPassAmount.Text = (objReNewVehicle.PassPriceID.Price).ToString("N2");
                    }
                    int daystoexpire = (Convert.ToDateTime(objReNewVehicle.ExpiryDate).Date - DateTime.Now.Date).Days;
                    if (daystoexpire >= 0)
                    {
                        daystoexpire = (daystoexpire + 1);
                        passduration = (passduration + daystoexpire);
                        objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                    }
                    else
                    {
                        objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                    }
                }
            }
            FillPassDetails();

        }
        public void FillPassDetails()
        {
            User objloginuser = null;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    objloginuser = (User)App.Current.Properties["LoginUser"];

                    labelNFCCharge.Text = objResultVMPass.NFCCardPrice == null || objResultVMPass.NFCCardPrice == 0 ? "0.00" + " Extra )" : objResultVMPass.NFCCardPrice.ToString("N2") + " Extra )";
                    slAllStationMessage.IsVisible = false;
                    labelMontlyPassStationTypes.Text = objResultVMPass.StationAccess.ToUpper();

                    if (objCustomerPass.IssuedCard)
                    {
                        labelInclude.Text = "(Including Tag)";
                        labelPassAmount.Text = (objResultVMPass.Price + objResultVMPass.NFCCardPrice).ToString("N2");
                    }
                    else
                    {
                        labelPassAmount.Text = objResultVMPass.Price.ToString("N2");
                    }

                    labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy");
                    labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy");


                    //Verify Station Type
                    if (objResultVMPass.StationAccess.ToUpper() == "Single Station")
                    {
                        labelParkingLocation.Text = objloginuser.LocationParkingLotID.LocationID.LocationName;
                        labelParkingLocation.IsVisible = true;
                        slAllStationMessage.IsVisible = false;
                    }
                    else if ((objResultVMPass.StationAccess.ToUpper() == "Multi Station".ToUpper() || objResultVMPass.StationAccess.ToUpper() == "Multi Stations".ToUpper()))
                    {
                        labelParkingLocation.Text = "";
                        labelParkingLocation.IsVisible = false;
                    }
                    else if (objResultVMPass.StationAccess.ToUpper() == "All Stations" || objResultVMPass.StationAccess.ToUpper() == "All Station")
                    {
                        labelParkingLocation.Text = "";
                        labelParkingLocation.IsVisible = false;
                        slAllStationMessage.IsVisible = true;
                    }
                    // Verify Customer Vehicle Type
                    if (objResultVMPass.VehicleTypeID.VehicleTypeCode == "2W")
                    {
                        imgCustomerVehcileType.Source = ImageSource.FromFile("bike_black.png");
                    }
                    else if (objResultVMPass.VehicleTypeID.VehicleTypeCode == "4W")
                    {
                        imgCustomerVehcileType.Source = ImageSource.FromFile("car_black.png");
                    }

                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassPage.xaml.cs", "", "MonthlyPassPage");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            int number = 0;
            string IsPassInOverstay = string.Empty;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {
                    if (entryPhoneNumber.Text != null && entryPhoneNumber.Text != "")
                    {
                        if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                        {
                            IsPassInOverstay = VerifyPassVehicleCheckInStatus(objResultVMPass.VehicleTypeID.VehicleTypeCode, entryRegistrationNumber.Text);
                            if (IsPassInOverstay == string.Empty)
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
                                            if (objResultVMPass.StationAccess.ToUpper() == "Multi Station".ToUpper() || objResultVMPass.StationAccess.ToUpper() == "Multi Stations".ToUpper())
                                            {
                                                objCustomerPass.IsMultiLot = true;
                                            }
                                            else
                                            {
                                                objCustomerPass.IsMultiLot = false;
                                            }
                                            if (checkAddNFCCard.IsChecked)
                                            {
                                                objCustomerPass.IssuedCard = true;
                                                objCustomerPass.CardAmount = objResultVMPass.NFCCardPrice;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price + objResultVMPass.NFCCardPrice;
                                            }
                                            else
                                            {
                                                objCustomerPass.IssuedCard = false;
                                                objCustomerPass.CardAmount = 0;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price;
                                            }
                                            objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                            objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                            objCustomerPass.PassPriceID.PassTypeID.PassTypeName = objResultVMPass.PassTypeID.PassTypeName;
                                            objCustomerPass.Amount = objResultVMPass.Price;
                                            objCustomerPass.PassPriceID.StationAccess = objResultVMPass.StationAccess;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                            objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                            objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                            objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            await Navigation.PushAsync(new MonthlyPassCashPaymentPage(objCustomerPass));
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Alert", entryRegistrationNumber.Text.ToUpper() + ": This vehicle already has Pass.", "Ok");
                                }
                            }
                            else
                            {

                                await DisplayAlert("Alert", "Please clear due amount to Buy/Renew Pass", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter Registration Number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter Phone Number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter Name", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassPage.xaml.cs", "", "SlCashPayment_Tapped");
            }
        }
        private async void SlEpayment_Tapped(object sender, EventArgs e)
        {
            int number = 0;
            string IsPassInOverstay = string.Empty;
            try
            {
                if (entryName.Text != null && entryName.Text != "")
                {
                    if (entryPhoneNumber.Text != null && entryPhoneNumber.Text != "")
                    {
                        if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                        {
                            IsPassInOverstay = VerifyPassVehicleCheckInStatus(objResultVMPass.VehicleTypeID.VehicleTypeCode, entryRegistrationNumber.Text);
                            if (IsPassInOverstay == string.Empty)
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
                                            objCustomerPass.PaymentTypeID.PaymentTypeCode = "EPay";
                                            if (objResultVMPass.StationAccess.ToUpper() == "Multi Station")
                                            {
                                                objCustomerPass.IsMultiLot = true;
                                            }
                                            else
                                            {
                                                objCustomerPass.IsMultiLot = false;
                                            }


                                            if (checkAddNFCCard.IsChecked)
                                            {
                                                objCustomerPass.IssuedCard = true;
                                                objCustomerPass.CardAmount = objResultVMPass.NFCCardPrice;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price + objResultVMPass.NFCCardPrice;
                                            }
                                            else
                                            {
                                                objCustomerPass.IssuedCard = false;
                                                objCustomerPass.CardAmount = 0;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price;
                                            }

                                            objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                            objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                            objCustomerPass.PassPriceID.PassTypeID.PassTypeName = objResultVMPass.PassTypeID.PassTypeName;
                                            objCustomerPass.Amount = objResultVMPass.Price;
                                            objCustomerPass.PassPriceID.StationAccess = objResultVMPass.StationAccess;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                            objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                            objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                            objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            await Navigation.PushAsync(new PassGenerationEPayPaymentConfirmationPage(objCustomerPass));
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Alert", entryRegistrationNumber.Text.ToUpper() + ": This vehicle already has Pass.", "Ok");
                                }
                            }
                            else
                            {

                                await DisplayAlert("Alert", "Please clear due amount to Buy/Renew Pass", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter Registration Number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter Phone Number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter Name", "Ok");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassPage.xaml.cs", "", "SlEpayment_Tapped");
            }
        }
        private void CheckAddNFCCard_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (checkAddNFCCard.IsChecked)
                {
                    objResultVMPass.NFCApplicable = true;
                    labelInclude.Text = "(Including Tag)";
                    labelPassAmount.Text = (objResultVMPass.Price + objResultVMPass.NFCCardPrice).ToString("N2");

                }
                else
                {
                    objResultVMPass.NFCApplicable = false;
                    labelInclude.Text = "";
                    labelPassAmount.Text = (objResultVMPass.Price).ToString("N2");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassPage.xaml.cs", "", "CheckAddNFCCard_CheckedChanged");
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
                    objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, 0, 0, objloginuser.UserID, "");
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

        public string VerifyPassVehicleCheckInStatus(string vehicleTypeCode, string registrationNumber) // Verify Vehicle already parked
        {
            string alreadyCheckIn = string.Empty;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                    objPassVehicle.RegistrationNumber = registrationNumber;
                    objPassVehicle.VehicleTypeCode = vehicleTypeCode;
                    CustomerParkingSlot resultobj = dal_DALCheckIn.VerifyVehicleChekcInStatus(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                    if (resultobj.CustomerParkingSlotID != 0 && resultobj.StatusID.StatusCode == "O")
                    {
                        alreadyCheckIn = resultobj.LocationParkingLotID.LocationID.LocationName + "-" + resultobj.LocationParkingLotID.LocationParkingLotName;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReNewPassPage.xaml.cs", "", "VerifyPassVehicleCheckInStatus");
            }
            return alreadyCheckIn;
        }

    }
}