using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParkHyderabadOperator.Model.APIInputModel;
using System.IO;
using System.Collections.Generic;
using ParkHyderabadOperator.DAL.DALMenuBar;
using System.Threading.Tasks;

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
                    lblCardType.Text = "ADD  TAG"; //+ objResultVMPass.CardTypeID.CardTypeName;
                   
                    if (objReNewVehicle.IssuedCard)
                    {
                        checkAddNFCCard.IsChecked = true;
                        labelInclude.Text = "(Including Tag)";
                        labelPassAmount.Text = String.Format("{0:0.#}", (objReNewVehicle.PassPriceID.Price + objReNewVehicle.PassPriceID.CardPrice));

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
                    GetVehiceDueAmont(objReNewVehicle.CustomerVehicleID.RegistrationNumber, objResultVMPass.VehicleTypeID.VehicleTypeCode);
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
                    labelInclude.Text = "Pass";
                    labelNFCCharge.Text = objResultVMPass.CardPrice == null || objResultVMPass.CardPrice == 0 ? "0.00" + " Extra )" : objResultVMPass.CardPrice.ToString("N2") + " Extra )";
                    slAllStationMessage.IsVisible = false;
                    labelMontlyPassStationTypes.Text = objResultVMPass.StationAccess.ToUpper();
                    labelPassType.Text = objResultVMPass.PassTypeID.PassTypeName.ToUpper();
                    lblCardType.Text = "ADD " + objResultVMPass.CardTypeID.CardTypeName;
                    if (objCustomerPass.IssuedCard)
                    {
                        labelInclude.Text = labelInclude.Text + "Pass (Including Tag)";
                        labelPassAmount.Text = String.Format("{0:0.#}", (objResultVMPass.Price + objResultVMPass.CardPrice));
                        labelTotalFee.Text = String.Format("{0:0.#}", objResultVMPass.Price + objResultVMPass.CardPrice);
                    }
                    else
                    {
                        labelPassAmount.Text = String.Format("{0:0.#}", objResultVMPass.Price);
                        labelTotalFee.Text = String.Format("{0:0.#}", objResultVMPass.Price);
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
                    imgCustomerVehcileType.Source = objResultVMPass.VehicleTypeID.VehicleIcon;
                    if (!string.IsNullOrEmpty(objloginuser.LocationParkingLotID.LotCloseTime))
                    {
                        objResultVMPass.EndDate = Convert.ToDateTime((Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy")) + " " + objloginuser.LocationParkingLotID.LotCloseTime);
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
                                            objCustomerPass.CustomerVehicleID.VehicleTypeID = objResultVMPass.VehicleTypeID;
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
                                                objCustomerPass.CardAmount = objResultVMPass.CardPrice;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price + objResultVMPass.CardPrice;
                                                objCustomerPass.CardTypeID.CardTypeID = objResultVMPass.CardTypeID.CardTypeID;
                                                objCustomerPass.CardTypeID.CardTypeName = objResultVMPass.CardTypeID.CardTypeName;
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
                                            objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
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
                                            objCustomerPass.CustomerVehicleID.VehicleTypeID = objResultVMPass.VehicleTypeID;
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
                                                objCustomerPass.CardAmount = objResultVMPass.CardPrice;
                                                objCustomerPass.TotalAmount = objResultVMPass.Price + objResultVMPass.CardPrice;
                                                objCustomerPass.CardTypeID.CardTypeID = objResultVMPass.CardTypeID.CardTypeID;
                                                objCustomerPass.CardTypeID.CardTypeName = objResultVMPass.CardTypeID.CardTypeName;
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
                                            objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
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
                    labelInclude.Text = "Pass (Including Tag)";
                    labelPassAmount.Text = String.Format("{0:0.#}", (objResultVMPass.Price + objResultVMPass.CardPrice));
                    labelTotalFee.Text = String.Format("{0:0.#}", (objResultVMPass.Price + objResultVMPass.CardPrice) + (string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text)));
                }
                else
                {
                    objResultVMPass.NFCApplicable = false;
                    labelInclude.Text = "Pass";
                    labelPassAmount.Text = String.Format("{0:0.#}", (objResultVMPass.Price));
                    labelTotalFee.Text = String.Format("{0:0.#}", (objResultVMPass.Price) + (string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text)));
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

        #region Vehicle Due Amount History ListView
        public async void LoadVehicleDueHistory()
        {
            try
            {
                ShowLoading(true);
                string vehicleType = string.Empty;
                var dal_Menubar = new DALMenubar();
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    List<CustomerParkingSlot> lstVehicleHistory = null;
                    CustomerVehicle objregistraionnumber = new CustomerVehicle();

                    imagePopVehicleImage.Source = imgCustomerVehcileType.Source;
                    labelPopVehicleDetails.Text = entryRegistrationNumber.Text;
                    await Task.Run(() =>
                    {
                        lstVehicleHistory = dal_Menubar.GetVehicleDueAmountHistory(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, objResultVMPass.VehicleTypeID.VehicleTypeCode);
                    });
                    if (lstVehicleHistory.Count > 0)
                    {
                        lvVehicleDueAmount.ItemsSource = lstVehicleHistory;

                    }
                    popupDueAmount.IsVisible = true;
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "LoadVehicleDueHistory");
            }
        }
        private void lblPopCloseGesture_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                popupDueAmount.IsVisible = false;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "lblPopCloseGesture_Tapped");
            }
        }
        private void imgDueInfo_Clicked(object sender, EventArgs e)
        {
            try
            {

                LoadVehicleDueHistory();

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "ImgClosePopUp_Clicked");
            }
        }
        private void slDueAmountGesture_Tapped(object sender, EventArgs e)
        {

            try
            {

                LoadVehicleDueHistory();

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "slDueAmountGesture_Tapped");
            }
        }
        #endregion

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;


        }

        private void entryRegistrationNumber_Completed(object sender, EventArgs e)
        {
            var text = ((Entry)sender).Text;
            GetVehiceDueAmont(text, objResultVMPass.VehicleTypeID.VehicleTypeCode);
        }
        public async void GetVehiceDueAmont(string RegistrationNumber, string VehicleTypeCode)
        {
            try
            {
                ShowLoading(true);
                var dal_Menubar = new DALMenubar();
                imagePopVehicleImage.Source = imgCustomerVehcileType.Source;
                labelPopVehicleDetails.Text = RegistrationNumber;
                decimal dueAmount = 0;
                await Task.Run(() =>
                {
                    dueAmount = dal_Menubar.GetVehicleDueAmount(Convert.ToString(App.Current.Properties["apitoken"]), RegistrationNumber, VehicleTypeCode);
                });
                labelDueAmount.Text = String.Format("{0:0.#}", dueAmount);

                if (checkAddNFCCard.IsChecked)
                {
                    labelTotalFee.Text = String.Format("{0:0.#}", (dueAmount + objResultVMPass.Price + objResultVMPass.CardPrice));
                }
                else
                {
                    labelTotalFee.Text = String.Format("{0:0.#}", (dueAmount + objResultVMPass.Price));
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "DailyPass");
            }
        }

    }
}