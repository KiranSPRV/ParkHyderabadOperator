using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmationPage : ContentPage
    {
        VehicleCheckIn objNewCheckIn;
        DALCheckIn dal_DALCheckIn = null;
        DALExceptionManagment dal_Exceptionlog = null;
        BlueToothDevicePrinting ObjblueToothDevicePrinting = null;
        string[] receiptlines = new string[16]; // Receipt Lines
        public ConfirmationPage()
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();

        }
        public ConfirmationPage(VehicleCheckIn obj)
        {
            InitializeComponent();
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            LoadVehicleChekInDetails(obj);


        }
        public async void LoadVehicleChekInDetails(VehicleCheckIn obj)
        {
            string vehicleTypeName = string.Empty;
            try
            {
                if (obj != null)
                {
                    objNewCheckIn = obj;
                    if (obj.VehicleTypeCode == "2W")
                    {
                        vehicleTypeName = "Two Wheeler";
                        ImgVehicleType.Source = ImageSource.FromFile("bike_black.jpg");
                    }
                    else if (obj.VehicleTypeCode == "4W")
                    {
                        vehicleTypeName = "Four Wheeler";
                        ImgVehicleType.Source = ImageSource.FromFile("car_black.jpg");
                    }
                    labelVehicleRegNumber.Text = obj.RegistrationNumber;
                    labelParkingLocation.Text = obj.LocationName + "-" + obj.LocationParkingLotName + "," + obj.BayNumber + "-" + obj.BayRange;
                    labelChekInAmount.Text = Convert.ToDecimal(obj.ParkingFees + obj.ClampFees).ToString("N2");
                    labelClampAmount.Text = "( Clamp Fees " + obj.ClampFees.ToString("N2") + " )";
                    if (obj.ClampFees > 0)
                    {
                        labelChekInAmountDetails.Text = "( " + vehicleTypeName + " - For " + obj.ParkingHours + " hrs )";
                    }
                    else
                    {
                        labelChekInAmountDetails.Text = "( " + vehicleTypeName + " - For " + obj.ParkingHours + " hrs )";
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "CheckIn vehicle details are not avialable.", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "LoadVehicleChekInDetails");
            }
        }
      
        private async void BtnYes_Clicked(object sender, EventArgs e)
        {

            ShowLoading(true);
            try
            {
                string printerName = string.Empty;
                MasterHomePage masterPage = null;
                btnYes.IsVisible = false;
                CustomerParkingSlot objResultCustomerParkingSlot = null;
                try
                {
                    if (DeviceInternet.InternetConnected())
                    {
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken") && objNewCheckIn != null)
                        {
                            await Task.Run(() =>
                            {
                                objResultCustomerParkingSlot = dal_DALCheckIn.SaveVehicleNewCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objNewCheckIn);
                                printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();
                                if (printerName != string.Empty && printerName != "")
                                {
                                    if (receiptlines != null && receiptlines.Length > 0)
                                    {
                                        string vehicleType = objResultCustomerParkingSlot.VehicleTypeID.VehicleTypeCode == "2W" ? "BIKE" : (objResultCustomerParkingSlot.VehicleTypeID.VehicleTypeCode == "4W" ? "CAR" : objResultCustomerParkingSlot.VehicleTypeID.VehicleTypeCode);
                                        receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                                        receiptlines[1] = "\x1B\x21\x01" + "       " + objResultCustomerParkingSlot.LocationParkingLotID.LocationID.LocationName + "-" + objResultCustomerParkingSlot.LocationParkingLotID.LocationParkingLotName + "\n";
                                        receiptlines[2] = " " + "\n";
                                        receiptlines[3] = "\x1B\x21\x08" + vehicleType + "     " + objResultCustomerParkingSlot.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00" + "\n";
                                        receiptlines[4] = "\x1B\x21\x08" + objResultCustomerParkingSlot.ActualStartTime == null ? "" : Convert.ToDateTime(objResultCustomerParkingSlot.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt") + "\x1B\x21\x00\n";
                                        receiptlines[5] = "" + "\n";
                                        receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + objResultCustomerParkingSlot.Amount.ToString("N2") + "\x1B\x21\x00\n";
                                        receiptlines[7] = "\x1B\x21\x01" + "Parked at - Bays:" + objResultCustomerParkingSlot.LocationParkingLotID.ParkingBayID.ParkingBayRange + "\x1B\x21\x00\n";
                                        receiptlines[8] = "\x1B\x21\x01" + "OPERATOR ID -" + objResultCustomerParkingSlot.UserCode + "\x1B\x21\x00\n";
                                        receiptlines[9] = "\x1B\x21\x01" + "Security available " + objResultCustomerParkingSlot.LocationParkingLotID.LotTimmings + "\x1B\x21\x00\n";
                                        receiptlines[10] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,      wallet,helmet etc." + "\x1B\x21\x00\n";
                                        receiptlines[11] = "\x1B\x21\x01" + "GST Number 0012" + "\x1B\x21\x00\n";
                                        receiptlines[12] = "\x1B\x21\x01" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                                        receiptlines[13] = "" + "\n";
                                        receiptlines[14] = "" + "\n";
                                        receiptlines[15] = "" + "\n";
                                    }
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
                                else
                                {
                                    masterPage = new MasterHomePage();
                                }
                            });
                            if (objResultCustomerParkingSlot != null && objResultCustomerParkingSlot.CustomerParkingSlotID != 0)
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
                            ShowLoading(false);
                        }
                    }
                    else
                    {

                        await DisplayAlert("Alert", "Please check your internet.", "Ok");
                        ShowLoading(false);
                    }
                    btnYes.IsVisible = true;
                }
                catch (Exception ex)
                {
                    ShowLoading(false);
                    btnYes.IsVisible = true;
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReceiptPage.xaml.cs", "", "BtnPrint_Clicked");
                }
            }
            catch (Exception ex)
            {

                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReceiptPage.xaml.cs", "", "BtnPrint_Clicked");
            }
            ShowLoading(false);

        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new CheckIn());
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnNo_Clicked");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutCheckInConfirmationPage.Opacity = 0.5;
            }
            else
            {
                absLayoutCheckInConfirmationPage.Opacity = 1;
            }

        }
       

    }
}