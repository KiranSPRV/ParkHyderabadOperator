using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
            int daystoexpire = 0;
            int passduration = 0;
            try
            {
                passduration = objResultVMPass.Duration == "" || objResultVMPass.Duration == null ? 0 : (Convert.ToInt32(objResultVMPass.Duration) - 1);
                objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
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
                        daystoexpire = (Convert.ToDateTime(objReNewVehicle.ExpiryDate).Date - DateTime.Now.Date).Days;

                        if (daystoexpire >= 0)
                        {
                            daystoexpire = (daystoexpire + 1);
                            passduration = (passduration + daystoexpire);
                            labelValidTo.Text = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration).ToString("dd MMM yyyy");

                            objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                        }
                        else
                        {
                            objResultVMPass.EndDate = Convert.ToDateTime(objResultVMPass.StartDate).AddDays(passduration);
                        }
                        GetVehiceDueAmont(objReNewVehicle.CustomerVehicleID.RegistrationNumber, objResultVMPass.VehicleTypeID.VehicleTypeCode);
                    }
                }

                labelPassType.Text = objResultVMPass.PassTypeID.PassTypeName.ToUpper();
                labelPassStationAccess.Text = objResultVMPass.StationAccess.ToUpper();

                labelPassAmount.Text = String.Format("{0:0.#}", objResultVMPass.Price);


                labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy");
                labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy");
                imgCustomerVehcileType.Source = objResultVMPass.VehicleTypeID.VehicleIcon;
                if (App.Current.Properties.ContainsKey("LoginUser"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    labelParkingLocation.Text = objloginuser.LocationParkingLotID.LocationID.LocationName + " Station Only";
                    if (!string.IsNullOrEmpty(objloginuser.LocationParkingLotID.LotCloseTime))
                    {
                        objResultVMPass.EndDate = Convert.ToDateTime((Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy")) + " " + objloginuser.LocationParkingLotID.LotCloseTime);
                    }
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
            string IsPassInOverstay = string.Empty;
            try
            {
                if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                {
                    string regNumber = entryRegistrationNumber.Text;
                    string regFormat = regNumber.Substring(regNumber.Length - 4);
                    if (int.TryParse(regFormat, out number))
                    {
                        if (entryName.Text != null && entryName.Text != "")
                        {

                            if (!string.IsNullOrEmpty(entryPhoneNumber.Text) && entryPhoneNumber.Text.Length == 10)
                            {
                                IsPassInOverstay = VerifyPassVehicleCheckInStatus(objResultVMPass.VehicleTypeID.VehicleTypeCode, entryRegistrationNumber.Text);
                                if (IsPassInOverstay == string.Empty)
                                {
                                    if (!IsVehiclehasPass())
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
                                            objCustomerPass.IsMultiLot = false;
                                            objCustomerPass.PassPriceID.PassPriceID = objResultVMPass.PassPriceID;
                                            objCustomerPass.PassPriceID.PassTypeID.PassTypeID = objResultVMPass.PassTypeID.PassTypeID;
                                            objCustomerPass.Amount = objResultVMPass.Price;
                                            objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
                                            objCustomerPass.TotalAmount = objResultVMPass.Price;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                            objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                            objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                            objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            await Navigation.PushAsync(new WeeklyPassCashPaymentPage(IsNewORReNew, objCustomerPass));
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
                                await DisplayAlert("Alert", "Please enter valid Phone Number", "Ok");
                            }

                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter Name", "Ok");

                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter Registration Number", "Ok");
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
            string IsPassInOverstay = string.Empty;
            try
            {
                if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text.Length >= 6)
                {
                    string regNumber = entryRegistrationNumber.Text;
                    string regFormat = regNumber.Substring(regNumber.Length - 4);
                    if (int.TryParse(regFormat, out number))
                    {
                        if (entryName.Text != null && entryName.Text != "")
                        {
                            if (!string.IsNullOrEmpty(entryPhoneNumber.Text) && entryPhoneNumber.Text.Length == 10)
                            {
                                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                {
                                    IsPassInOverstay = VerifyPassVehicleCheckInStatus(objResultVMPass.VehicleTypeID.VehicleTypeCode, entryRegistrationNumber.Text);
                                    if (IsPassInOverstay == string.Empty)
                                    {
                                        if (!IsVehiclehasPass())
                                        {
                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                            CustomerVehiclePass objCustomerPass = new CustomerVehiclePass();
                                            objCustomerPass.CustomerVehicleID.VehicleTypeID = objResultVMPass.VehicleTypeID;
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
                                            objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                            objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                            objCustomerPass.CreatedBy.UserID = objloginuser.UserID;
                                            objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            var weeklyPassPaymentConfirmationPage = new WeeklyPassPaymentConfirmationPage(IsNewORReNew, objCustomerPass);
                                            await Navigation.PushAsync(weeklyPassPaymentConfirmationPage);
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
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please enter valid Phone Number", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter Name", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter Registration Number", "Ok");
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
            try
            {
                var text = ((Entry)sender).Text;
                GetVehiceDueAmont(text, objResultVMPass.VehicleTypeID.VehicleTypeCode);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "DailyPass");
            }
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
                labelTotalFee.Text = String.Format("{0:0.#}", dueAmount + objResultVMPass.Price);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "DailyPass");
            }
        }



        private void entryPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entryPhoneNumber.Text))
            {
                if (entryPhoneNumber.Text.Length > 9)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(entryRegistrationNumber.Text))
                        {
                            var text = entryRegistrationNumber.Text;
                            GetVehiceDueAmont(text, objResultVMPass.VehicleTypeID.VehicleTypeCode);
                        }
                      
                    }
                    catch (Exception ex)
                    {
                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassPage.xaml.cs", "", "MonthlyPassPage");
                    }
                }

            }
        }
    }
}