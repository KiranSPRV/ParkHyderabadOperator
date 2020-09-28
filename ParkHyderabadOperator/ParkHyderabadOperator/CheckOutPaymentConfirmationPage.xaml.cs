using ParkHyderabadOperator.DAL.DALCheckOut;
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
    public partial class CheckOutPaymentConfirmationPage : ContentPage
    {
        string PageCalledBy = string.Empty;
        CustomerParkingSlot objviolationVehicleChekOut;
        DALExceptionManagment dal_Exceptionlog;
        BlueToothDevicePrinting ObjblueToothDevicePrinting = null;
        string[] receiptlines = new string[16]; // Receipt Lines
        public CheckOutPaymentConfirmationPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            
        }
        public CheckOutPaymentConfirmationPage(string redirectdFrom, CustomerParkingSlot objInput)
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            PageCalledBy = redirectdFrom;
            objviolationVehicleChekOut = objInput;
            if (objInput.VehicleTypeID.VehicleTypeCode == "2W")
            {
                labelChekInAmount.Text = "(Two Wheeler - For " + objInput.Duration + " Hours)";
                ImgVehicleType.Source = "bike_black.png";
            }
            else if (objInput.VehicleTypeID.VehicleTypeCode == "4W")
            {
                labelChekInAmount.Text = "(Four Wheeler - For " + objInput.Duration + " Hours)";
                ImgVehicleType.Source = "car_black.png";
            }
            labelVehicleRegNumber.Text = objInput.CustomerVehicleID.RegistrationNumber;
            labelParkingLocation.Text = objInput.LocationParkingLotID.LocationID.LocationName + "-" + objInput.LocationParkingLotID.LocationParkingLotName + "," + objInput.LocationParkingLotID.ParkingBayID.ParkingBayName + " " + objInput.LocationParkingLotID.ParkingBayID.ParkingBayRange;

            if (redirectdFrom == "ViolationVehicleInformation")
            {
                labelChekInAmount.Text = "(Parking ₹" + objInput.Amount.ToString("N2") + " Clamp ₹" + objInput.ClampFees.ToString("N2") + ")";
                lableAmount.Text = (objInput.Amount + objInput.ClampFees).ToString("N2");
            }
            else if (redirectdFrom == "PassCheckInVehicleInformation")
            {
                labelChekInAmount.Text = "( Clamp ₹" + objInput.ClampFees.ToString("N2") + ")";
                lableAmount.Text = (objInput.ClampFees).ToString("N2");
            }
            else if (redirectdFrom == "OverstayVehicleInformation")
            {

                lableAmount.Text = (objInput.ExtendAmount + objInput.ClampFees).ToString("N2");
                labelChekInAmount.Text = "(Two Wheeler - For " + objInput.Duration + " Hours)";
            }
        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                if (PageCalledBy == "ViolationVehicleInformation")
                { await Navigation.PushAsync(new ViolationVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                else if (PageCalledBy == "OverstayVehicleInformation")
                { await Navigation.PushAsync(new OverstayVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                else if (PageCalledBy == "PassCheckInVehicleInformation")
                { await Navigation.PushAsync(new PassCheckInVehicleInformation(objviolationVehicleChekOut.CustomerParkingSlotID)); }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckOutPaymentConfirmationPage.xaml.cs", "", "BtnNo_Clicked");
                ShowLoading(false);
            }
        }
        private async void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                btnYes.IsEnabled = false;
                btnNo.IsVisible = false;
                DALVehicleCheckOut dal_CheckOut = null;
                MasterHomePage masterPage = null;
                CustomerParkingSlot objCheckOutReceipt = null;
                if (DeviceInternet.InternetConnected())
                {
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        dal_CheckOut = new DALVehicleCheckOut();
                        string printerName = string.Empty;
                        await Task.Run(() =>
                        {
                            User objCheckoutBy = (User)App.Current.Properties["LoginUser"];
                            objviolationVehicleChekOut.CreatedBy = objCheckoutBy.UserID;
                            objCheckOutReceipt = dal_CheckOut.VehicleCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), objviolationVehicleChekOut);
                            printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();
                            if (printerName != string.Empty && printerName != "")
                            {
                                if (receiptlines != null && receiptlines.Length > 0)
                                {
                                    string vehicleType = objCheckOutReceipt.VehicleTypeID.VehicleTypeCode == "2W" ? "BIKE" : (objCheckOutReceipt.VehicleTypeID.VehicleTypeCode == "4W" ? "CAR" : objCheckOutReceipt.VehicleTypeID.VehicleTypeCode);
                                    receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                                    receiptlines[1] = "\x1B\x21\x01" + "       " + objCheckOutReceipt.LocationParkingLotID.LocationID.LocationName + "-" + objCheckOutReceipt.LocationParkingLotID.LocationParkingLotName + "\n";
                                    receiptlines[2] = " " + "\n";
                                    receiptlines[3] = "\x1B\x21\x08" + vehicleType + "     " + objCheckOutReceipt.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00" + "\n";
                                    receiptlines[4] = "\x1B\x21\x08" + objCheckOutReceipt.ActualEndTime == null ? "" : Convert.ToDateTime(objCheckOutReceipt.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt") + "\x1B\x21\x00\n";
                                    receiptlines[5] = "" + "\n";
                                    receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + (objviolationVehicleChekOut.ExtendAmount + objviolationVehicleChekOut.ClampFees + objviolationVehicleChekOut.ViolationFees).ToString("N2") + "\x1B\x21\x00\n";
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
                        if (objCheckOutReceipt != null && objCheckOutReceipt.CustomerParkingSlotID != 0)
                        {
                            if (printerName != string.Empty && printerName != "")
                            {
                                await Navigation.PushAsync(masterPage);
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                                await Navigation.PushAsync(masterPage);
                            }
                        }
                        else
                        {

                            await DisplayAlert("Alert", "Check-In Fail,Please contact admin.", "Ok");
                            await Navigation.PushAsync(masterPage);
                        }
                        
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }
                ShowLoading(false);
                btnYes.IsEnabled = true;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckOutPaymentConfirmationPage.xaml.cs", "", "BtnCheckOut_Clicked");
                ShowLoading(false);
                btnYes.IsEnabled = true;
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutConfirmationpage.Opacity = 0.5;
            }
            else
            {
                absLayoutConfirmationpage.Opacity = 1;
            }

        }
    }
}