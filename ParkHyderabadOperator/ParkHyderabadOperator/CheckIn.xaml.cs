﻿using ParkHyderabadOperator.DAL;
using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    
        public CheckIn() // New Check-In
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            lblPageHeading.Text = "CHECK IN";
            stLayoutCheckIn.IsVisible = false;
            slGovVehicleImage.IsVisible = false;
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            lstparkingbay = new List<ParkingBay>();
            LoadLocationBayNumbers();
            SlTwoWheeler_Tapped(this, new EventArgs());
            LoadHoursPicker();
            LoadMinutesPicker();
            defaultHours = 0;


        }
        public CheckIn(CustomerParkingSlot objextendChekIn)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            lblPageHeading.Text = "EXTEND TIME";
            stLayoutCheckIn.IsVisible = false;
            slGovVehicleImage.IsVisible = false;
            slGovernment.IsVisible = false;
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            lstparkingbay = new List<ParkingBay>();
            objextendCustomerParkingSlot = objextendChekIn;
            LoadLocationBayNumbers();
            LoadExtendedHoursPicker();
            LoadMinutesPicker();
            LoadVehicleExtension(objextendCustomerParkingSlot);
            defaultHours = 0;
            pickerHours.SelectedIndex = defaultHours;
            GetParkingFeeDeatils();
        } //  Extende Vehicle Check-In
        public async void LoadLocationBayNumbers()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    lstparkingbay = dal_DALCheckIn.GetLocationParkingBay(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser.LocationParkingLotID);
                    if (lstparkingbay.Count > 0)
                    {
                        pickerBayNumers.ItemsSource = lstparkingbay;
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please contact Admin,(Check Bay numbers/Lot timing)", "Ok");
                    }

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
                    pickerHours.SelectedIndex = 0;
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
        public void LoadMinutesPicker()
        {
            try
            {
                List<string> lstparkingMinutes = dal_DALCheckIn.GetParkingMinutes();
                if (lstparkingMinutes.Count > 0)
                {
                    pickerMinutes.ItemsSource = lstparkingMinutes;
                    pickerMinutes.SelectedIndex = 0;

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadMinutesPicker");
            }
        }
        public void LoadVehicleExtension(CustomerParkingSlot objextendChekIn)
        {
            try
            {
                entryRegistrationNumber.IsEnabled = false;

                pickerBayNumers.IsEnabled = false;
                SelectedVehicle = objextendCustomerParkingSlot.VehicleTypeID.VehicleTypeCode;
                entryRegistrationNumber.Text = objextendCustomerParkingSlot.CustomerVehicleID.RegistrationNumber;
                entryPhoneNumber.Text = objextendCustomerParkingSlot.PhoneNumber;
                if (SelectedVehicle == "2W")
                {
                    imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle_ticked.png");
                    imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
                    slFourWheelerImage.IsEnabled = false;
                }
                else if (SelectedVehicle == "4W")
                {
                    imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                    imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle_ticked.png");
                    slTwoWheelerImage.IsEnabled = false;
                }
                for (int x = 0; x < lstparkingbay.Count; x++)
                {
                    if (lstparkingbay[x].ParkingBayID == objextendCustomerParkingSlot.LocationParkingLotID.ParkingBayID.ParkingBayID)
                    {
                        pickerBayNumers.SelectedIndex = x;
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "LoadMinutesPicker");
            }

        }
        private void PickerHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedVehicle != string.Empty)
            {

                GetParkingFeeDeatils();
            }
            else
            {
                pickerHours.SelectedIndex = 0;
                pickerMinutes.SelectedIndex = 0;
            }


        }

        #region Vehicle Type Selection

        private async void SlTwoWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                SelectedVehicle = "2W";
                imgBtnTwoWheeler.Source = "Twowheeler_circle_ticked.png";
                imgBtnFourWheeler.Source = "Fourwheeler_circle.png";
                imgBtnTwoWheeler.HeightRequest = 50;
                List<ParkingBay> twowheelerbayNumbers = null;
                if (lstparkingbay.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        twowheelerbayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList();
                    });
                    if (twowheelerbayNumbers != null)
                    {
                        pickerBayNumers.ItemsSource = twowheelerbayNumbers;
                        pickerHours.SelectedIndex = defaultHours;


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
                            stLayoutCheckIn.IsVisible = false;
                            slGovVehicleImage.IsVisible = false;
                            stlayoutCheckInPayment.IsVisible = true;
                            slParkinghours.IsVisible = true;
                            lblPhoneNumber.Text = "Phone Number (Optional)";
                            GetParkingFeeDeatils();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Bay numbers unavailable", "Ok");
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "SlTwoWheeler_Tapped");
            }
        }
        private async void SlFourWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                SelectedVehicle = "4W";
                imgBtnTwoWheeler.Source = "Twowheeler_circle.png";
                imgBtnFourWheeler.Source = "Fourwheeler_circle_ticked.png";
                imgBtnTwoWheeler.HeightRequest = 50;
                List<ParkingBay> fourwheelerbayNumbers = null;

                if (lstparkingbay.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        fourwheelerbayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList();
                    });
                    if (fourwheelerbayNumbers != null)
                    {
                        pickerBayNumers.ItemsSource = fourwheelerbayNumbers;
                        pickerHours.SelectedIndex = defaultHours;// for 2hr default;
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
                            stLayoutCheckIn.IsVisible = false;
                            slGovVehicleImage.IsVisible = false;
                            stlayoutCheckInPayment.IsVisible = true;
                            slParkinghours.IsVisible = true;
                            lblPhoneNumber.Text = "Phone Number (Optional)";
                            GetParkingFeeDeatils();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Bay numbers unavailable", "Ok");
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "SlFourWheeler_Tapped");
            }
        }
        #endregion

        #region Parking Fee Details
        public async void GetParkingFeeDeatils()
        {
            try
            {
                List<VehicleParkingFee> lstLocationParkingLotVehiclePrice = new List<VehicleParkingFee>();
                DateTime checkInStarttime;
                decimal paidParkingFees = 0;
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

                        lstLocationParkingLotVehiclePrice = dal_DALCheckIn.GetLocationParkingLotVehicleParkingFees(Convert.ToString(App.Current.Properties["apitoken"]), SelectedVehicle, Convert.ToInt32(pickerHours.SelectedItem.ToString()), objloginuser.LocationParkingLotID.LocationParkingLotID, paidParkingFees);
                        if (lstLocationParkingLotVehiclePrice.Count > 0)
                        {
                            labelSpotExpiresMessage.Text = string.Empty;
                            labelParkingFee.Text = string.Empty;
                            for (var f = 0; f < lstLocationParkingLotVehiclePrice.Count; f++)
                            {
                                if (lstLocationParkingLotVehiclePrice[f].DayOfWeek.ToUpper() == DateTime.Now.DayOfWeek.ToString().ToUpper())
                                {
                                    DateTime openTime = DateTime.Parse(lstLocationParkingLotVehiclePrice[f].LotOpenTime);
                                    DateTime closetime = DateTime.Parse(lstLocationParkingLotVehiclePrice[f].LotCloseTime);
                                    Lotclosetime = DateTime.Parse(lstLocationParkingLotVehiclePrice[f].LotCloseTime);
                                    IsParkingFullDay = Convert.ToBoolean(lstLocationParkingLotVehiclePrice[f].IsFullDay);
                                    if (checkInStarttime < closetime)
                                    {
                                        double checkintimeduration = Math.Round((closetime - checkInStarttime).TotalMinutes);
                                        if (checkInStarttime < openTime)
                                        {
                                            checkInStarttime = openTime;
                                        }
                                        checkInTime = checkInStarttime;
                                        if (checkInStarttime.AddHours(Convert.ToInt32(lstLocationParkingLotVehiclePrice[f].Duration)) < closetime)
                                        {
                                            labelSpotExpiresMessage.Text = string.Empty;
                                            labelParkingFee.Text = lstLocationParkingLotVehiclePrice[f].Fees.ToString("N");
                                            labelSpotExpiresMessage.Text = checkInStarttime.AddHours(Convert.ToInt32(lstLocationParkingLotVehiclePrice[f].Duration)).ToString("hh:mm tt");
                                            stlayoutCheckInPayment.IsVisible = true;
                                        }
                                        else
                                        {
                                            if (checkintimeduration < minimumcheckinMinutes)
                                            {
                                                if (lsthoursPicker.Count > 0)
                                                {
                                                    string minHours = lsthoursPicker.Min();
                                                    lstLocationParkingLotVehiclePrice = dal_DALCheckIn.GetLocationParkingLotVehicleParkingFees(Convert.ToString(App.Current.Properties["apitoken"]), SelectedVehicle, Convert.ToInt32(minHours), objloginuser.LocationParkingLotID.LocationParkingLotID, 0);
                                                    if (lstLocationParkingLotVehiclePrice.Count > 0)
                                                    {
                                                        IsminChckinTime = true;
                                                        pickerHours.SelectedItem = lsthoursPicker.Min();
                                                        labelSpotExpiresMessage.Text = string.Empty;
                                                        labelParkingFee.Text = lstLocationParkingLotVehiclePrice[0].Fees.ToString("N");
                                                        labelSpotExpiresMessage.Text = lstLocationParkingLotVehiclePrice[0].LotCloseTime;
                                                        Lotclosetime = DateTime.Parse(lstLocationParkingLotVehiclePrice[f].LotCloseTime);
                                                        stlayoutCheckInPayment.IsVisible = true;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                stlayoutCheckInPayment.IsVisible = false;
                                                await DisplayAlert("Alert", "Please check Lot timings from " + lstLocationParkingLotVehiclePrice[f].LotOpenTime + " to " + lstLocationParkingLotVehiclePrice[f].LotCloseTime, "Ok");
                                                return;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        stlayoutCheckInPayment.IsVisible = false;
                                        pickerBayNumers.IsEnabled = false;
                                        await DisplayAlert("Alert", "Please check Lot timings from " + lstLocationParkingLotVehiclePrice[f].LotOpenTime + " to " + lstLocationParkingLotVehiclePrice[f].LotCloseTime, "Ok");
                                        return;
                                    }
                                }
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
                                    if (labelParkingFee.Text != "" && labelParkingFee.Text != string.Empty)
                                    {
                                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                        {
                                            // Vehicle Check-In
                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                            VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                            objPassVehicle.UserID = objloginuser.UserID;
                                            objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objPassVehicle.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objPassVehicle.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                            objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                            objPassVehicle.BayNumber = objselectedbay.ParkingBayName;
                                            objPassVehicle.BayRange = objselectedbay.ParkingBayRange;
                                            objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                            objPassVehicle.PhoneNumber = entryPhoneNumber.Text;
                                            objPassVehicle.ParkingHours = selectedhours;
                                            objPassVehicle.ParkingFees = Convert.ToDecimal(labelParkingFee.Text);
                                            objPassVehicle.PaymentType = "Cash";
                                            if (objextendCustomerParkingSlot == null)
                                            {
                                                existingCheckIn = VerifyVehicleCheckInStatus();
                                                if (existingCheckIn == string.Empty)
                                                {
                                                    objPassVehicle.ParkingStartTime = checkInTime.ToString("MM/dd/yyyy hh:mm tt");
                                                    if (IsParkingFullDay)
                                                    {

                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");
                                                    }
                                                    else
                                                    {
                                                        if (!IsminChckinTime)
                                                        {
                                                            objPassVehicle.ParkingEndTime = checkInTime.AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm tt");
                                                        }
                                                        else
                                                        {
                                                            objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");
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
                                                objPassVehicle.ParkingStartTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).ToString("MM/dd/yyyy hh:mm tt");
                                                objPassVehicle.ClampFees = Convert.ToDecimal(objextendCustomerParkingSlot.ClampFees);
                                                if (IsParkingFullDay)
                                                {
                                                    TimeSpan extfullday = (Lotclosetime - Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime));
                                                    int lotHours = Convert.ToInt32(extfullday.TotalHours);
                                                    objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");

                                                }
                                                else
                                                {
                                                    if (!IsminChckinTime)
                                                    {
                                                        objPassVehicle.ParkingEndTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm tt");
                                                    }
                                                    else
                                                    {
                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");
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
                                    if (labelParkingFee.Text != "" && labelParkingFee.Text != string.Empty)
                                    {
                                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                        {
                                            // Vehicle Check-In
                                            VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                            objPassVehicle.UserID = objloginuser.UserID;
                                            objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                            objPassVehicle.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                            objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                            objPassVehicle.LocationParkingLotName = objloginuser.LocationParkingLotID.LocationParkingLotName;
                                            objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                            objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                            objPassVehicle.BayNumber = objselectedbay.ParkingBayName;
                                            objPassVehicle.BayRange = objselectedbay.ParkingBayRange;
                                            objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                            objPassVehicle.PhoneNumber = entryPhoneNumber.Text;
                                            objPassVehicle.ParkingHours = selectedhours;
                                            objPassVehicle.ParkingFees = Convert.ToDecimal(labelParkingFee.Text);
                                            objPassVehicle.PaymentType = "EPay";

                                            if (objextendCustomerParkingSlot == null)
                                            {
                                                existingCheckIn = VerifyVehicleCheckInStatus();
                                                if (existingCheckIn == string.Empty)
                                                {
                                                    objPassVehicle.ParkingStartTime = checkInTime.ToString("MM/dd/yyyy hh:mm tt");
                                                    if (IsParkingFullDay)
                                                    {

                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");

                                                    }
                                                    else
                                                    {
                                                        if (!IsminChckinTime)
                                                        {
                                                            objPassVehicle.ParkingEndTime = checkInTime.AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm tt");
                                                        }
                                                        else
                                                        {
                                                            objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");
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
                                                objPassVehicle.ParkingStartTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).ToString("MM/dd/yyyy hh:mm tt");

                                                if (IsParkingFullDay)
                                                {

                                                    objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");

                                                }
                                                else
                                                {
                                                    if (!IsminChckinTime)
                                                    {
                                                        objPassVehicle.ParkingEndTime = Convert.ToDateTime(objextendCustomerParkingSlot.ActualEndTime).AddHours(selectedhours).ToString("MM/dd/yyyy hh:mm tt");
                                                    }
                                                    else
                                                    {
                                                        objPassVehicle.ParkingEndTime = Lotclosetime.ToString("MM/dd/yyyy hh:mm tt");
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
        private void ChkGovernment_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
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
                    CustomerParkingSlot resultobj = dal_DALCheckIn.VerifyVehicleChekcInStatus(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                    if (resultobj.CustomerParkingSlotID != 0)
                    {
                        alreadyCheckIn = resultobj.LocationParkingLotID.LocationID.LocationName + "-" + resultobj.LocationParkingLotID.LocationParkingLotName;
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
                                await Task.Run(() =>
                                {
                                    objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, objloginuser.LocationParkingLotID.LocationID.LocationID, objloginuser.LocationParkingLotID.LocationParkingLotID, objloginuser.UserID, "");
                                });
                                if (objCustomerPass.CustomerVehiclePassID != 0 && Convert.ToDateTime(objCustomerPass.ExpiryDate).Date >= DateTime.Now.Date)
                                {
                                    if(SelectedVehicle == objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper())
                                    {
                                        if ((objCustomerPass.PassPriceID.StationAccess == "All Stations" || objCustomerPass.PassPriceID.StationAccess == "All Station"))
                                        {

                                            stLayoutCheckIn.IsVisible = true;
                                            stlayoutCheckInPayment.IsVisible = false;
                                            slParkinghours.IsVisible = false;
                                            SelectedVehicle = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper();
                                            if (SelectedVehicle == "2W")
                                            {

                                                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle_ticked.png");
                                                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
                                            }
                                            else if (SelectedVehicle == "4W")
                                            {
                                                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                                                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle_ticked.png");
                                            }

                                            await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                        }
                                        else if (objCustomerPass.LocationID.LocationID == objloginuser.LocationParkingLotID.LocationID.LocationID)
                                        {
                                            stLayoutCheckIn.IsVisible = true;
                                            stlayoutCheckInPayment.IsVisible = false;
                                            slParkinghours.IsVisible = false;
                                            SelectedVehicle = objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper();
                                            if (SelectedVehicle == "2W")
                                            {

                                                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle_ticked.png");
                                                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
                                            }
                                            else if (SelectedVehicle == "4W")
                                            {
                                                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                                                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle_ticked.png");
                                            }
                                            await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                        }
                                        else
                                        {
                                            stLayoutCheckIn.IsVisible = false;
                                            stlayoutCheckInPayment.IsVisible = true;
                                            slParkinghours.IsVisible = true;
                                            if (SelectedVehicle == string.Empty || SelectedVehicle == "")
                                            {
                                                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                                                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
                                            }
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
                                        await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a "+ passvehicletype + " pass,Please select valid vehicle type", "Ok");
                                    }
                                }
                                else
                                {
                                    stLayoutCheckIn.IsVisible = false;
                                    stlayoutCheckInPayment.IsVisible = true;
                                    slParkinghours.IsVisible = true;
                                    slGovernment.IsVisible = true;
                                    if (SelectedVehicle == string.Empty || SelectedVehicle == "")
                                    {
                                        imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                                        imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
                                    }
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
            imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
            imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle.png");
            entryRegistrationNumber.Text = null;
            entryPhoneNumber.Text = null;
            LoadLocationBayNumbers();
            LoadHoursPicker();
            LoadMinutesPicker();
        }
        private void PickerBayNumers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CheckInVehicleValidation();
               
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "PickerBayNumers_SelectedIndexChanged");
            }
        }

        #region Navigation BackButton

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {

            }
        }

        private async void SlBackbuttonClick_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

    }
}