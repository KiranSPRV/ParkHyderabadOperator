using ParkHyderabadOperator.DAL;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutReceiptPage : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[17]; // Receipt Lines
        string VehicleInformation = string.Empty;

        public CheckOutReceiptPage()
        {
            InitializeComponent();

            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            
        }
        public CheckOutReceiptPage(string vehicleInformation)
        {
            InitializeComponent();
            VehicleInformation = vehicleInformation;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
           
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
                vehicleType = objCheckOutReceipt.VehicleTypeID.VehicleTypeDisplayName;
                imageVehicleImage.Source = objCheckOutReceipt.VehicleTypeID.VehicleIcon;
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


                        receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                        receiptlines[1] = "\x1B\x21\x01" + "       " + objCheckOutReceipt.LocationParkingLotID.LocationID.LocationName + "-" + objCheckOutReceipt.LocationParkingLotID.LocationParkingLotName + "\x1B\x21\x00\n";
                        receiptlines[2] = "" + "\n";
                        receiptlines[3] = "\x1B\x21\x08" + vehicleType + ":" + objCheckOutReceipt.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                        receiptlines[4] = "\x1B\x21\x01" + "In :" + (objCheckOutReceipt.ActualStartTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                        receiptlines[5] = "\x1B\x21\x01" + "Out:" + (objCheckOutReceipt.ActualEndTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                        receiptlines[6] = "\x1B\x21\x01" + "Paid Amount: Rs" + ((objCheckOutReceipt.ExtendAmount + objCheckOutReceipt.ViolationFees + objCheckOutReceipt.ClampFees)).ToString("N2") + "\x1B\x21\x01" + "\n";
                        receiptlines[7] = "\x1B\x21\x01" + "(Includes Violation)" + "\x1B\x21\x01" + "\n";
                        receiptlines[8] = "\x1B\x21\x01" + "Violation Fee:" + "Rs" + (objCheckOutReceipt.ClampFees).ToString("N2") + "\x1B\x21\x01" + "\n";
                        receiptlines[9] = "\x1B\x21\x06" + "Operator Id:" + objCheckOutReceipt.UserCode + "\x1B\x21\x00\n";
                        receiptlines[10] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objCheckOutReceipt.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                        receiptlines[11] = "\x1B\x21\x06" + "Security available " + objCheckOutReceipt.LocationParkingLotID.LotOpenTime + "-" + objCheckOutReceipt.LocationParkingLotID.LotCloseTime + "\x1B\x21\x00\n";
                        receiptlines[12] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                        receiptlines[13] = "\x1B\x21\x06" + "GST Number"+ objCheckOutReceipt.GSTNumber + "\x1B\x21\x00\n";
                        receiptlines[14] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                        receiptlines[15] = "" + "\n";
                        receiptlines[16] = "" + "\n";

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
                MasterHomePage masterPage = null;
                printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();
                await Task.Run(() =>
                {
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
                    masterPage = new MasterHomePage();
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
      

    }
}