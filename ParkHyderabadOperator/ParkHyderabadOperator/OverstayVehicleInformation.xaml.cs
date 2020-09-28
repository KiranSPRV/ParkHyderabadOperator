using ParkHyderabadOperator.DAL.DALCheckOut;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OverstayVehicleInformation : ContentPage
    {
        DALHome objHome;
        CustomerParkingSlot objresult;
        DALExceptionManagment dal_Exceptionlog;
        DALViolationandClamp dal_ViolationClamp;
        List<ViolationReason> lstReasons = null;
        ViolationVehicleFees objInputs = null;
        int voilationBasicMinutes = 15;
        public OverstayVehicleInformation()
        {
            InitializeComponent();
            slClampVehicle.IsVisible = false;
            slFOC.IsVisible = false;
            BtnCheckOut.IsVisible = false;
            dal_Exceptionlog = new DALExceptionManagment();
            objHome = new DALHome();
            dal_ViolationClamp = new DALViolationandClamp();
            lstReasons = new List<ViolationReason>();
        }
        public OverstayVehicleInformation(int CustomerParkingSlotID)
        {
            InitializeComponent();
            slClampVehicle.IsVisible = false;
            slFOC.IsVisible = false;
            slCash.IsVisible = false;
            slEPay.IsVisible = false;
            objHome = new DALHome();
            dal_ViolationClamp = new DALViolationandClamp();
            lstReasons = new List<ViolationReason>();
            LoadGetViolationReasons();
            LoadParkingVehicleDetails(CustomerParkingSlotID);
        }
        private void LoadParkingVehicleDetails(int customerParkingSlotID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    objresult = objHome.GetSelectedParkedVehicleDetails(Convert.ToString(App.Current.Properties["apitoken"]), customerParkingSlotID);
                    if (objresult.CustomerParkingSlotID != 0)
                    {

                        labelParkingLocation.Text = objresult.LocationParkingLotID.LocationID.LocationName + "-" + objresult.LocationParkingLotID.LocationParkingLotName;
                        labelBayNumber.Text = "Bay Number " + objresult.LocationParkingLotID.ParkingBayID.ParkingBayRange;
                        labelCheckInBy.Text = objresult.CreatedByName + " #" + objresult.UserCode;
                        labelOverstayFrom.Text = objresult.ExpectedStartTime == null ? null : Convert.ToDateTime(objresult.ExpectedStartTime).ToString("dd MMM yyyy, hh:mm tt");
                        labelOverstayTo.Text = objresult.ActualEndTime == null ? DateTime.Now.ToString("dd MMM yyyy, hh:mm tt") : Convert.ToDateTime(objresult.ActualEndTime).ToString("dd MMM yyyy, hh:mm tt");

                        User objloginuser = (User)App.Current.Properties["LoginUser"];
                        if (objloginuser.UserTypeID.UserTypeName.ToUpper() == "Supervisor".ToUpper())
                        {
                            slFOC.IsVisible = true;
                        }

                        if (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "2W")
                        {
                            imageVehicleImage.Source = "bike_black.png";
                            labelVehicleDetails.Text = objresult.CustomerVehicleID.RegistrationNumber;
                        }
                        else if (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "4W")
                        {
                            imageVehicleImage.Source = "car_black.png";
                            labelVehicleDetails.Text = objresult.CustomerVehicleID.RegistrationNumber;
                        }
                        imageParkingFeeImage.Source = "rupee_black.png";
                        labelParkingFeesDetails.Text = (objresult.PaidAmount).ToString("N2") + "/-"; //(objresult.Amount+objresult.ExtendAmount).ToString("N2") + "/-";

                        TimeSpan parkingduration = Convert.ToDateTime(objresult.ExpectedStartTime) - Convert.ToDateTime(objresult.ActualEndTime);
                        labelParkingPaymentType.Text = "Paid for " + string.Format(Math.Abs(parkingduration.Hours) + "hr") + "- By " + objresult.PaymentTypeID.PaymentTypeName;

                        DateTime? actualendime = objresult.ActualEndTime == null ? DateTime.Now : Convert.ToDateTime(objresult.ActualEndTime);
                        if (actualendime != null)
                        {
                            DateTime CurrentTime = DateTime.Now;
                            TimeSpan t = CurrentTime - Convert.ToDateTime(actualendime);
                            int totalHours = (t.Days > 0 ? t.Hours + (t.Days * 24) : t.Hours);

                            objInputs = new ViolationVehicleFees();
                            objInputs.CustomerParkingSlotId = objresult.CustomerParkingSlotID;
                            objInputs.LocationParkingLotID = objresult.LocationParkingLotID.LocationParkingLotID;
                            objInputs.ParkingStartTime = Convert.ToDateTime(actualendime);
                            objInputs.ParkingEndTime = CurrentTime;
                            objInputs.VehicleTypeCode = objresult.VehicleTypeID.VehicleTypeCode;
                            if (Convert.ToInt32(t.TotalMinutes) > voilationBasicMinutes)
                            {
                                slCheckOut.IsVisible = false;
                                slCash.IsVisible = true;
                                slEPay.IsVisible = true;
                                objInputs = dal_ViolationClamp.GetViolationVehicleCharges(Convert.ToString(App.Current.Properties["apitoken"]), objInputs);
                            }
                            labelOverstayTimeDetails.Text = "OVERSTAY- " + (objresult.ActualEndTime == null ? "" : Convert.ToDateTime(objresult.ActualEndTime).ToString("hh:mm tt")) + " TO " + DateTime.Now.ToString("hh:mm tt");
                            labelOverstayTime.Text = string.Format(objInputs.TotalHours + " h : " + t.Minutes + " m");
                            objresult.ExtendAmount = objInputs.ParkingFee;
                            objresult.Duration = Convert.ToString(objInputs.TotalHours);
                        }

                        #region Clamp and Warning data loading

                        slVehicleWarning.IsVisible = (objresult.ViolationWarningCount >= 3) ? false : true;
                        checkBoxClampVehicle.IsChecked = objresult.IsClamp;
                        checkBoxClampVehicle.IsChecked = objresult.IsClamp;
                        checkBoxClampVehicle.IsEnabled = !objresult.IsClamp;
                        chkWarning.IsChecked = objresult.IsWarning;
                        chkWarning.IsEnabled = !objresult.IsWarning;
                        lblWarningCount.Text = (objresult.ViolationWarningCount > 3) ? "" : Convert.ToString(objresult.ViolationWarningCount) + " Warning(s) Completed";
                        lblClampFees.Text = (objresult.ClampFees > 0) ? objresult.ClampFees.ToString("N2") : "0.00";
                        lblParkingFees.Text = objInputs.ParkingFee == 0 ? "0.00" : objInputs.ParkingFee.ToString("N2");
                        lblTotal.Text = (objresult.ClampFees + objInputs.ParkingFee).ToString("N2");
                        if (objresult.ViolationReasonID.ViolationReasonID != 0)
                        {
                            for (int x = 0; x < lstReasons.Count; x++)
                            {
                                if (lstReasons[x].Reason.ToUpper() == objresult.ViolationReasonID.Reason.ToUpper())
                                {
                                    pickerViolationReason.SelectedIndex = x;
                                    pickerViolationReason.IsEnabled = false;
                                    checkBoxClampVehicle.IsEnabled = false;
                                    chkWarning.IsEnabled = false;
                                }
                            }

                        }
                        slPaymentAndExpand.IsVisible = true;
                        frmClampbutton.IsVisible = false;


                        #endregion

                        
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "LoadParkingVehicleDetails");
            }

        }

        #region Clamp Related Code
        private void LoadGetViolationReasons()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstReasons = dal_ViolationClamp.GetViolationReasons(Convert.ToString(App.Current.Properties["apitoken"]), "O");
                    pickerViolationReason.ItemsSource = lstReasons;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "LoadGetViolationReasons");
            }
        }
        private void CheckBoxClampVehicle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                if (checkBoxClampVehicle.IsChecked || chkWarning.IsChecked)
                {
                    slClampVehicle.IsVisible = true;
                    frmClampbutton.IsVisible = true;
                    slPaymentAndExpand.IsVisible = false;
                }
                else
                {
                    slClampVehicle.IsVisible = false;
                    frmClampbutton.IsVisible = false;
                    slPaymentAndExpand.IsVisible = true;
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
                if (checkBoxClampVehicle.IsChecked || chkWarning.IsChecked)
                {
                    slClampVehicle.IsVisible = true;
                    frmClampbutton.IsVisible = true;
                    slPaymentAndExpand.IsVisible = false;
                }
                else
                {
                    slClampVehicle.IsVisible = false;
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "ChkWarning_CheckedChanged");
            }
        }
        private async void BtnClamp_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                BtnClamp.IsEnabled = false;
                MasterHomePage masterPage = null;
                string resultmsg = string.Empty;
                if (pickerViolationReason.SelectedItem != null)
                {
                    ViolationReason objselectedreason = (ViolationReason)pickerViolationReason.SelectedItem;
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
                            await DisplayAlert("Alert", "Fail,Please contact admin", "Ok");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please select reason", "Ok");
                }
                BtnClamp.IsEnabled = true;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                BtnClamp.IsEnabled = true;
                ShowLoading(false);
            }
        }
        #endregion
        private async void SlEPAYPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                objresult.ActualEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "EPay";

                var checkOutPaymentConfirmationPage = new CheckOutPaymentConfirmationPage("OverstayVehicleInformation", objresult);
                await Navigation.PushAsync(checkOutPaymentConfirmationPage);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "SlEPAYPayment_Tapped");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                objresult.ActualEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "Cash";
                var checkOutPaymentConfirmationPage = new CheckOutPaymentConfirmationPage("OverstayVehicleInformation", objresult);
                await Navigation.PushAsync(checkOutPaymentConfirmationPage);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "SlCashPayment_Tapped");
            }
        }
        private async void BtnExtendedTime_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(objresult.ActualEndTime).Date == DateTime.Now.Date)
                {

                    var checkIn = new CheckIn(objresult);
                    await Navigation.PushAsync(checkIn);
                }
                else
                {
                    await DisplayAlert("Alert", "Extend not avilable for " + DateTime.Now.Date.ToString("MMM dd yyyy") + ",Please contact admin", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "BtnExtendedTime_Clicked");
            }
        }
        private async void SlFOCPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                objresult.ActualEndTime = DateTime.Now;
                var FOCPage = new FOCConfirmationPage(objresult, "OverstayVehicleInformation");
                await Navigation.PushAsync(FOCPage);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "SlFOCPayment_Tapped");
            }
        }
        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            try
            {

                CustomerParkingSlot objcheckoutresult = null;
                MasterHomePage masterpage = null;
                DALVehicleCheckOut dal_VehicleCheckOut = new DALVehicleCheckOut();
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    objresult.ActualEndTime = DateTime.Now;
                    await Task.Run(() =>
                    {
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
                        await DisplayAlert("Alert", "Fail,please contact admin", "Ok");
                    }

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Fail,please contact admin", "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "OverstayVehicleInformation.xaml.cs", "", "BtnCheckOut_Clicked");
            }
        }

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutOverstaypage.Opacity = 0.5;
            }
            else
            {
                absLayoutOverstaypage.Opacity = 1;
            }

        }

    }
}