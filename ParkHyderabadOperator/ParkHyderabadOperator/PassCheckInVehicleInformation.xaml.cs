using ParkHyderabadOperator.DAL.DALCheckOut;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassCheckInVehicleInformation : ContentPage
    {
        DALHome objHome;
        CustomerParkingSlot objresult;
        DALViolationandClamp dal_ViolationClamp;
        DALExceptionManagment dal_Exceptionlog;
        List<ViolationReason> lstReasons = null;
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[17]; // Receipt Lines

        public PassCheckInVehicleInformation()
        {
            InitializeComponent();
            objHome = new DALHome();
            slClampReason.IsVisible = false;
            slFOC.IsVisible = false;
            slCashAndEpay.IsVisible = false;
            dal_ViolationClamp = new DALViolationandClamp();
            dal_Exceptionlog = new DALExceptionManagment();
            lstReasons = new List<ViolationReason>();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
        }
        public PassCheckInVehicleInformation(int CustomerParkingLotID)
        {
            InitializeComponent();
            objHome = new DALHome();
            slClampReason.IsVisible = false;
            slFOC.IsVisible = false;
            dal_ViolationClamp = new DALViolationandClamp();
            dal_Exceptionlog = new DALExceptionManagment();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadGetViolationReasons();
            LoadParkedVehicleDetails(CustomerParkingLotID);
        }
        private void LoadParkedVehicleDetails(int customerParkingLotID)
        {
            try
            {
                string vehicleType = string.Empty;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    objresult = objHome.GetSelectedParkedVehicleDetails(Convert.ToString(App.Current.Properties["apitoken"]), customerParkingLotID);
                    if (objresult.CustomerParkingSlotID != 0)
                    {

                        labelParkingLocation.Text = objresult.LocationParkingLotID.LocationID.LocationName + "-" + objresult.LocationParkingLotID.LocationParkingLotName;
                        labelBayNumber.Text = "Bay Number " + objresult.LocationParkingLotID.ParkingBayID.ParkingBayRange;
                        labelCheckInBy.Text = objresult.CreatedByName + " #" + objresult.UserCode;

                        labelCheckInFrom.Text = objresult.ExpectedStartTime == null ? "" : Convert.ToDateTime(objresult.ExpectedStartTime).ToString("dd MMM yyyy, hh:mm tt");
                        labelCheckInTo.Text = objresult.ActualEndTime == null ? "" : Convert.ToDateTime(objresult.ActualEndTime).ToString("dd MMM yyyy, hh:mm tt");
                        imageParkingFeeImage.Source = "rupee_black.png";

                        decimal parkingAmount = objresult.PaidAmount;
                        labelParkingFeesDetails.Text = parkingAmount.ToString("N2") + "/-";

                        TimeSpan parkingduration = Convert.ToDateTime(objresult.ActualEndTime) - Convert.ToDateTime(objresult.ExpectedStartTime);
                        if (objresult.Duration != "" && objresult.Duration != string.Empty)
                        {
                            var parkhours = ((Math.Abs(parkingduration.Hours) == 0 || Math.Abs(parkingduration.Hours) == 1)) ? Convert.ToInt32(objresult.Duration) : Math.Abs(parkingduration.Hours);
                            labelParkingPaymentType.Text = "Paid for " + string.Format(parkhours + "hr") + " - By " + objresult.PaymentTypeID.PaymentTypeName;
                        }
                        
                        labelVehicleDetails.Text = objresult.CustomerVehicleID.RegistrationNumber;
                        imageVehicleImage.Source = (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "2W" ? "bike_black.png" : (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "4W") ? "car_black.png" : "bike_black.png");
                        vehicleType = (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "2W" ? "BIKE" : (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "4W") ? "CAR" : "BIKE");

                        User objloginuser = (User)App.Current.Properties["LoginUser"];

                        slSpotExpireTimeDisplay.IsVisible = true;
                        slVehicleWarning.IsVisible = (objresult.ViolationWarningCount >= 3) ? false : true;
                        lblWarningCount.Text = (objresult.ViolationWarningCount > 3) ? "" : Convert.ToString(objresult.ViolationWarningCount) + " Warning(s) Completed";
                        lblWarningCount.IsVisible = true;
                        imgbtnPrint.IsVisible = true;
                        if (objloginuser.UserTypeID.UserTypeName.ToUpper() != "Operator".ToUpper())
                        {
                            slFOC.IsVisible = true;
                        }
                        if (objresult.ApplicationTypeID.ApplicationTypeCode == "P")
                        {
                            labelParkingFeesDetails.Text = "Pass Check-In";
                            labelParkingPaymentType.Text = "";
                            imageParkingFeeImage.Source = "";

                        }
                        if (objresult.StatusID.StatusCode.ToUpper() == "G")
                        {
                            imgbtnPrint.IsVisible = false;
                            slSpotExpireTimeDisplay.IsVisible = false;
                            slFeesDetails.IsVisible = false;
                            slFOC.IsVisible = false;
                            slClamp.IsVisible = false;
                            chkWarning.IsVisible = false;
                            slVehicleWarning.IsVisible = false;
                            labelParkingFeesDetails.Text = "Free of charge - Government Vehicle";
                            labelParkingPaymentType.Text = "";
                            imgGovPhone.Source = "phone.png";
                            imgGovPhone.HeightRequest = 20;
                            labelPhoneNumber.Text = objresult.CustomerID.PhoneNumber;
                            lblWarningCount.IsVisible = false;
                            if (objresult.GovernmentVehicleImage != null)
                            {
                                imageGovernmentVehicle.Source = ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress(objresult.GovernmentVehicleImage)));
                                imageGovernmentVehicle.HeightRequest = 150;
                                labelGovImageLocation.Text = objresult.VehicleImageLottitude + "," + objresult.VehicleImageLongitude + Environment.NewLine + Convert.ToDateTime(objresult.CreatedOn).ToString("dd MMM yyyy");
                            }

                            else
                            {
                                imageGovernmentVehicle.IsVisible = false;
                                imageGovernmentVehicle.HeightRequest = 0;
                            }
                        }
                        if (objresult.ActualEndTime != null)
                        {

                            DateTime exptendime = Convert.ToDateTime(objresult.ActualEndTime);
                            DateTime CurrentTime = DateTime.Now;
                            TimeSpan t = CurrentTime - exptendime;
                            labelSpotExpiresTime.Text = string.Format("{0:%h} h : {0:%m} m", t);
                        }
                        #region Clamp and Warning data loading


                        checkBoxClampVehicle.IsChecked = objresult.IsClamp;
                        checkBoxClampVehicle.IsEnabled = !objresult.IsClamp;
                        chkWarning.IsChecked = objresult.IsWarning;
                        chkWarning.IsEnabled = !objresult.IsWarning;
                        slCash.IsVisible = (objresult.ClampFees > 0) ? true : false;
                        slEPay.IsVisible = (objresult.ClampFees > 0) ? true : false;
                        lblClampFees.Text = (objresult.ClampFees > 0) ? objresult.ClampFees.ToString("N2") : "0.00";
                        lblParkingFees.Text = "0.00";
                        lblTotal.Text = objresult.ClampFees.ToString("N2");
                        if (objresult.StatusID.StatusCode.ToUpper() == "G")
                        {
                            slCheckOut.IsVisible = false;
                            if (objloginuser.UserTypeID.UserTypeName.ToUpper() != "Operator".ToUpper())
                            {
                                slCheckOut.IsVisible = true;
                                imgbtnPrint.IsVisible = true;
                            }
                        }
                        else
                        {
                            slCheckOut.IsVisible = (objresult.ClampFees > 0) ? false : true;
                        }
                        if (objresult.ViolationReasonID.ViolationReasonID != 0)
                        {
                            for (int x = 0; x < lstReasons.Count; x++)
                            {
                                if (lstReasons[x].ViolationReasonID == objresult.ViolationReasonID.ViolationReasonID)
                                {
                                    pickerViolationReason.SelectedIndex = x;
                                    pickerViolationReason.IsEnabled = false;
                                    checkBoxClampVehicle.IsEnabled = false;
                                    chkWarning.IsEnabled = false;
                                }
                            }
                            slClampReason.IsVisible = true;

                        }
                        frmClampbutton.IsVisible = false;
                        slPaymentAndExpand.IsVisible = true;
                        #endregion
                        try
                        {
                            if (receiptlines != null && receiptlines.Length > 0)
                            {

                                receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                                receiptlines[1] = "\x1B\x21\x01" + "          " + objresult.LocationParkingLotID.LocationID.LocationName + objresult.LocationParkingLotID.LocationParkingLotName + "\x1B\x21\x00\n";
                                receiptlines[2] = "" + "\n";
                                receiptlines[3] = "\x1B\x21\x08" + vehicleType + ":" + objresult.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                                receiptlines[4] = "\x1B\x21\x01" + (objresult.ActualStartTime == null ? "" : "In:" + Convert.ToDateTime(objresult.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                                receiptlines[5] = "\x1B\x21\x01" + "Paid: Rs" + objresult.Amount.ToString("N2") + "(Up to " + objresult.Duration + " hours)" + "\x1B\x21\x00\n";
                                receiptlines[6] = "\x1B\x21\x01" + "Valid Till:" + (objresult.ActualEndTime == null ? "" : Convert.ToDateTime(objresult.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00\n";
                                receiptlines[7] = "\x1B\x21\x01" + "Parked at: (Bays)" + objresult.LocationParkingLotID.ParkingBayID.ParkingBayRange + "\x1B\x21\x00\n";
                                receiptlines[8] = "\x1B\x21\x06" + "Operator Id :" + objresult.UserCode + "\x1B\x21\x00\n";
                                receiptlines[9] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objresult.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                                receiptlines[10] = "\x1B\x21\x06" + "Security available " + objresult.LocationParkingLotID.LotOpenTime + "-" + objresult.LocationParkingLotID.LotCloseTime + "\x1B\x21\x00\n";
                                receiptlines[11] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                                receiptlines[12] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                                receiptlines[13] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                                receiptlines[14] = "" + "\n";
                                receiptlines[15] = "" + "\n";

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        objresult.CreatedBy = objloginuser.UserID;


                    }
                    else
                    {

                        DisplayAlert("Alert", "Vehicle details unvailable,Please contact Admin", "Ok");

                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "LoadParkedVehicleDetails");


            }
        }

        #region Clamp Related Code
        private void LoadGetViolationReasons()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstReasons = dal_ViolationClamp.GetViolationReasons(Convert.ToString(App.Current.Properties["apitoken"]), "CHKIN");
                    pickerViolationReason.ItemsSource = lstReasons;
                }
            }
            catch (Exception ex)
            {

                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "LoadGetViolationReasons");
            }
        }
        private void CheckBoxClampVehicle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if ((checkBoxClampVehicle.IsChecked) || (chkWarning.IsChecked))
                {
                    slClampReason.IsVisible = true;
                    slPaymentAndExpand.IsVisible = false;
                    frmClampbutton.IsVisible = true;
                }
                else
                {
                    slClampReason.IsVisible = false;
                    slPaymentAndExpand.IsVisible = true;
                    frmClampbutton.IsVisible = false;
                }

                if (checkBoxClampVehicle.IsChecked)
                {
                    if (objresult.ViolationWarningCount < 3)
                    {
                        chkWarning.IsChecked = true;
                    }
                }


                if (chkWarning.IsChecked)
                {

                    BtnClamp.Text = "Warning";

                }
                if (checkBoxClampVehicle.IsChecked)
                {
                    BtnClamp.Text = "Clamp";

                }
            }
            catch (Exception ex)
            {

            }

        }
        private void ChkWarning_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

            try
            {
                if ((checkBoxClampVehicle.IsChecked) || (chkWarning.IsChecked))
                {
                    slClampReason.IsVisible = true;
                    slPaymentAndExpand.IsVisible = false;
                    frmClampbutton.IsVisible = true;
                }
                else
                {
                    slClampReason.IsVisible = false;
                    frmClampbutton.IsVisible = false;
                    slPaymentAndExpand.IsVisible = true;
                }

                if (checkBoxClampVehicle.IsChecked)
                {
                    if (chkWarning.IsChecked)
                    {
                        if (objresult.ViolationWarningCount < 3)
                        {
                            chkWarning.IsChecked = true;
                        }
                    }

                }


                if (checkBoxClampVehicle.IsChecked)
                {
                    BtnClamp.Text = "Clamp";

                }
                if (chkWarning.IsChecked)
                {
                    BtnClamp.Text = "Warning";

                }
            }
            catch (Exception ex)
            {

            }

        }

        #endregion
        private async void BtnExtendedTime_Clicked(object sender, EventArgs e)
        {
            try
            {
                var checkIn = new CheckIn(objresult);
                await Navigation.PushAsync(checkIn);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Payment Failed,Please contact Admin", "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "BtnExtendedTime_Clicked");
            }
        }
        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                BtnCheckOut.IsVisible = false;
                CustomerParkingSlot objcheckoutresult = null;
                MasterHomePage masterpage = null;
                DALVehicleCheckOut dal_VehicleCheckOut = new DALVehicleCheckOut();
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objCheckOutBy = (User)App.Current.Properties["LoginUser"];
                    objresult.ActualEndTime = DateTime.Now;
                    await Task.Run(() =>
                    {

                        objresult.ExtendAmount = 0;
                        objresult.ClampFees = 0;
                        objresult.PaymentTypeID.PaymentTypeName = "";
                        objresult.Duration = null;
                        objresult.CreatedBy = objCheckOutBy.UserID;

                        objcheckoutresult = dal_VehicleCheckOut.VehicleCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), objresult);
                        if (objcheckoutresult != null)
                        {
                            masterpage = new MasterHomePage();
                        }
                    });
                    if (objcheckoutresult != null)
                    {

                        await Navigation.PushAsync(masterpage);
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Payment Failed,Please contact Admin", "Ok");
                    }

                }
                ShowLoading(false);
                BtnCheckOut.IsVisible = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Payment Failed,Please contact Admin", "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "BtnCheckOut_Clicked");
                ShowLoading(false);
                BtnCheckOut.IsVisible = false;
            }
        }
        private async void SlFOCPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                objresult.ActualEndTime = DateTime.Now;
                objresult.ExtendAmount = 0;

                var FOCPage = new FOCConfirmationPage(objresult, "PassCheckInVehicleInformation");
                await Navigation.PushAsync(FOCPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "SlFOCPayment_Tapped");
                ShowLoading(false);
            }
        }

        #region Clamp And Warning Saving
        private async void BtnClamp_Clicked(object sender, EventArgs e)
        {
            try
            {
                string resultmsg = string.Empty;
                ShowLoading(true);
                BtnClamp.IsEnabled = false;
                MasterHomePage masterPage = null;
                if (pickerViolationReason.SelectedItem != null)
                {
                    ViolationReason objselectedreason = (ViolationReason)pickerViolationReason.SelectedItem;
                    if (chkWarning.IsChecked || checkBoxClampVehicle.IsChecked)
                    {
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                        {
                            await Task.Run(() =>
                            {
                                User objClampedBy = (User)App.Current.Properties["LoginUser"];
                                objresult.IsClamp = checkBoxClampVehicle.IsChecked;
                                objresult.IsWarning = chkWarning.IsChecked;
                                objresult.ViolationReasonID.ViolationReasonID = objselectedreason.ViolationReasonID;
                                objresult.CreatedBy = objClampedBy.UserID;
                                resultmsg = dal_ViolationClamp.UpdaetVehicleClampStatus(Convert.ToString(App.Current.Properties["apitoken"]), objresult);
                                if (resultmsg == "Success")
                                {
                                    masterPage = new MasterHomePage();
                                }
                            });
                            if (resultmsg == "Success")
                            {
                                await Navigation.PushAsync(masterPage);
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Fai,Please contact Admin", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Unable to get user details,Please contact Admin", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please select warning or clamp", "Ok");
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Reason", "Ok");
                }
                ShowLoading(true);
                BtnClamp.IsEnabled = true;
            }
            catch (Exception ex)
            {
                BtnClamp.IsEnabled = true;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassCheckInVehicleInformation.xaml.cs", "", "BtnClamp_Clicked");
            }
        }

        #endregion

        #region Clamp CheckOut
        private async void SlEPAYPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                objresult.ActualEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "EPay";
                objresult.ExtendAmount = 0;
                var checkOutPaymentConfirmationPage = new CheckOutPaymentConfirmationPage("PassCheckInVehicleInformation", objresult);
                await Navigation.PushAsync(checkOutPaymentConfirmationPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            { }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                objresult.ActualEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "Cash";
                objresult.ExtendAmount = 0;
                var checkOutPaymentConfirmationPage = new CheckOutPaymentConfirmationPage("PassCheckInVehicleInformation", objresult);
                await Navigation.PushAsync(checkOutPaymentConfirmationPage);
                ShowLoading(false);
            }
            catch (Exception ex)
            { }
        }
        #endregion

        private async void ImgbtnPrint_Clicked(object sender, EventArgs e)
        {

            try
            {
                string printerName = string.Empty;
                MasterHomePage masterPage = null;
                try
                {
                    await Task.Run(() =>
                    {
                        printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();

                        if (printerName != string.Empty && printerName != "")
                        {
                            if (receiptlines.Length > 0)
                            {
                                for (var l = 0; l < receiptlines.Length; l++)
                                {
                                    string printtext = receiptlines[l];
                                    if (printtext != "")
                                    {
                                        ObjblueToothDevicePrinting.PrintCommand(printerName, printtext);
                                    }
                                }
                                masterPage = new MasterHomePage();
                            }

                        }
                        else
                        {
                            masterPage = new MasterHomePage();
                        }
                    });
                    if (printerName != string.Empty && printerName != "")
                    {
                        await Navigation.PushAsync(masterPage);
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Unable to find Bluetooth device", "Ok");
                        await Navigation.PushAsync(masterPage);
                    }

                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex) { }


        }

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutCheckInVehicleInfopage.Opacity = 0.5;
            }
            else
            {
                absLayoutCheckInVehicleInfopage.Opacity = 1;
            }

        }
    }
}