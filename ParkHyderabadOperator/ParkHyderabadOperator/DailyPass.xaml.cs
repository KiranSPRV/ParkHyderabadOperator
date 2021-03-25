using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using ParkHyderabadOperator.DAL.DALCheckIn;
using System.Collections.Generic;
using ParkHyderabadOperator.DAL.DALMenuBar;
using System.Threading.Tasks;

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
                        GetVehiceDueAmont(objReNewVehicle.CustomerVehicleID.RegistrationNumber, objResultVMPass.VehicleTypeID.VehicleTypeCode);
                    }
                }
                labelPassType.Text = objResultVMPass.PassTypeID.PassTypeName.ToUpper();
                labelPassStationAccess.Text = objResultVMPass.StationAccess.ToUpper();

                labelPassAmount.Text = String.Format("{0:0.#}", objResultVMPass.Price);


                imgCustomerVehcileType.Source = objResultVMPass.VehicleTypeID.VehicleIcon;
                if (objResultVMPass.PassTypeID.PassTypeCode == "EP")// Event Pass
                {
                    objResultVMPass.Duration = (objResultVMPass.Duration == null || objResultVMPass.Duration == "0" || objResultVMPass.Duration == "") ? "0" : objResultVMPass.Duration;

                    // EP Start Date Validation
                    if (DateTime.Now.Date >= Convert.ToDateTime(objResultVMPass.StartDate).Date && DateTime.Now.Date <= Convert.ToDateTime(objResultVMPass.EndDate).Date)
                    {
                        objResultVMPass.StartDate = DateTime.Now.Date;
                    }
                    labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy");

                    labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy");
                }
                else if (objResultVMPass.PassTypeID.PassTypeCode == "DP")
                {
                    labelValidFrom.Text = Convert.ToDateTime(objResultVMPass.StartDate).ToString("dd MMM yyyy, hh:mm tt");
                    labelValidTo.Text = Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy, hh:mm tt");
                }
                if (App.Current.Properties.ContainsKey("LoginUser"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    labelParkingLocation.Text = objloginuser.LocationParkingLotID.LocationID.LocationName + " Station Only";
                    if (!string.IsNullOrEmpty( objloginuser.LocationParkingLotID.LotCloseTime))
                    {
                        objResultVMPass.EndDate = Convert.ToDateTime((Convert.ToDateTime(objResultVMPass.EndDate).ToString("dd MMM yyyy")) + " " + objloginuser.LocationParkingLotID.LotCloseTime);
                    }
                    
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
                if (Convert.ToDateTime(objResultVMPass.EndDate).Date >= DateTime.Now.Date)
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
                                        objCustomerPass.CustomerVehicleID.VehicleTypeID = objResultVMPass.VehicleTypeID;
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
                                        objCustomerPass.TotalAmount = objResultVMPass.Price;
                                        objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
                                        objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                        objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                        objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                        objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                        objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                        objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
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
                                await DisplayAlert("Alert", "Please enter Registration Number", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please phone number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter Name", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check pass Expiry Date", "Ok");
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
                if (Convert.ToDateTime(objResultVMPass.EndDate).Date >= DateTime.Now.Date)
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
                                        objCustomerPass.CustomerVehicleID.VehicleTypeID = objResultVMPass.VehicleTypeID;
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
                                        objCustomerPass.TotalAmount = objResultVMPass.Price;
                                        objCustomerPass.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
                                        objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                        objCustomerPass.PrimaryLocationParkingLotID.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                        objCustomerPass.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                        objCustomerPass.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                        objCustomerPass.StartDate = Convert.ToDateTime(objResultVMPass.StartDate);
                                        objCustomerPass.ExpiryDate = Convert.ToDateTime(objResultVMPass.EndDate);
                                        objCustomerPass.PassPurchaseLocationID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                        objCustomerPass.PassPurchaseLocationID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
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
                else
                {
                    await DisplayAlert("Alert", "Please check pass Expiry Date", "Ok");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "SlEpayment_Tapped");
            }


        }
        public bool IsVehiclePassVerification(string apitoken, int LocationID, string RegistrationNumber, string PassType, int PassTypeID, string VehicleTypeCode, DateTime startDate)
        {
            bool IsVehiclehasPass = false;
            try
            {
                User objloginuser = (User)App.Current.Properties["LoginUser"];
                DALCheckIn dal_DALCheckIn = new DALCheckIn();
                objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, objloginuser.LocationParkingLotID.LocationID.LocationID, objloginuser.LocationParkingLotID.LocationParkingLotID, objloginuser.UserID, "");
                if (objCustomerPass.CustomerVehiclePassID != 0)
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
    }
}