using ParkHyderabadOperator.DAL;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutReceiptPage : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[16]; // Receipt Lines
        string VehicleInformation = string.Empty;
        public CheckOutReceiptPage()
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadParkingVehicleDetails(null, null, VehicleInformation);
        }
        public CheckOutReceiptPage(string vehicleInformation)
        {
            InitializeComponent();
            VehicleInformation = vehicleInformation;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadParkingVehicleDetails(null, null, VehicleInformation);
        }
        public CheckOutReceiptPage(string vehicleInformation, CustomerParkingSlot objInput)
        {
            InitializeComponent();
            VehicleInformation = vehicleInformation;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadVehicleCheckOutDetails(vehicleInformation, objInput);
        }
        private void LoadVehicleCheckOutDetails(string pageStatus, CustomerParkingSlot objCheckOutReceipt)
        {
            try
            {
                string vehicleType = string.Empty;
                labelParkingLocation.Text = objCheckOutReceipt.LocationParkingLotID.LocationID.LocationName + " " + objCheckOutReceipt.LocationParkingLotID.LocationParkingLotName + " " + objCheckOutReceipt.LocationParkingLotID.ParkingBayID.ParkingBayName + " " + objCheckOutReceipt.LocationParkingLotID.ParkingBayID.ParkingBayRange;
                labelValidFrom.Text = objCheckOutReceipt.ActualStartTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt");
                labelValidTo.Text = objCheckOutReceipt.ActualEndTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt");
                if (objCheckOutReceipt.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    vehicleType = "BIKE";
                    imageVehicleImage.Source = ImageSource.FromFile("bike_black.jpg");
                }
                else if (objCheckOutReceipt.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    vehicleType = "CAR";
                    imageVehicleImage.Source = ImageSource.FromFile("car_black.jpg");
                }
                labelVehicleDetails.Text = objCheckOutReceipt.CustomerVehicleID.RegistrationNumber;
                imageParkingFeeImage.Source = "rupee_black.png";
                labelParkingFeesDetails.Text = (objCheckOutReceipt.ExtendAmount + objCheckOutReceipt.ViolationFees + objCheckOutReceipt.ClampFees).ToString("N2") + "/-"; //objCheckOutReceipt.PaidAmount.ToString("N2") + "/-";
                labelParkingPaymentType.Text = "Paid for " + objCheckOutReceipt.Duration + "hr - " + "By " + objCheckOutReceipt.PaymentTypeID.PaymentTypeName;
                labelCheckOutFeesDetails.Text = "(Parking" + " ₹" + (objCheckOutReceipt.ExtendAmount + objCheckOutReceipt.ViolationFees).ToString("N2") + " + " + "Clamp" + " ₹" + objCheckOutReceipt.ClampFees + ")";
                imageOperatorProfile.Source = "operator.png";

                if (objCheckOutReceipt.CreatedByName != "")
                {
                    labelOperatorName.Text = objCheckOutReceipt.CreatedByName;
                    labelOperatorID.Text = "- #" + objCheckOutReceipt.UserCode;
                    imageOperatorProfile.IsVisible = true;
                }
                else
                {
                    imageOperatorProfile.IsVisible = false;
                }


                try
                {
                    if (receiptlines != null && receiptlines.Length > 0)
                    {

                        receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                        receiptlines[1] = "\x1B\x21\x01" + "       " + objCheckOutReceipt.LocationParkingLotID.LocationID.LocationName + "-" + objCheckOutReceipt.LocationParkingLotID.LocationParkingLotName + "\n";
                        receiptlines[2] = " " + "\n";
                        receiptlines[3] = "\x1B\x21\x08" + vehicleType + "     " + objCheckOutReceipt.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00" + "\n";
                        receiptlines[4] = "\x1B\x21\x08" + objCheckOutReceipt.ActualEndTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt") + "\x1B\x21\x00\n";
                        receiptlines[5] = "" + "\n";
                        receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + (objCheckOutReceipt.ExtendAmount + objCheckOutReceipt.ViolationFees + objCheckOutReceipt.ClampFees).ToString("N2") + "\x1B\x21\x00\n";
                        receiptlines[7] = "\x1B\x21\x01" + "Parked at - Bays:" + objCheckOutReceipt.LocationParkingLotID.ParkingBayID.ParkingBayRange + "\x1B\x21\x00\n";
                        receiptlines[8] = "\x1B\x21\x01" + "OPERATOR ID -" + objCheckOutReceipt.UserCode + "\x1B\x21\x00\n";
                        receiptlines[9] = "\x1B\x21\x01" + "Security available " + objCheckOutReceipt.LocationParkingLotID.LotTimmings + "\x1B\x21\x00\n";
                        receiptlines[10] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,      wallet,helmet etc." + "\x1B\x21\x00\n";
                        receiptlines[11] = "\x1B\x21\x01" + "GST Number 0012" + "\x1B\x21\x00\n";
                        receiptlines[12] = "\x1B\x21\x01" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                        receiptlines[13] = "" + "\n";
                        receiptlines[14] = "" + "\n";
                        receiptlines[15] = "" + "\n";
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void BtnPrint_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                string printerName = string.Empty;
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

                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
            }
        }
        private async void BtnDone_Clicked(object sender, EventArgs e)
        {

            try
            {
                ShowLoading(true);
                BtnDone.IsVisible = false;
                MasterHomePage masterPage = null;
                await Task.Run(() =>
                    {
                        masterPage = new MasterHomePage();
                    });
                await Navigation.PushAsync(masterPage);
                ShowLoading(false);
                BtnDone.IsVisible = true;
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                BtnDone.IsVisible = true;
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutReceiptpage.Opacity = 0.5;
            }
            else
            {
                absLayoutReceiptpage.Opacity = 1;
            }

        }
        private void LoadParkingVehicleDetails(string Vehicle, string CheckInID, string VehicleInformation)
        {
            try
            {
                ParkingVechiles objParkingVechiles = new ParkingVechiles();
                VMVehicleParking objVMVehicleParking = objParkingVechiles.GetParkingVehicleDetails(null, null);
                labelParkingLocation.Text = objVMVehicleParking.Location + ", " + objVMVehicleParking.BayNumber;
                labelValidFrom.Text = objVMVehicleParking.ParkingFromTime;
                labelValidTo.Text = objVMVehicleParking.ParkingToTime;

                imageVehicleImage.Source = objVMVehicleParking.VehicleImage;
                labelVehicleDetails.Text = objVMVehicleParking.VehicleNumber;
                if (objVMVehicleParking.PaymentType == "Cash")
                {
                    imageParkingFeeImage.Source = "rupee_black.png";
                }
                labelParkingFeesDetails.Text = objVMVehicleParking.ParkingFees + "/-";


                if (VehicleInformation == "ViolationVehicleInformation")
                {
                    labelParkingPaymentType.Text = "Paid for 13hr - " + "By " + objVMVehicleParking.PaymentType;
                    labelCheckOutFeesDetails.Text = "(Parking" + " ₹" + "40" + " + " + "Clamp" + " ₹" + "50)";
                }
                else if (VehicleInformation == "OverstayVehicleInformation")
                {
                    labelParkingPaymentType.Text = "Paid for 3hr - " + "By " + objVMVehicleParking.PaymentType;
                    labelCheckOutFeesDetails.Text = "";
                    labelCheckOutFeesDetails.IsVisible = false;
                }

                imageOperatorProfile.Source = "operator.png";
                labelOperatorName.Text = "Rambabu";
                labelOperatorID.Text = " - #123456";
            }
            catch (Exception ex)
            {

            }
        }


    }
}