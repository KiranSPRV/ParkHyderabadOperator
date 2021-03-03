using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
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
    public partial class ViolationVehicleInformation : ContentPage
    {
        DALHome dal_Home;
        DALViolationandClamp dal_violation;
        CustomerParkingSlot objresult;
        DALExceptionManagment dal_Exceptionlog;
        public ViolationVehicleInformation()
        {
            InitializeComponent();
            dal_Home = new DALHome();
            dal_violation = new DALViolationandClamp();
            slFOC.IsVisible = false;
            imageViolation.IsVisible = false;
        }
        public ViolationVehicleInformation(int CustomerParkingSlotID)
        {
            InitializeComponent();
            dal_Home = new DALHome();
            dal_violation = new DALViolationandClamp();
            dal_Exceptionlog = new DALExceptionManagment();
            slFOC.IsVisible = false;
            imageViolation.IsVisible = false;
            GetViolationVehcileInformation(CustomerParkingSlotID);

        }
        public void GetViolationVehcileInformation(int customerParkingSlotID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    objresult = dal_Home.GetSelectedParkedVehicleDetails(Convert.ToString(App.Current.Properties["apitoken"]), customerParkingSlotID);
                    if (objresult.CustomerParkingSlotID != 0)
                    {

                        User objloginuser = (User)App.Current.Properties["LoginUser"];
                        if (objloginuser.UserTypeID.UserTypeName.ToUpper() != "Operator".ToUpper())
                        {
                            slFOC.IsVisible = true;
                        }
                        if (objresult.VehicleParkingImage != null)
                        {
                            imageViolation.IsVisible = true;
                        }
                        labelParkingLocation.Text = objresult.LocationParkingLotID.LocationID.LocationName + "-" + objresult.LocationParkingLotID.LocationParkingLotName;
                        labelBayNumber.Text = "Bay Number " + objresult.LocationParkingLotID.ParkingBayID.ParkingBayRange;
                        labelCheckInBy.Text = objresult.CreatedByName + " #" + objresult.UserCode;
                        labelVehicleDetails.Text = objresult.CustomerVehicleID.RegistrationNumber;
                        labelValidFrom.Text = objresult.ExpectedStartTime == null ? Convert.ToDateTime(objresult.ActualStartTime).ToString("dd MMM yyyy, hh:mm tt") : Convert.ToDateTime(objresult.ExpectedStartTime).ToString("dd MMM yyyy, hh:mm tt");
                        labelValidTo.Text = objresult.ActualEndTime == null ? DateTime.Now.ToString("dd MMM yyyy, hh:mm tt") : Convert.ToDateTime(objresult.ActualEndTime).ToString("dd MMM yyyy, hh:mm tt");
                        imageVehicleImage.Source = objresult.CustomerVehicleID.VehicleTypeID.VehicleIcon;

                        DateTime starttime = objresult.ExpectedStartTime == null ? DateTime.Now : Convert.ToDateTime(objresult.ExpectedStartTime);
                        if (starttime != null)
                        {
                            DateTime CurrentTime = DateTime.Now;
                            TimeSpan t = CurrentTime - starttime;
                            if (t != null)
                            {
                                int totalHours = (t.Days > 0 ? t.Hours + (t.Days * 24) : t.Hours);
                                labelReason.Text = objresult.ViolationReasonID.Reason;
                                ViolationVehicleFees objInputs = new ViolationVehicleFees();
                                objInputs.CustomerParkingSlotId = objresult.CustomerParkingSlotID;
                                objInputs.LocationParkingLotID = objresult.LocationParkingLotID.LocationParkingLotID;
                                objInputs.ParkingStartTime = starttime;
                                objInputs.ParkingEndTime = CurrentTime;
                                objInputs.VehicleTypeCode = objresult.VehicleTypeID.VehicleTypeCode;

                                //Get Violation Fees Details
                                objInputs = dal_violation.GetViolationVehicleCharges(Convert.ToString(App.Current.Properties["apitoken"]), objInputs);
                                labelParkingFee.Text = String.Format("{0:0.#}", objInputs.ParkingFee == 0 ? 0 : objInputs.ParkingFee);
                                objresult.Amount = objInputs.ParkingFee;
                                objresult.ViolationFees = objInputs.TotalFee == 0 ? 0 : (Convert.ToDecimal(objInputs.TotalFee) - (objInputs.ClampFee));
                                lblWarningCount.Text = (objresult.ViolationWarningCount > 3) ? "" : Convert.ToString(objresult.ViolationWarningCount) + " Warning(s)";
                                labelClampFee.Text = String.Format("{0:0.#}", (objresult.IsWarning ? 0 : objresult.ClampFees));
                                labelDueAmount.Text = String.Format("{0:0.#}", objresult.DueAmount);
                                labelTotalFee.Text = String.Format("{0:0.#}", (objresult.ViolationFees + objresult.ClampFees+ objresult.DueAmount));
                                objresult.Duration = Convert.ToString(objInputs.TotalHours);
                                labelParkingHours.Text = "Parked for " + string.Format(objInputs.TotalHours + "h:" + t.Minutes + "m");

                            }


                        }

                    }

                    else
                    {

                        DisplayAlert("Alert", "Vehicle already Checkedout", "Ok");

                    }
                }
            }
            catch (Exception ex)
            {


                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "GetViolationVehcileInformation");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                objresult.ActualEndTime = DateTime.Now;
                objresult.ExpectedEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "Cash";
                await Navigation.PushAsync(new CheckOutPaymentConfirmationPage("ViolationVehicleInformation", objresult));
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "SlCashPayment_Tapped");
            }

        }
        private async void SlEPAYPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                objresult.ActualEndTime = DateTime.Now;
                objresult.PaymentTypeID.PaymentTypeName = "EPay";
                var checkOutPaymentConfirmationPage = new CheckOutPaymentConfirmationPage("ViolationVehicleInformation", objresult);
                await Navigation.PushAsync(checkOutPaymentConfirmationPage);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "SlEPAYPayment_Tapped");
            }
        }
        private async void SlFOCPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                objresult.ActualEndTime = DateTime.Now;
                var fOCConfirmationPage = new FOCConfirmationPage(objresult, "ViolationVehicleInformation");
                await Navigation.PushAsync(fOCConfirmationPage);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "SlFOCPayment_Tapped");
            }
        }
        private void ImageViolation_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (objresult.VehicleParkingImage != null)
                {

                    popupImageView.IsVisible = true;
                    imgViolationpopupImage.Source = ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress(objresult.VehicleParkingImage)));
                    labelViolationImageLocation.Text = objresult.VehicleImageLottitude + "," + objresult.VehicleImageLongitude + Environment.NewLine + Convert.ToDateTime(objresult.CreatedOn).ToString("dd MMM yyyy");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "ImageViolation_Clicked");
            }
        }
        private void ImgClosePopUp_Clicked(object sender, EventArgs e)
        {
            try
            {
                popupImageView.IsVisible = false;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationVehicleInformation.xaml.cs", "", "ImgClosePopUp_Clicked");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;


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
                    List<CustomerParkingSlot> lstHistory = null;
                    imagePopVehicleImage.Source = objresult.VehicleTypeID.VehicleIcon;
                    labelPopVehicleDetails.Text = objresult.CustomerVehicleID.RegistrationNumber;
                    await Task.Run(() =>
                    {
                        lstHistory = dal_Menubar.GetVehicleDueAmountHistory(Convert.ToString(App.Current.Properties["apitoken"]), objresult.CustomerVehicleID.RegistrationNumber, objresult.VehicleTypeID.VehicleTypeCode);
                    });
                    if (lstHistory.Count > 0)
                    {
                        lvVehicleDueAmount.ItemsSource = lstHistory;
                        
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


    }
}