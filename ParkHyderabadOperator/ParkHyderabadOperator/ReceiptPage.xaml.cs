using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptPage : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[16]; // Receipt Lines
        DALExceptionManagment dal_Exceptionlog;
        public ReceiptPage()
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
        }
        public ReceiptPage(CustomerParkingSlot objReceipt)
        {
            string vehicleTypeName = string.Empty;
            dal_Exceptionlog = new DALExceptionManagment();
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadReceiptDetails(objReceipt);
        }
        public void LoadReceiptDetails(CustomerParkingSlot objCheckInReceipt)
        {
            try
            {
                string vehicleType = string.Empty;
                labelParkingLocation.Text = objCheckInReceipt.LocationParkingLotID.LocationID.LocationName + "-" + objCheckInReceipt.LocationParkingLotID.LocationParkingLotName + "," + objCheckInReceipt.LocationParkingLotID.ParkingBayID.ParkingBayName + " " + objCheckInReceipt.LocationParkingLotID.ParkingBayID.ParkingBayRange;
                labelValidFrom.Text = objCheckInReceipt.ActualStartTime == null ? "" : Convert.ToDateTime(objCheckInReceipt.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt");
                labelValidTo.Text = objCheckInReceipt.ActualEndTime == null ? "" : Convert.ToDateTime(objCheckInReceipt.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt");
                if (objCheckInReceipt.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    vehicleType = "BIKE";
                    imageVehicleImage.Source = ImageSource.FromFile("bike_black.jpg");
                }
                else if (objCheckInReceipt.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    vehicleType = "CAR";
                    imageVehicleImage.Source = ImageSource.FromFile("car_black.jpg");
                }
                labelRegistration.Text = objCheckInReceipt.CustomerVehicleID.RegistrationNumber;
                imageParkingFeeImage.Source = "rupee_black.png";

                TimeSpan t = Convert.ToDateTime(objCheckInReceipt.ActualEndTime) - Convert.ToDateTime(objCheckInReceipt.ActualStartTime);
                int totalHours = (t.Days > 0 ? t.Hours + (t.Days * 24) : t.Hours);
                string duration = "Paid for " + string.Format(totalHours + "hr");
                decimal totalParkingAmount = objCheckInReceipt.PaidAmount;// + objCheckInReceipt.ExtendAmount;
                labelParkingFeesDetails.Text = totalParkingAmount.ToString("N2") + "/-" + duration;
                labelParkingPaymentType.Text = "- By " + objCheckInReceipt.PaymentTypeID.PaymentTypeName;
                imageOperatorProfile.Source = "operator.png";
                labelOperatorName.Text = objCheckInReceipt.CreatedByName;
                labelOperatorID.Text = "- #" + objCheckInReceipt.UserCode;
                objCheckInReceipt.LocationParkingLotID.LotTimmings = "6am - 10pm    (Mon-Sat) and 12pm - 8pm (Sun)";
                try
                {
                    if (receiptlines != null && receiptlines.Length > 0)
                    {

                        receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                        receiptlines[1] = "\x1B\x21\x01" + "       " + objCheckInReceipt.LocationParkingLotID.LocationID.LocationName + "-" + objCheckInReceipt.LocationParkingLotID.LocationParkingLotName + "\n";
                        receiptlines[2] = " " + "\n";
                        receiptlines[3] = "\x1B\x21\x08" + vehicleType + "     " + objCheckInReceipt.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00" + "\n";
                        receiptlines[4] = "\x1B\x21\x08" + objCheckInReceipt.ActualStartTime == null ? "" : Convert.ToDateTime(objCheckInReceipt.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt") + "\x1B\x21\x00\n";
                        receiptlines[5] = "" + "\n";
                        receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + objCheckInReceipt.Amount.ToString("N2") + "\x1B\x21\x00\n";
                        receiptlines[7] = "\x1B\x21\x01" + "Parked at - Bays:" + objCheckInReceipt.LocationParkingLotID.ParkingBayID.ParkingBayRange + "\x1B\x21\x00\n";
                        receiptlines[8] = "\x1B\x21\x01" + "OPERATOR ID -" + objCheckInReceipt.UserCode + "\x1B\x21\x00\n";
                        receiptlines[9] = "\x1B\x21\x01" + "Security available " + objCheckInReceipt.LocationParkingLotID.LotTimmings + "\x1B\x21\x00\n";
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReceiptPage.xaml.cs", "", "LoadReceiptDetails");
            }
        }
        private async void BtnPrint_Clicked(object sender, EventArgs e)
        {
            BtnPrint.IsVisible = false;
            ShowLoading(true);
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
                        await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                        await Navigation.PushAsync(masterPage);
                    }
                    BtnPrint.IsVisible = true;
                    ShowLoading(false);
                }
                catch (Exception ex)
                {
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReceiptPage.xaml.cs", "", "BtnPrint_Clicked");
                }
            }
            catch (Exception ex)
            {
                BtnPrint.IsVisible = true;
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReceiptPage.xaml.cs", "", "BtnPrint_Clicked");
            }
            ShowLoading(false);
            BtnPrint.IsVisible = false;
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
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}