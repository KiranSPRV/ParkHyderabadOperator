using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.IO;
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
            slFOC.IsVisible = false;
            imageViolation.IsVisible = false;
            GetViolationVehcileInformation(CustomerParkingSlotID);

        }
        public void GetViolationVehcileInformation(int customerParkingSlotID)
        {

            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
            {
                objresult = dal_Home.GetSelectedParkedVehicleDetails(Convert.ToString(App.Current.Properties["apitoken"]), customerParkingSlotID);
                if (objresult.CustomerParkingSlotID != 0)
                {


                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    if (objloginuser.UserTypeID.UserTypeName.ToUpper() == "Supervisor".ToUpper())
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
                    if (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "2W")
                    {
                        imageVehicleImage.Source = "bike_black.png";
                    }
                    else if (Convert.ToString(objresult.VehicleTypeID.VehicleTypeCode) == "4W")
                    {
                        imageVehicleImage.Source = "car_black.png";
                    }
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
                            labelParkingFee.Text = objInputs.ParkingFee == 0 ? "0.00" : objInputs.ParkingFee.ToString("N2");
                            objresult.Amount = objInputs.ParkingFee;
                            objresult.ViolationFees = objInputs.TotalFee == 0 ? 0 : (Convert.ToDecimal(objInputs.TotalFee) - (objInputs.ClampFee));
                            lblWarningCount.Text = (objresult.ViolationWarningCount > 3) ? "" : Convert.ToString(objresult.ViolationWarningCount) + " Warning(s)";
                            labelClampFee.Text = objresult.IsWarning ? "0.00" : objresult.ClampFees.ToString("N2");
                            labelTotalFee.Text = (objresult.ViolationFees + objresult.ClampFees).ToString("N2");
                            objresult.Duration = Convert.ToString(objInputs.TotalHours);
                            labelParkingHours.Text = "Parked for " + string.Format(objInputs.TotalHours + "h:" + t.Minutes + "m");

                        }


                    }
                    
                }

                else
                {

                    DisplayAlert("Alert", "Vehicle already checkout", "Ok");

                }
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
            { }
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
            { }
        }
        private async void ImageViolation_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (objresult.VehicleParkingImage != null)
                {

                    popupImageView.IsVisible = true;
                    imgViolationpopupImage.Source = ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress(objresult.VehicleParkingImage)));
                    //GeocodingDetails objgeodetails = new GeocodingDetails();
                    // var resultloc =await objgeodetails.GetGeoCodingPlaceMark( Convert.ToDouble(objresult.VehicleImageLottitude), Convert.ToDouble(objresult.VehicleImageLongitude));
                    labelViolationImageLocation.Text = objresult.VehicleImageLottitude + "," + objresult.VehicleImageLongitude + Environment.NewLine + Convert.ToDateTime(objresult.CreatedOn).ToString("dd MMM yyyy");
                }

            }
            catch (Exception ex)
            {

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

            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;


        }
    }
}