using ParkHyderabadOperator.DAL;
using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel;
using ParkHyderabadOperator.ViewModel.VMHome;
using ParkHyderabadOperator.ViewModel.VMPass;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckIn : ContentPage
    {
        DALCheckIn dal_DALCheckIn;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objCustomerPass = null;
        CustomerParkingSlot objextendCustomerParkingSlot = null;
        string[] LotVehicleCapability = null;
        List<ParkingBay> lstparkingbay = null;
        string SelectedVehicle = string.Empty;
        string fileName = string.Empty;
        byte[] imgCameraByteData = null;
        int defaultHours = 0;
        DateTime Lotclosetime;
        bool IsParkingFullDay = false;
        List<string> lsthoursPicker = null;
        DateTime checkInTime;
        int minimumcheckinMinutes = 120;// 2hrs
        bool IsminChckinTime = false;
        private ObservableCollection<VehicleType> obslotvehicleType = null;

        public CheckIn() // New Check-In
        {
            InitializeComponent();
            stLayoutCheckIn.IsVisible = false;
            slGovVehicleImage.IsVisible = false;
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            lstparkingbay = new List<ParkingBay>();
            LoadLocationBayNumbers();
            LoadHoursPicker();
            LoadLotVehicleAvilability();
        }

        public CheckIn(CustomerParkingSlot objextendChekIn)
        {
            InitializeComponent();
            stLayoutCheckIn.IsVisible = false;
            slGovVehicleImage.IsVisible = false;
            slGovernment.IsVisible = false;
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            lstparkingbay = new List<ParkingBay>();
            objextendCustomerParkingSlot = objextendChekIn;
            LoadLocationBayNumbers();
            LoadExtendedHoursPicker();

            defaultHours = 0;
            pickerHours.SelectedIndex = defaultHours;
            GetParkingFeeDeatils();


        } //  Extende Vehicle Check-In
        public void LoadLotVehicleAvilability()
        {
            try
            {
                if (!DeviceInternet.InternetConnected())
                {
                    slGovernment.IsVisible = false;
                }
                GetAllVehicleType();
                LoadOfflineCheckInsCount().Wait();

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadLotVehicleAvilability");
            }
        }
        public void LoadLocationBayNumbers()
        {
            try
            {
                lstparkingbay = dal_DALCheckIn.GetLocationParkingBayOffline();
                if (lstparkingbay.Count > 0)
                {
                    pickerBayNumers.ItemsSource = lstparkingbay;
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadLocationBayNumbers");
            }
        }
        public void LoadHoursPicker()
        {
            try
            {
                lsthoursPicker = dal_DALCheckIn.GetParkingHours();
                if (lsthoursPicker.Count > 0)
                {
                    pickerHours.ItemsSource = lsthoursPicker;
                    pickerHours.HeightRequest = 50;
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadParkingHours");
            }
        }
        public void LoadExtendedHoursPicker()
        {
            try
            {
                List<string> lsthoursPicker = dal_DALCheckIn.GetParkingExtendHours();
                if (lsthoursPicker.Count > 0)
                {
                    pickerHours.ItemsSource = lsthoursPicker;
                    pickerHours.SelectedIndex = 0;
                    pickerHours.HeightRequest = 50;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadParkingHours");
            }
        }

        private void LoadOfflineParkingPriceDetials(string vehicleType, int parkingHours)
        {
            try
            {
                List<VehicleParkingFee> lstparkingMinutes = dal_DALCheckIn.GetVehicleParkingFeesDetailsOffline(vehicleType, parkingHours);

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadOfflineParkingPriceDetials");
            }
        }
        private VMLocationLots LoadLoginUserDefaultLocationLots(User objLoginUser)
        {
            VMLocationLots defultVMLocationLots = new VMLocationLots();
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    var lstlots = dal_Home.GetUserAllocatedLocationAndLots(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);
                    if (lstlots.Count > 0)
                    {
                        var defaultLot = lstlots[0];
                        defultVMLocationLots = defaultLot;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "LoadLoginUserDefaultLocationLots");
            }
            return defultVMLocationLots;
        }

        private void PickerHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedVehicle != string.Empty)
            {
                if (!IsminChckinTime)
                {
                    GetParkingFeeDeatils();
                }

            }



        }
        private void PickerBayNumers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    CheckInVehicleValidation();
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "PickerBayNumers_SelectedIndexChanged");
            }
        }



        #region Parking Fee Details
        public async void GetParkingFeeDeatils()
        {
            try
            {
                List<VehicleParkingFee> lstLocationParkingLotVehiclePrice = new List<VehicleParkingFee>();
                DateTime checkInStarttime;
                decimal paidParkingFees = 0;

                if (string.IsNullOrEmpty(labelDueAmount.Text)) //Default Zero
                {
                    labelDueAmount.Text = "0";
                }

                decimal dueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);

                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    if (objextendCustomerParkingSlot == null) // If this is new Check-In 
                    {
                        checkInStarttime = DateTime.Now;
                    }
                    else
                    {
                        checkInStarttime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime);
                        paidParkingFees = Convert.ToDecimal(objextendCustomerParkingSlot.Amount);

                    }
                    if (pickerHours.SelectedItem != null)
                    {
                        DateTime openTime = DateTime.Parse(objloginuser.LocationParkingLotID.LotOpenTime);
                        Lotclosetime = DateTime.Parse(objloginuser.LocationParkingLotID.LotCloseTime);
                        lstLocationParkingLotVehiclePrice = dal_DALCheckIn.GetVehicleParkingFeesDetailsOffline(SelectedVehicle, Convert.ToInt32(pickerHours.SelectedItem.ToString()));

                        if (lstLocationParkingLotVehiclePrice.Count > 0)
                        {


                            labelSpotExpiresMessage.Text = string.Empty;
                            labelParkingFee.Text = string.Empty;
                            IsParkingFullDay = Convert.ToBoolean(lstLocationParkingLotVehiclePrice[0].IsFullDay);
                            if (checkInStarttime < Lotclosetime)
                            {
                                double checkintimeduration = Math.Round((Lotclosetime - checkInStarttime).TotalMinutes);
                                if (checkInStarttime < openTime)
                                {
                                    checkInStarttime = openTime;
                                }
                                checkInTime = checkInStarttime;
                                if (checkInStarttime.AddHours(Convert.ToInt32(lstLocationParkingLotVehiclePrice[0].Duration)) < Lotclosetime)
                                {
                                    labelSpotExpiresMessage.Text = string.Empty;
                                    labelParkingFee.Text = String.Format("{0:0.#}", lstLocationParkingLotVehiclePrice[0].Fees);
                                    labelTotalFee.Text = String.Format("{0:0.#}", lstLocationParkingLotVehiclePrice[0].Fees + dueAmount);
                                    labelSpotExpiresMessage.Text = checkInStarttime.AddHours(Convert.ToInt32(lstLocationParkingLotVehiclePrice[0].Duration)).ToString("hh:mm tt");
                                    stlayoutCheckInPayment.IsVisible = true;
                                }
                                else
                                {
                                    if (checkintimeduration < minimumcheckinMinutes)
                                    {
                                        if (lsthoursPicker.Count > 0)
                                        {
                                            string minHours = lsthoursPicker.Min();
                                            lstLocationParkingLotVehiclePrice = dal_DALCheckIn.GetVehicleParkingFeesDetailsOffline(SelectedVehicle, Convert.ToInt32(minHours));
                                            if (lstLocationParkingLotVehiclePrice.Count > 0)
                                            {
                                                IsminChckinTime = true;
                                                pickerHours.SelectedItem = lsthoursPicker.Min();
                                                labelSpotExpiresMessage.Text = string.Empty;
                                                labelParkingFee.Text = String.Format("{0:0.#}", lstLocationParkingLotVehiclePrice[0].Fees);
                                                labelTotalFee.Text = String.Format("{0:0.#}", lstLocationParkingLotVehiclePrice[0].Fees + dueAmount);
                                                labelSpotExpiresMessage.Text = Lotclosetime.ToString("hh:mm tt");
                                                stlayoutCheckInPayment.IsVisible = true;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        stlayoutCheckInPayment.IsVisible = false;
                                        return;
                                    }

                                }
                            }
                            else
                            {
                                stlayoutCheckInPayment.IsVisible = false;
                                pickerBayNumers.IsEnabled = false;
                                await DisplayAlert("Alert", "Please check Lot timings from " + openTime + " to " + Lotclosetime, "Ok");
                                return;
                            }
                            if (labelParkingFee.Text == string.Empty || labelParkingFee.Text == null)
                            {
                                stlayoutCheckInPayment.IsVisible = false;
                                await DisplayAlert("Alert", "Please contact Admin parking fees details are not available for this hours.", "Ok");

                                return;
                            }
                        }
                        else
                        {
                            labelSpotExpiresMessage.Text = string.Empty;
                            labelParkingFee.Text = string.Empty;
                            stlayoutCheckInPayment.IsVisible = false;
                            await DisplayAlert("Alert", "Parking details not found", "Ok");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "GetParkingFeeDeatils");
            }
        }
        #endregion

        #region Registraion Number Pass Checking
        private async void EntryRegistrationNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedVehicle != string.Empty)
            {
                pickerBayNumers.SelectedIndex = -1;
            }
            else
            {
                await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
            }
        }
        #endregion

        #region Check-In Payment 
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            int number;
            string existingCheckIn = string.Empty;
            try
            {
                ShowLoading(true);
                if (SelectedVehicle != string.Empty)
                {
                    if (entryRegistrationNumber.Text != null && (entryRegistrationNumber.Text.Length >= 6 && entryRegistrationNumber.Text.Length <= 10))
                    {
                        string regNumber = entryRegistrationNumber.Text;
                        string regFormat = regNumber.Substring(regNumber.Length - 4);
                        if (int.TryParse(regFormat, out number))
                        {
                            if (pickerBayNumers.SelectedItem != null)
                            {
                                ParkingBay objselectedbay = (ParkingBay)pickerBayNumers.SelectedItem;
                                if (pickerHours.SelectedItem != null)
                                {
                                    int selectedhours = Convert.ToInt32(pickerHours.SelectedItem.ToString());
                                    var selVehicleType = (VehicleType)collstviewVehicleTye.SelectedItem;
                                    if (labelParkingFee.Text != "" && labelParkingFee.Text != string.Empty)
                                    {

                                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                        {
                                            // Vehicle Check-In
                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                            VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                            objPassVehicle.UserID = objloginuser.UserID;
                                            objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                            objPassVehicle.VehicleTypeName = selVehicleType.VehicleTypeName;
                                            objPassVehicle.VehicleTypeDisplayName = selVehicleType.VehicleTypeDisplayName;
                                            objPassVehicle.VehicleImage = selVehicleType.VehicleIcon;
                                            objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                            objPassVehicle.BayNumber = objselectedbay.ParkingBayName;
                                            objPassVehicle.BayRange = objselectedbay.ParkingBayRange;
                                            objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                            objPassVehicle.PhoneNumber = entryPhoneNumber.Text;
                                            objPassVehicle.ParkingHours = selectedhours;
                                            objPassVehicle.ParkingFees = Convert.ToDecimal(labelParkingFee.Text);
                                            objPassVehicle.DueAmount =string.IsNullOrEmpty(labelDueAmount.Text)?0: Convert.ToDecimal(labelDueAmount.Text);
                                            
                                            objPassVehicle.PaymentType = "Cash";
                                            if (objloginuser.LocationParkingLotID.LocationParkingLotID == 0)
                                            {
                                                if (DeviceInternet.InternetConnected())
                                                {
                                                    var rstLot = LoadLoginUserDefaultLocationLots(objloginuser);
                                                    objPassVehicle.LocationParkingLotID = rstLot.LocationParkingLotID;
                                                    objPassVehicle.LocationParkingLotName = rstLot.LotName;
                                                    objPassVehicle.LocationID = rstLot.LocationID;
                                                    objPassVehicle.LocationName = rstLot.LocationName;
                                                }
                                            }
                                            else
                                            {
                                                objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                                objPassVehicle.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                                objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                                objPassVehicle.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            }
                                            if (objextendCustomerParkingSlot == null)
                                            {
                                                existingCheckIn = VerifyVehicleCheckInStatus();
                                                if (existingCheckIn == string.Empty)
                                                {
                                                    objPassVehicle.ParkingStartTime = checkInTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    if (IsParkingFullDay)
                                                    {

                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }
                                                    else
                                                    {
                                                        if (!IsminChckinTime)
                                                        {
                                                            objPassVehicle.ParkingEndTime = checkInTime.AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                        else
                                                        {
                                                            objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }

                                                    var confirmationPage = new ConfirmationPage(objPassVehicle);
                                                    await Navigation.PushAsync(confirmationPage);
                                                }
                                                else
                                                {
                                                    await DisplayAlert("Alert", "Vehicle Checked In at " + existingCheckIn, "Ok");
                                                }
                                            }
                                            else
                                            {
                                                objPassVehicle.CustomerParkingSlotID = objextendCustomerParkingSlot.CustomerParkingSlotID;
                                                objPassVehicle.ParkingStartTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).ToString("MM/dd/yyyy hh:mm:ss tt");
                                                objPassVehicle.ClampFees = Convert.ToDecimal(objextendCustomerParkingSlot.ClampFees);
                                                if (IsParkingFullDay)
                                                {
                                                    TimeSpan extfullday = (Lotclosetime - Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime));
                                                    int lotHours = Convert.ToInt32(extfullday.TotalHours);
                                                    objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");

                                                }
                                                else
                                                {
                                                    if (!IsminChckinTime)
                                                    {
                                                        objPassVehicle.ParkingEndTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }
                                                    else
                                                    {
                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }

                                                }
                                                var confPage = new ConfirmationPage(objPassVehicle);
                                                await Navigation.PushAsync(confPage);
                                            }
                                        }
                                        else
                                        {

                                            await DisplayAlert("Alert", "Token details  unavailable", "Ok");
                                        }
                                    }
                                    else
                                    {

                                        await DisplayAlert("Alert", "Please select Parking hours.", "Ok");
                                    }
                                }
                                else
                                {

                                    await DisplayAlert("Alert", "Please select Parking hours.", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please select Bay Number.", "Ok");
                            }

                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter valid Registration Number.", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "BtnCheckIn_Clicked");
            }

        }
        private async void SlEpayment_Tapped(object sender, EventArgs e)
        {
            int number;
            string existingCheckIn = string.Empty;
            try
            {
                ShowLoading(true);
                if (SelectedVehicle != string.Empty)
                {
                    if (entryRegistrationNumber.Text != null && (entryRegistrationNumber.Text.Length >= 6 && entryRegistrationNumber.Text.Length <= 10))
                    {
                        string regNumber = entryRegistrationNumber.Text;
                        string regFormat = regNumber.Substring(regNumber.Length - 4);
                        if (int.TryParse(regFormat, out number))
                        {

                            if (pickerBayNumers.SelectedItem != null)
                            {
                                if (pickerHours.SelectedItem != null)
                                {
                                    ParkingBay objselectedbay = (ParkingBay)pickerBayNumers.SelectedItem;
                                    int selectedhours = Convert.ToInt32(pickerHours.SelectedItem.ToString());
                                    var selVehicleType = (VehicleType)collstviewVehicleTye.SelectedItem;


                                    if (labelParkingFee.Text != "" && labelParkingFee.Text != string.Empty)
                                    {
                                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                        {
                                            // Vehicle Check-In
                                            VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                            objPassVehicle.UserID = objloginuser.UserID;
                                            if (objloginuser.LocationParkingLotID.LocationParkingLotID == 0)
                                            {
                                                var rstLot = LoadLoginUserDefaultLocationLots(objloginuser);
                                                objPassVehicle.LocationParkingLotID = rstLot.LocationParkingLotID;
                                                objPassVehicle.LocationParkingLotName = rstLot.LocationParkingLotName;
                                                objPassVehicle.LocationID = rstLot.LocationID;
                                                objPassVehicle.LocationName = rstLot.LocationName;
                                            }
                                            else
                                            {
                                                objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                                objPassVehicle.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                                objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                                objPassVehicle.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            }

                                            objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                            objPassVehicle.VehicleTypeName = selVehicleType.VehicleTypeName;
                                            objPassVehicle.VehicleTypeDisplayName = selVehicleType.VehicleTypeDisplayName;
                                            objPassVehicle.VehicleImage = selVehicleType.VehicleIcon;
                                            objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                            objPassVehicle.BayNumber = objselectedbay.ParkingBayName;
                                            objPassVehicle.BayRange = objselectedbay.ParkingBayRange;
                                            objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                            objPassVehicle.PhoneNumber = entryPhoneNumber.Text;
                                            objPassVehicle.ParkingHours = selectedhours;
                                            objPassVehicle.ParkingFees = Convert.ToDecimal(labelParkingFee.Text);
                                            objPassVehicle.DueAmount = string.IsNullOrEmpty(labelDueAmount.Text) ? 0 : Convert.ToDecimal(labelDueAmount.Text);
                                            objPassVehicle.PaymentType = "EPay";

                                            if (objextendCustomerParkingSlot == null)
                                            {
                                                existingCheckIn = VerifyVehicleCheckInStatus();
                                                if (existingCheckIn == string.Empty)
                                                {
                                                    objPassVehicle.ParkingStartTime = checkInTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    if (IsParkingFullDay)
                                                    {

                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");

                                                    }
                                                    else
                                                    {
                                                        if (!IsminChckinTime)
                                                        {
                                                            objPassVehicle.ParkingEndTime = checkInTime.AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                        else
                                                        {
                                                            objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }

                                                    }

                                                    var conPage = new ConfirmationPage(objPassVehicle);
                                                    await Navigation.PushAsync(conPage);
                                                }
                                                else
                                                {
                                                    await DisplayAlert("Alert", "Vehicle Checked In at  " + existingCheckIn, "Ok");
                                                }
                                            }
                                            else
                                            {
                                                objPassVehicle.CustomerParkingSlotID = objextendCustomerParkingSlot.CustomerParkingSlotID;
                                                objPassVehicle.ParkingStartTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).ToString("MM/dd/yyyy hh:mm:ss tt");

                                                if (IsParkingFullDay)
                                                {

                                                    objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");

                                                }
                                                else
                                                {
                                                    if (!IsminChckinTime)
                                                    {
                                                        objPassVehicle.ParkingEndTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }
                                                    else
                                                    {
                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }

                                                }

                                                var newCheckInConPage = new ConfirmationPage(objPassVehicle);
                                                await Navigation.PushAsync(newCheckInConPage);
                                            }
                                        }
                                        else
                                        {
                                            await DisplayAlert("Alert", "Token details  unavailable", "Ok");
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Please select Parking hours.", "Ok");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Please select Parking hours.", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please select Bay Number.", "Ok");
                            }

                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please enter valid Registration Number.", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "BtnCheckIn_Clicked");
            }
        }
        #endregion

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;

            if (show)
            {
                absLayoutCheckInpage.Opacity = 0.5;
            }
            else
            {
                absLayoutCheckInpage.Opacity = 1;
            }

        }
        private async void ChkGovernment_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    if (chkGovernment.IsChecked)
                    {
                        checkInTime = DateTime.Now;
                        stLayoutCheckIn.IsVisible = true;
                        slGovVehicleImage.IsVisible = true;
                        stlayoutCheckInPayment.IsVisible = false;
                        slParkinghours.IsVisible = false;
                        lblPhoneNumber.Text = "Phone Number";
                    }
                    else
                    {
                        stLayoutCheckIn.IsVisible = false;
                        slGovVehicleImage.IsVisible = false;
                        stlayoutCheckInPayment.IsVisible = true;
                        slParkinghours.IsVisible = true;
                        lblPhoneNumber.Text = "Phone Number (Optional)";
                    }
                }
                else
                {
                    chkGovernment.IsChecked = false;
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");

                }
            }
            catch (Exception ex)
            {

                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "ChkGovernment_CheckedChanged");
            }
        }

        #region Check-In (Goverment/Pass )
        private async void BtnCheckIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                int number;
                string msg = string.Empty;
                string existingCheckIn = string.Empty;
                MasterHomePage masterpage = null;
                btnCheckIn.IsVisible = false;
                ShowLoading(true);
                if (SelectedVehicle != string.Empty)
                {

                    if (checkInTime < Lotclosetime)
                    {
                        if (entryRegistrationNumber.Text != null && (entryRegistrationNumber.Text.Length >= 6 && entryRegistrationNumber.Text.Length <= 10))
                        {
                            string regNumber = entryRegistrationNumber.Text;
                            string regFormat = regNumber.Substring(regNumber.Length - 4);
                            if (int.TryParse(regFormat, out number))
                            {
                                if (pickerBayNumers.SelectedItem != null)
                                {
                                    ParkingBay objselectedbay = (ParkingBay)pickerBayNumers.SelectedItem;
                                    if (objselectedbay != null)
                                    {
                                        existingCheckIn = VerifyVehicleCheckInStatus();
                                        if (existingCheckIn == string.Empty)
                                        {
                                            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                            {
                                                VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                                objPassVehicle.UserID = objloginuser.UserID;
                                                objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                                objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                                objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                                objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                                objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                                objPassVehicle.PhoneNumber = entryPhoneNumber.Text;

                                                GeocodingDetails objGeocodingDetails = new GeocodingDetails();
                                                Xamarin.Essentials.Location resultloc = await objGeocodingDetails.GetCurrentLocation();

                                                if (chkGovernment.IsChecked)
                                                {
                                                    // Government vehicle Check In
                                                    if (entryPhoneNumber.Text != null && entryPhoneNumber.Text != "")
                                                    {
                                                        if (Convert.ToString(entryPhoneNumber.Text).Length >= 10)
                                                        {
                                                            if (imgCameraByteData != null && imgCameraByteData.Length > 0)
                                                            {
                                                                objPassVehicle.StatusName = "Government";
                                                                objPassVehicle.GovernmentVehicleImage = ByteArrayCompressionUtility.Compress(imgCameraByteData);
                                                                if (resultloc != null)
                                                                {
                                                                    objPassVehicle.VehicleImageLottitude = Convert.ToDecimal(resultloc.Latitude);
                                                                    objPassVehicle.VehicleImageLongitude = Convert.ToDecimal(resultloc.Longitude);
                                                                }

                                                                await Task.Run(() =>
                                                                  {
                                                                      msg = dal_DALCheckIn.SaveGovernmentVehicleCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                                                                      if (msg == "Success")
                                                                      {
                                                                          masterpage = new MasterHomePage();
                                                                      }
                                                                  });
                                                                if (msg == "Success")
                                                                {
                                                                    await Navigation.PushAsync(masterpage);
                                                                }
                                                                else
                                                                {
                                                                    await DisplayAlert("Alert", "Unable to Check In, Please contact Admin", "Ok");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                await DisplayAlert("Alert", "Please select Vehicle Image.", "Ok");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            await DisplayAlert("Alert", "Please enter valid Phone Number.", "Ok");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await DisplayAlert("Alert", "Please enter Phone Number.", "Ok");
                                                    }
                                                }
                                                else if (objCustomerPass != null)
                                                {
                                                    // Pass holder vehicle Check In
                                                    objPassVehicle.StatusName = "Pass";
                                                    objPassVehicle.CustomerID = objCustomerPass.CustomerVehicleID.CustomerID.CustomerID;
                                                    await Task.Run(() =>
                                                    {
                                                        msg = dal_DALCheckIn.SavePassVehicleCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                                                        if (msg == "Success")
                                                        {
                                                            masterpage = new MasterHomePage();

                                                        }
                                                    });
                                                    if (msg == "Success")
                                                    {
                                                        await Navigation.PushAsync(masterpage);
                                                    }
                                                    else
                                                    {
                                                        await DisplayAlert("Alert", "Unable to Check In, Please contact Admin", "Ok");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            await DisplayAlert("Alert", "Vehicle Checked In at " + existingCheckIn, "Ok");
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Please select Bay Number", "Ok");
                                    }

                                }
                                else
                                {

                                    await DisplayAlert("Alert", "Please select Bay Number", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter Registration Number minimum 6 digits", "Ok");
                        }
                    }
                    else
                    {
                        ShowLoading(false);
                        btnCheckIn.IsVisible = false;
                        await DisplayAlert("Alert", "Please check Lot closing time " + Lotclosetime.ToString("HH:mm tt"), "Ok");
                        return;
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
                }
                btnCheckIn.IsVisible = true;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                btnCheckIn.IsVisible = true;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "BtnCheckIn_Clicked");
            }
        }
        #endregion
        private async void BtnCamera_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }
                else
                {

                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {

                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                        Directory = "Violation",
                        SaveToAlbum = false,
                        CustomPhotoSize = 20,//Resize to 90% of original
                        CompressionQuality = 92


                    });
                    if (file != null)
                    {
                        if (file.Path.Contains("/"))
                        {
                            try
                            {
                                string[] getfilename = file.Path.Split('/');
                                fileName = getfilename[getfilename.Length - 1];
                                var memoryStream = new MemoryStream();
                                file.GetStream().CopyTo(memoryStream);
                                imgCameraByteData = memoryStream.ToArray();
                                btnCamera.Source = file.Path;
                            }
                            catch (Exception ex)
                            {

                                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "Violation.xaml.cs", "", "BtnCamera_Clicked");
                                await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("No Camera", "Unable to find file path:", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("No Camera", "Unable to open:", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "BtnCamera_Clicked");
                await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
            }
        }
        private async void BtnGallery_Clicked(object sender, EventArgs e)
        {
            try
            {

                try
                {
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                        return;
                    }
                    MediaFile file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Custom,
                        CustomPhotoSize = 20,//Resize to 90% of original
                        CompressionQuality = 92

                    });


                    if (file == null)
                        return;
                    string fileName = string.Empty;

                    if (file.Path.Contains('/'))
                    {
                        try
                        {

                            string[] getfilename = file.Path.Split('/');
                            fileName = getfilename[getfilename.Length - 1];
                            var memoryStream = new MemoryStream();
                            file.GetStream().CopyTo(memoryStream);
                            imgCameraByteData = memoryStream.ToArray();
                            BtnGallery.Source = file.Path;
                        }
                        catch (Exception ex)
                        {

                        }


                    }

                }
                catch (Exception ex) { }

            }
            catch (Exception ex)
            {

                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "BtnGallery_Clicked");
                await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
            }

        }
        public string VerifyVehicleCheckInStatus() // Verify Vehicle Check-In
        {
            string alreadyCheckIn = string.Empty;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                    objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    objPassVehicle.VehicleTypeCode = SelectedVehicle;
                    objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                    if (DeviceInternet.InternetConnected())
                    {
                        CustomerParkingSlot resultobj = dal_DALCheckIn.VerifyVehicleChekcInStatus(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                        if (resultobj.CustomerParkingSlotID != 0)
                        {
                            alreadyCheckIn = resultobj.LocationParkingLotID.LocationID.LocationName + "-" + resultobj.LocationParkingLotID.LocationParkingLotName;
                        }
                    }
                    else
                    {
                        var existingVehicle = App.SQLiteDb.GetIVehicleDetailsAsync(entryRegistrationNumber.Text);
                        if (existingVehicle != null)
                        {
                            alreadyCheckIn = objloginuser.LocationParkingLotID.LocationID.LocationName + "-" + objloginuser.LocationParkingLotID.LocationParkingLotName;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "VerifyVehicleCheckInStatus");
            }
            return alreadyCheckIn;
        }
        public async void CheckInVehicleValidation()
        {
            try
            {
                ShowLoading(true);
                if (!chkGovernment.IsChecked && objextendCustomerParkingSlot == null)
                {
                    if (entryRegistrationNumber != null)
                    {
                        if (entryRegistrationNumber.Text.Length >= 6)
                        {
                            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                Model.APIOutPutModel.Location objLoginUserLocation = objloginuser.LocationParkingLotID.LocationID;
                                VMVehiclePassWithDueAmount objVMVehiclePassWithDueAmount = new VMVehiclePassWithDueAmount();
                                await Task.Run(() =>
                                {
                                    objVMVehiclePassWithDueAmount = dal_DALCheckIn.GetVerifyVehicleHasPassWithDueAmount(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, SelectedVehicle, objloginuser.LocationParkingLotID.LocationID.LocationID, objloginuser.LocationParkingLotID.LocationParkingLotID, objloginuser.UserID, "");

                                });
                                objCustomerPass = objVMVehiclePassWithDueAmount.CustomerVehiclePassID;

                                labelDueAmount.Text = String.Format("{0:0.#}", objVMVehiclePassWithDueAmount.VehicleDueAmount);
                                labelTotalFee.Text = String.Format("{0:0.#}", (string.IsNullOrEmpty(labelParkingFee.Text) ? 0 : Convert.ToDecimal(labelParkingFee.Text) + objVMVehiclePassWithDueAmount.VehicleDueAmount));
                                if ((objCustomerPass.CustomerVehiclePassID != 0) && ((Convert.ToDateTime(objCustomerPass.StartDate).Date <= DateTime.Now.Date)&& (Convert.ToDateTime(objCustomerPass.ExpiryDate).Date >= DateTime.Now.Date)))
                                {
                                    if (SelectedVehicle == objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper())
                                    {
                                        if ((objCustomerPass.PassPriceID.StationAccess == "All Stations" || objCustomerPass.PassPriceID.StationAccess == "All Station"))
                                        {

                                            stLayoutCheckIn.IsVisible = true;
                                            stlayoutCheckInPayment.IsVisible = false;
                                            slParkinghours.IsVisible = false;
                                            SelectedVehicle = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper();
                                            GetSelectedVehicleType(SelectedVehicle);

                                            await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                        }
                                        else if (objCustomerPass.IsMultiLot)
                                        {
                                            DALHome dal_Home = new DALHome();
                                            List<VMMultiLocations> passLocations = dal_Home.GetAllPassLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), SelectedVehicle, objCustomerPass.CustomerVehiclePassID);
                                            var isvalid = passLocations.Where(p => p.LocationID == objloginuser.LocationParkingLotID.LocationID.LocationID);
                                            if (isvalid != null)
                                            {
                                                stLayoutCheckIn.IsVisible = true;
                                                stlayoutCheckInPayment.IsVisible = false;
                                                slParkinghours.IsVisible = false;
                                                SelectedVehicle = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper();
                                                GetSelectedVehicleType(SelectedVehicle);
                                                await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                            }
                                        }
                                        else if (objCustomerPass.LocationID.LocationID == objloginuser.LocationParkingLotID.LocationID.LocationID)
                                        {
                                            stLayoutCheckIn.IsVisible = true;
                                            stlayoutCheckInPayment.IsVisible = false;
                                            slParkinghours.IsVisible = false;
                                            SelectedVehicle = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper();
                                            GetSelectedVehicleType(SelectedVehicle);
                                            await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                        }
                                        else
                                        {
                                            stLayoutCheckIn.IsVisible = false;
                                            stlayoutCheckInPayment.IsVisible = true;
                                            slParkinghours.IsVisible = true;

                                        }
                                        slGovernment.IsVisible = false;
                                    }
                                    else
                                    {
                                        string passvehicletype = string.Empty;
                                        stLayoutCheckIn.IsVisible = false;
                                        stlayoutCheckInPayment.IsVisible = false;
                                        slParkinghours.IsVisible = false;
                                        if (objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper() == "2W")
                                        {
                                            passvehicletype = "two wheeler";
                                        }
                                        else if (objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper() == "4W")
                                        {
                                            passvehicletype = "four wheeler";
                                        }
                                        await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a " + passvehicletype + " pass,Please select valid vehicle type", "Ok");
                                    }
                                }
                                else
                                {
                                    stLayoutCheckIn.IsVisible = false;
                                    stlayoutCheckInPayment.IsVisible = true;
                                    slParkinghours.IsVisible = true;
                                    slGovernment.IsVisible = true;

                                }
                            }
                        }
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "EntryRegistrationNumber_TextChanged");
            }
        }
        public void RefreshControls()
        {

            entryRegistrationNumber.Text = null;
            entryPhoneNumber.Text = null;
            LoadLocationBayNumbers();
            LoadHoursPicker();

        }

        #region Dynamic VehicleType
        public async void GetAllVehicleType()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType != null && lstVehicleType.Count > 0)
                    {
                        var objlotavilability = (User)App.Current.Properties["LoginUser"];
                        LotVehicleCapability = objlotavilability.LocationParkingLotID.LotVehicleAvailabilityName;
                        lblcheckInLocation.Text = objlotavilability.LocationParkingLotID.LocationParkingLotName;
                        if (LotVehicleCapability != null && LotVehicleCapability.Length > 0)
                        {
                            var resultvehihcle = lstVehicleType.Where(v => LotVehicleCapability.Any(r => r.ToUpperInvariant().Contains(v.VehicleTypeCode))).ToList();
                            if (resultvehihcle != null & resultvehihcle.Count > 0)
                            {
                                lstVehicleType = resultvehihcle;
                            }
                        }
                        lstVehicleType = lstVehicleType.OrderBy(i => i.VehicleTypeID).ToList();
                        obslotvehicleType = new ObservableCollection<VehicleType>(lstVehicleType);
                        if (obslotvehicleType.Count > 0)
                        {
                            collstviewVehicleTye.WidthRequest = 300;
                            collstviewVehicleTye.ItemsSource = obslotvehicleType;
                            collstviewVehicleTye.SelectedItem = obslotvehicleType[0];
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "GetAllVehicleType");
            }
        }

        public async void GetSelectedVehicleType(string VehicleTypeCode)
        {
            try
            {


                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType.Count > 0)
                    {
                        var resultvehihcle = lstVehicleType.Where(v => v.VehicleTypeCode == VehicleTypeCode).ToList();
                        if (resultvehihcle != null & resultvehihcle.Count > 0)
                        {
                            obslotvehicleType = new ObservableCollection<VehicleType>(resultvehihcle);
                            if (obslotvehicleType.Count > 0)
                            {
                                for (var item = 0; item < obslotvehicleType.Count; item++)
                                {
                                    obslotvehicleType[item].VehicleDisplayImage = obslotvehicleType[item].VehicleActiveImage;
                                    obslotvehicleType[item] = obslotvehicleType[item];
                                }
                                collstviewVehicleTye.WidthRequest = 90;
                                collstviewVehicleTye.ItemsSource = obslotvehicleType;
                                SelectedVehicle = obslotvehicleType[0].VehicleTypeCode;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "GetSelectedVehicleType");
            }
        }
        private void collstviewVehicleTye_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.CurrentSelection;
                var selectedvehicle = item[0] as VehicleType;
                if (!string.IsNullOrEmpty(selectedvehicle.VehicleImage))
                {
                    SelectedVehicle = selectedvehicle.VehicleTypeCode;
                    UpdateCollectionViewSelectedItem(selectedvehicle);
                    if (chkGovernment.IsChecked)
                    {
                        stLayoutCheckIn.IsVisible = true;
                        slGovVehicleImage.IsVisible = true;
                        stlayoutCheckInPayment.IsVisible = false;
                        slParkinghours.IsVisible = false;
                        lblPhoneNumber.Text = "Phone Number";
                    }
                    else
                    {
                        pickerHours.SelectedIndex = 0;
                        stLayoutCheckIn.IsVisible = false;
                        slGovVehicleImage.IsVisible = false;
                        stlayoutCheckInPayment.IsVisible = true;
                        slParkinghours.IsVisible = true;
                        lblPhoneNumber.Text = "Phone Number (Optional)";
                        GetParkingFeeDeatils();


                    }

                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "collstviewVehicleTye_SelectionChanged");
            }
        }
        public void UpdateCollectionViewSelectedItem(VehicleType selectedVehicle)
        {
            try
            {
                for (var item = 0; item < obslotvehicleType.Count; item++)
                {
                    if (obslotvehicleType[item].VehicleTypeID == selectedVehicle.VehicleTypeID)
                    {

                        obslotvehicleType[item].VehicleDisplayImage = obslotvehicleType[item].VehicleActiveImage;
                    }
                    else
                    {
                        obslotvehicleType[item].VehicleDisplayImage = obslotvehicleType[item].VehicleInActiveImage;
                    }
                    obslotvehicleType[item] = obslotvehicleType[item];
                }
                collstviewVehicleTye.ItemsSource = obslotvehicleType;


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckIn.xaml.cs", "", "UpdateCollectionViewSelectedItem");
            }
        }

        #endregion

        #region Navigation BackButton

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterDetailHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterDetailHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {

            }
        }



        #endregion

        #region Vehicle Due Amount History ListView
        public async void LoadVehicleDueHistory()
        {
            try
            {
                ShowLoading(true);
                string vehicleType = string.Empty;
                DALMenubar dal_Menubar = new DALMenubar();
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    List<CustomerParkingSlot> lstVehicleHistory = null;
                    
                    await Task.Run(() =>
                    {
                        lstVehicleHistory = dal_Menubar.GetVehicleDueAmountHistory(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, SelectedVehicle);
                    });
                    if (lstVehicleHistory.Count > 0)
                    {
                        imagePopVehicleImage.Source = lstVehicleHistory[0].CustomerVehicleID.VehicleTypeID.VehicleTypeIcon;
                        labelPopVehicleDetails.Text = entryRegistrationNumber.Text;
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

        private async void imgHome_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);

                frmHome.BorderColor = Color.FromHex("#3293FA");
                MasterDetailHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterDetailHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                frmHome.BorderColor = Color.FromHex("#DFDFDFDF");

                ShowLoading(false);
            }
            catch (Exception ex)
            {

            }
        }
        private async void frmHomeGesture_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                frmHome.BorderColor = Color.FromHex("#3293FA");
                MasterDetailHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterDetailHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                frmHome.BorderColor = Color.FromHex("#DFDFDFDF");
                ShowLoading(false);
            }
            catch (Exception ex)
            {

            }
        }
        private async void frmOnlineSynchGesutre_Tapped(object sender, EventArgs e)
        {

            try
            {
                ShowLoading(true);
                string resultMsg = null;
                if (DeviceInternet.InternetConnected())
                {
                    frmOnlineSynch.BorderColor = Color.FromHex("#3293FA");
                    var loguser = (User)App.Current.Properties["LoginUser"];
                    resultMsg = await dal_DALCheckIn.CheckInOfflineSync(Convert.ToString(App.Current.Properties["apitoken"]), loguser);

                    lblOfflineRecCount.Text = "0";
                    frmOnlineSynch.BorderColor = Color.FromHex("#DFDFDFDF");

                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");

                }
                if (!string.IsNullOrEmpty(resultMsg))
                {
                    await DisplayAlert("Alert", "Offline Vehicles: " + resultMsg, "Ok");
                }

                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "frmOnlineSynchGesutre_Tapped");
            }
        }
        public async Task LoadOfflineCheckInsCount()
        {
            int offlinceCount = 0;
            try
            {

                List<VehicleCheckIn> lstchekIns = await App.SQLiteDb.GetAllVehicleAsync();
                if (lstchekIns != null)
                {
                    if (lstchekIns.Count > 0)
                    {
                        offlinceCount = lstchekIns.Count;
                    }
                }
                lblOfflineRecCount.Text = Convert.ToString(offlinceCount);

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadLocationBayNumbers");
            }
        }
    }
}