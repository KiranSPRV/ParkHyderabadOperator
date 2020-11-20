using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassPaymentReceiptPage : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        DALExceptionManagment dal_Exceptionlog;
        string[] receiptlines = new string[17]; // Receipt Lines
        public PassPaymentReceiptPage()
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            dal_Exceptionlog = new DALExceptionManagment();
        }
        public PassPaymentReceiptPage(string generatedPassType, string StationType)
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            dal_Exceptionlog = new DALExceptionManagment();

        }
        public PassPaymentReceiptPage(CustomerVehiclePass objReceiptDetails)
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            dal_Exceptionlog = new DALExceptionManagment();
            LoadCustomerPassPaymentDetails(objReceiptDetails);
        }
        public void LoadCustomerPassPaymentDetails(CustomerVehiclePass objReceipt)
        {
            try
            {
                string vehicleType = string.Empty;
                string stations = string.Empty;
                labelParkingReceiptTitle.Text = "InstaParking-" + objReceipt.PassPriceID.PassTypeID.PassTypeName;

                if (objReceipt.PassPriceID.PassTypeID.PassTypeCode == "DP")
                {
                    labelParkingLot.Text = objReceipt.LocationID.LocationName + "-" + objReceipt.PassPriceID.StationAccess;
                    labelValidFrom.Text = Convert.ToDateTime(objReceipt.StartDate).ToString("dd MMM yyyy hh:mm tt");
                    labelValidTo.Text = Convert.ToDateTime(objReceipt.ExpiryDate).ToString("dd MMM yyyy hh:mm tt");

                }
                else if (objReceipt.PassPriceID.PassTypeID.PassTypeCode == "EP")
                {
                    labelParkingLot.Text = objReceipt.LocationID.LocationName + "-" + objReceipt.PassPriceID.StationAccess;
                    labelValidFrom.Text = Convert.ToDateTime(objReceipt.StartDate).ToString("dd MMM yyyy");
                    labelValidTo.Text = Convert.ToDateTime(objReceipt.ExpiryDate).ToString("dd MMM yyyy");

                }
                else if (objReceipt.PassPriceID.PassTypeID.PassTypeCode == "WP")
                {
                    labelParkingLot.Text = objReceipt.LocationID.LocationName + "-" + objReceipt.PassPriceID.StationAccess;
                    labelValidFrom.Text = Convert.ToDateTime(objReceipt.StartDate).ToString("dd MMM yyyy");
                    labelValidTo.Text = Convert.ToDateTime(objReceipt.ExpiryDate).ToString("dd MMM yyyy");
                }
                else if (objReceipt.PassPriceID.PassTypeID.PassTypeCode == "MP")
                {
                    if (objReceipt.PassPriceID.StationAccess == "Single Station")
                    {
                        labelParkingLot.Text = objReceipt.LocationID.LocationName + "-" + objReceipt.PassPriceID.StationAccess;
                        stations = objReceipt.LocationID.LocationName;
                    }
                    else if (objReceipt.PassPriceID.StationAccess == "Multi Station" || objReceipt.PassPriceID.StationAccess == "Multi Stations")
                    {

                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            var lstLocation = (List<Model.APIOutPutModel.Location>)App.Current.Properties["MultiSelectionLocations"];
                            if (lstLocation.Count > 0)
                            {
                               
                                for (var s = 0; s < lstLocation.Count; s++)
                                {
                                    stations = stations + lstLocation[s].LocationName + ",";
                                }
                                labelParkingLot.Text = "Multi Stations:" + stations + ".";
                            }

                        }
                    }
                    else if (objReceipt.PassPriceID.StationAccess == "All Station" || objReceipt.PassPriceID.StationAccess == "All Stations")
                    {
                        labelParkingLot.Text = "All Metro Stations";
                        stations = "All Metro Stations";
                    }
                    labelValidFrom.Text = Convert.ToDateTime(objReceipt.StartDate).ToString("dd MMM yyyy");
                    labelValidTo.Text = Convert.ToDateTime(objReceipt.ExpiryDate).ToString("dd MMM yyyy");

                }
                if (objReceipt.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    vehicleType = "BIKE";
                    imageVehicleImage.Source = "bike_black.png";
                }
                if (objReceipt.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    vehicleType = "CAR";
                    imageVehicleImage.Source = "car_black.png";
                }

                labelCustomerName.Text = objReceipt.CustomerVehicleID.CustomerID.Name;
                labelVehicleDetails.Text = objReceipt.CustomerVehicleID.RegistrationNumber;

                if (objReceipt.IssuedCard)
                {
                    labelParkingFeesDetails.Text = objReceipt.TotalAmount.ToString("N2") + "/-";
                    labelParkingPaymentType.Text = "Paid (Including NFC) - By " + objReceipt.PaymentTypeID.PaymentTypeName;
                }
                else
                {
                    labelParkingFeesDetails.Text = objReceipt.Amount.ToString("N2") + "/-";
                    labelParkingPaymentType.Text = "Paid - By " + objReceipt.PaymentTypeID.PaymentTypeName;
                }

                if (objReceipt.CreatedBy.UserName != "")
                {
                    labelOperatorName.Text = objReceipt.CreatedBy.UserName;
                    labelOperatorID.Text = "- #" + Convert.ToString(objReceipt.CreatedBy.UserCode);
                    labelOrderID.Text = "#" + objReceipt.CustomerVehiclePassID;
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
                        receiptlines[1] = "\x1B\x21\x01" + "          " + objReceipt.CreatedBy.LocationParkingLotID.LocationID.LocationName + "\x1B\x21\x00\n";
                        receiptlines[3] = "" + "\n";
                        receiptlines[4] = "\x1B\x21\x08" + vehicleType + "     " + objReceipt.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                        receiptlines[5] = "\x1B\x21\x01" + "Valid From:" + Convert.ToDateTime(objReceipt.StartDate).ToString("dd MMM yyyy") + "\x1B\x21\x00" + "\n";
                        receiptlines[6] = "\x1B\x21\x01" + "Valid Till:" + Convert.ToDateTime(objReceipt.ExpiryDate).ToString("dd MMM yyyy") + "\x1B\x21\x00" + "\n";
                        receiptlines[7] = "\x1B\x21\x01" + "(Pass Type :" + objReceipt.PassPriceID.PassTypeID.PassTypeName + ")" + "\x1B\x21\x00\n";
                        receiptlines[8] = "\x1B\x21\x01" + "Station(s):" + stations + "\x1B\x21\x01" + "\n";
                        receiptlines[9] = "\x1B\x21\x01" + "Paid: Rs" + (objReceipt.IssuedCard ? objReceipt.TotalAmount.ToString("N2")+"(NFC Rs"+ objReceipt.PassPriceID.NFCCardPrice.ToString("N2") +")": objReceipt.Amount.ToString("N2")) + "\x1B\x21\x01" + "\n";
                        receiptlines[10] = "\x1B\x21\x06" + "Operator Id:" + objReceipt.CreatedBy.UserCode + "\x1B\x21\x00\n";
                        receiptlines[11] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objReceipt.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                        receiptlines[12] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                        receiptlines[13] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                        receiptlines[14] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                        receiptlines[15] = "" + "\n";
                        receiptlines[16] = "" + "\n";

                    }
                }
                catch (Exception ex)
                {
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassPaymentReceiptPage.xaml.cs", "", "receiptlines");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassPaymentReceiptPage.xaml.cs", "", "LoadCustomerPassPaymentDetails");
            }
        }
        private async void BtnDone_Clicked(object sender, EventArgs e)
        {
            try
            {
                BtnDone.IsVisible = false;
                ShowLoading(true);
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassPaymentReceiptPage.xaml.cs", "", "BtnDone_Clicked");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutPassReceiptpage.Opacity = 0.5;
            }
            else
            {
                absLayoutPassReceiptpage.Opacity = 1;
            }

        }
        private async void BtnPrint_Clicked(object sender, EventArgs e)
        {
            try
            {
                string printerName = string.Empty;
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                {
                    App.Current.Properties.Remove("MultiSelectionLocations");
                }
                try
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
                            var masterPage = new MasterHomePage();
                            await Navigation.PushAsync(masterPage);

                        }
                    }
                    else
                    {
                        var masterPage = new MasterHomePage();
                        await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                        await Navigation.PushAsync(masterPage);
                    }

                }
                catch (Exception ex)
                {

                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "PassPaymentReceiptPage.xaml.cs", "", "BtnDone_Clicked");
            }
        }
    }
}