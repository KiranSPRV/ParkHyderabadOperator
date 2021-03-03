using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.IO;
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
        string printerName = string.Empty;
        bool IsbtnClicked = false;
        CustomerParkingSlot objResultCustomerParkingSlot = null;
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
            printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();

        }
        public async void LoadVehicleChekInDetails(VehicleCheckIn obj)
        {
            string vehicleTypeName = string.Empty;
            try
            {
                if (obj != null)
                {
                    objNewCheckIn = obj;
                    ImgVehicleType.Source =  obj.VehicleImage;
                    vehicleTypeName = obj.VehicleTypeName;
                    labelVehicleRegNumber.Text = obj.RegistrationNumber;
                    labelParkingLocation.Text = obj.LocationName + "-" + obj.LocationParkingLotName + "," + "Bay Number " + "-" + obj.BayRange;
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
                    await DisplayAlert("Alert", "Check In vehicle details unavailable.", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "LoadVehicleChekInDetails");
            }
        }
        private async void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (IsbtnClicked)
                    return;
                IsbtnClicked = true;
                btnYes.IsVisible = false;
                CheckIn checkInPage = null;
                try
                {
                    ShowLoading(true);
                    if (DeviceInternet.InternetConnected())
                    {
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken") && objNewCheckIn != null)
                        {
                            await Task.Run(() =>
                            {
                                objResultCustomerParkingSlot = dal_DALCheckIn.SaveVehicleNewCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objNewCheckIn);
                                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", "Save New CheckIn : " + objNewCheckIn.RegistrationNumber + "-At" + DateTime.Now, "ConfirmationPage.xaml.cs", "", "BtnYes_Clicked");
                                checkInPage = new CheckIn();
                            });
                            if (objResultCustomerParkingSlot.CustomerParkingSlotID != 0)
                            {
                                if (printerName != string.Empty && printerName != "")
                                {
                                    PrintReceipt();
                                    await Navigation.PushAsync(checkInPage);
                                }
                                else
                                {

                                    await DisplayAlert("Alert", "Unable to find Bluetooth device", "Ok");
                                    await Navigation.PushAsync(checkInPage);
                                    ShowLoading(false);
                                    btnYes.IsVisible = true;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Check-In Failed,Please contact Admin.", "Ok");
                                await Navigation.PushAsync(checkInPage);
                                ShowLoading(false);
                                btnYes.IsVisible = true;
                            }
                        }
                    }
                    else
                    {
                        await Task.Run(() =>
                        {
                            App.SQLiteDb.SaveOfflineCheckInAsync(objNewCheckIn).Wait();
                            checkInPage = new CheckIn();
                        });
                        if (printerName != string.Empty && printerName != "")
                        {
                            PrintReceipt();
                            await Navigation.PushAsync(checkInPage);
                        }
                        else
                        {

                            await DisplayAlert("Alert", "Unable to find Bluetooth device", "Ok");
                            await Navigation.PushAsync(checkInPage);
                            ShowLoading(false);
                            btnYes.IsVisible = true;
                        }
                        ShowLoading(false);
                        btnYes.IsVisible = true;
                    }

                }
                catch (Exception ex)
                {
                    ShowLoading(false);
                    btnYes.IsVisible = true;
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnPrint_Clicked");
                }
                IsbtnClicked = false;
            }
            catch (Exception ex)
            {

                ShowLoading(false);
                btnYes.IsVisible = true;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnPrint_Clicked");
            }
        }
        public async void PrintReceipt()
        {
            try
            {
                string[] receiptlines = new string[17]; // Receipt Lines

                await Task.Run(() =>
                {
                    if (receiptlines != null && receiptlines.Length > 0)
                    {
                        if (DeviceInternet.InternetConnected())
                        {
                            string vehicleType = objResultCustomerParkingSlot.VehicleTypeID.VehicleTypeDisplayName;
                            receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                            receiptlines[1] = "\x1B\x21\x01" + "       " + objResultCustomerParkingSlot.LocationParkingLotID.LocationID.LocationName + "-" + objResultCustomerParkingSlot.LocationParkingLotID.LocationParkingLotName + "\x1B\x21\x00\n";
                            receiptlines[2] = "" + "\n";
                            receiptlines[3] = "\x1B\x21\x08" + vehicleType + ":" + objResultCustomerParkingSlot.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                            receiptlines[4] = "\x1B\x21\x01" + (objResultCustomerParkingSlot.ActualStartTime == null ? "" : "In:" + Convert.ToDateTime(objResultCustomerParkingSlot.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                            receiptlines[5] = "\x1B\x21\x01" + "Paid: Rs" + objResultCustomerParkingSlot.Amount.ToString("N2") + "(Up to " + objResultCustomerParkingSlot.Duration + " hours)" + "\x1B\x21\x00\n";
                            receiptlines[6] = "\x1B\x21\x01" + "Valid Till:" + (objResultCustomerParkingSlot.ActualEndTime == null ? "" : Convert.ToDateTime(objResultCustomerParkingSlot.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00\n";
                            receiptlines[7] = "\x1B\x21\x01" + "Parked at: (Bays)" + objResultCustomerParkingSlot.LocationParkingLotID.ParkingBayID.ParkingBayRange + "\x1B\x21\x00\n";
                            receiptlines[8] = "\x1B\x21\x06" + "OperatorId :" + objResultCustomerParkingSlot.UserCode + "\x1B\x21\x00\n";
                            receiptlines[9] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objResultCustomerParkingSlot.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                            receiptlines[10] = "\x1B\x21\x06" + "Security available " + objResultCustomerParkingSlot.LocationParkingLotID.LotOpenTime + "-" + objResultCustomerParkingSlot.LocationParkingLotID.LotCloseTime + "\x1B\x21\x00\n";
                            receiptlines[11] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                            receiptlines[12] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                            receiptlines[13] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                            receiptlines[14] = "" + "\n";
                            receiptlines[15] = "" + "\n";
                            receiptlines[16] = "" + "\n";
                        }
                        else
                        {
                            var localUser = (User)App.Current.Properties["LoginUser"];
                            string vehicleType = objNewCheckIn.VehicleTypeDisplayName;
                            receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                            receiptlines[1] = "\x1B\x21\x01" + "       " + objNewCheckIn.LocationName + "-" + objNewCheckIn.LocationParkingLotName + "\x1B\x21\x00\n";
                            receiptlines[2] = "" + "\n";
                            receiptlines[3] = "\x1B\x21\x08" + vehicleType + ":" + objNewCheckIn.RegistrationNumber + "\x1B\x21\x00\n";
                            receiptlines[4] = "\x1B\x21\x01" + (objNewCheckIn.ParkingStartTime == null ? "" : "In:" + Convert.ToDateTime(objNewCheckIn.ParkingStartTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                            receiptlines[5] = "\x1B\x21\x01" + "Paid: Rs" + objNewCheckIn.ParkingFees.ToString("N2") + "(Up to " + objNewCheckIn.ParkingHours + " hours)" + "\x1B\x21\x00\n";
                            receiptlines[6] = "\x1B\x21\x01" + "Valid Till:" + (objNewCheckIn.ParkingEndTime == null ? "" : Convert.ToDateTime(objNewCheckIn.ParkingEndTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00\n";
                            receiptlines[7] = "\x1B\x21\x01" + "Parked at: (Bays)" + objNewCheckIn.BayRange + "\x1B\x21\x00\n";
                            receiptlines[8] = "\x1B\x21\x06" + "OperatorId :" + localUser.UserCode + "\x1B\x21\x00\n";
                            receiptlines[9] = "\x1B\x21\x01" + "(Supervisor Mobile:" + localUser.PhoneNumber + ")" + "\x1B\x21\x00\n";
                            receiptlines[10] = "\x1B\x21\x06" + "Security available" + localUser.LocationParkingLotID.LotOpenTime + "-" + localUser.LocationParkingLotID.LotCloseTime + "\x1B\x21\x00\n";
                            receiptlines[11] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                            receiptlines[12] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                            receiptlines[13] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                            receiptlines[14] = "" + "\n";
                            receiptlines[15] = "" + "\n";
                            receiptlines[16] = "" + "\n";
                        }
                    }
                    for (var l = 0; l < receiptlines.Length; l++)
                    {
                        string printtext = receiptlines[l];
                        if (printtext != "")
                        {
                            ObjblueToothDevicePrinting.PrintCommand(printerName, printtext);
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                btnYes.IsVisible = true;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ConfirmationPage.xaml.cs", "", "BtnPrint_Clicked");
            }
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