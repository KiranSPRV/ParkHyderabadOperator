using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        List<CustomerVehicle> lstVehicle;
        List<CustomerParkingSlot> lstVehicleHistory;
        DALMenubar dal_Menubar = null;
        DALExceptionManagment dal_Exceptionlog;
        bool IsRun = false;
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[17]; // Receipt Lines
        public HistoryPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Menubar = new DALMenubar();
            lstVehicle = new List<CustomerVehicle>();
            lstVehicleHistory = new List<CustomerParkingSlot>();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();

        }

        #region Search BAR
        public void ListVehicles(string RegistrationNumber)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstVehicle = dal_Menubar.GetAllVehicleRegistrationNumbersBySearch(Convert.ToString(App.Current.Properties["apitoken"]), RegistrationNumber);
                    listViewVehicleRegistrationNumbers.ItemsSource = lstVehicle;
                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "ListVehicles");
            }
        }
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            listViewVehicleRegistrationNumbers.IsVisible = true;
            listViewVehicleRegistrationNumbers.BeginRefresh();
            try
            {
                if (e.NewTextValue != null && e.NewTextValue != "")
                {
                    if (e.NewTextValue.Length >= 3)
                    {
                        ListVehicles(e.NewTextValue.ToLower());
                        if (lstVehicle.Count > 0)
                        {
                            var dataEmpty = lstVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                            {
                                listViewVehicleRegistrationNumbers.IsVisible = false;
                                LstVWParkingVehicle.ItemsSource = null;
                                searchBar.Text = "";
                            }
                            else
                                listViewVehicleRegistrationNumbers.ItemsSource = lstVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                        }
                    }
                    else
                    {
                        LstVWParkingVehicle.ItemsSource = null;
                        listViewVehicleRegistrationNumbers.IsVisible = false;
                       
                    }

                }
                else
                {
                    ImgSelectedVehicle.Source = null;
                    labelSelectedVehicleRegNumber.Text = "";
                    LstVWParkingVehicle.ItemsSource = null;
                    listViewVehicleRegistrationNumbers.IsVisible = false;

                }
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "SearchBar_OnTextChanged");

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            try
            {
                CustomerVehicle listsd = (CustomerVehicle)e.Item;
                searchBar.Text = listsd.RegistrationNumber;
                labelSelectedVehicleRegNumber.Text = listsd.RegistrationNumber;
                ImgSelectedVehicle.Source = listsd.VehicleTypeID.VehicleIcon;
                listViewVehicleRegistrationNumbers.IsVisible = false;
                ((ListView)sender).SelectedItem = null;
                LoadVehicleParkingHistory(listsd);

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "listViewVehicleRegistrationNumbers_OnItemTapped");
            }

        }
        #endregion

        #region History ListView
        public void LoadVehicleParkingHistory(CustomerVehicle objregistraionnumber)
        {
            try
            {
                CustomerParkingSlot objresult = null;
                string vehicleType = string.Empty;
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstVehicleHistory = dal_Menubar.GetVehicleParkingHistory(Convert.ToString(App.Current.Properties["apitoken"]), objregistraionnumber);
                    if (lstVehicleHistory.Count > 0)
                    {
                        LstVWParkingVehicle.ItemsSource = lstVehicleHistory;
                        objresult = lstVehicleHistory[0];
                    }

                    try
                    {
                        if (receiptlines != null && receiptlines.Length > 0)
                        {

                            vehicleType = Convert.ToString(objresult.VehicleTypeID.VehicleTypeDisplayName);

                            receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                            receiptlines[1] = "\x1B\x21\x01" + "       " + objresult.LocationParkingLotID.LocationID.LocationName + "-" + objresult.LocationParkingLotID.LocationParkingLotName + "\x1B\x21\x00\n";
                            receiptlines[2] = "" + "\n";
                            receiptlines[3] = "\x1B\x21\x08" + vehicleType + ":" + objresult.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                            receiptlines[4] = "\x1B\x21\x01" + "In :" + (objresult.ActualStartTime == null ? "" : Convert.ToDateTime(objresult.ActualStartTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                            receiptlines[5] = "\x1B\x21\x01" + "Out:" + (objresult.ActualEndTime == null ? "" : Convert.ToDateTime(objresult.ActualEndTime).ToString("dd MMM yyyy,hh:mm tt")) + "\x1B\x21\x00" + "\n";
                            receiptlines[6] = "\x1B\x21\x01" + "Paid Amount: Rs" + (objresult.PaidAmount).ToString("N2") + "\x1B\x21\x01" + "\n";
                            receiptlines[7] = "\x1B\x21\x01" + "(Includes Violation)" + "\x1B\x21\x01" + "\n";
                            receiptlines[8] = "\x1B\x21\x01" + "Violation Fee:" + "Rs" + (objresult.ClampFees).ToString("N2") + "\x1B\x21\x01" + "\n";
                            receiptlines[9] = "\x1B\x21\x06" + "Operator Id:" + objresult.UserCode + "\x1B\x21\x00\n";
                            receiptlines[10] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objresult.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                            receiptlines[11] = "\x1B\x21\x06" + "Security available " + objresult.LocationParkingLotID.LotOpenTime + "-" + objresult.LocationParkingLotID.LotCloseTime + "\x1B\x21\x00\n";
                            receiptlines[12] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                            receiptlines[13] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                            receiptlines[14] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                            receiptlines[15] = "" + "\n";
                            receiptlines[16] = "" + "\n";

                        }
                    }
                    catch (Exception ex)
                    {
                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "LoadVehicleParkingHistory (Printing Code)");
                    }

                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "LoadVehicleParkingHistory");
            }
        }

        #endregion

        private void SwitchViolation_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (switchViolation.IsToggled)
                {
                    if (lstVehicleHistory.Count >= 1)
                    {
                        var lstviolations = lstVehicleHistory.Where(i => (i.ApplicationTypeID.ApplicationTypeCode.ToUpper().Contains("V")) || (i.StatusID.StatusCode.ToUpper().Contains("FOC")) || (i.IsWarning) || (i.IsClamp));
                        if (lstviolations.Count() > 0)
                        {
                            LstVWParkingVehicle.ItemsSource = lstviolations;
                        }
                        else
                        {
                            LstVWParkingVehicle.ItemsSource = null;
                        }

                    }
                }
                else
                {
                    LstVWParkingVehicle.ItemsSource = lstVehicleHistory;
                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "SwitchViolation_Toggled");
            }
        }
        private async void ImgbtnPrint_Clicked(object sender, EventArgs e)
        {
            try
            {
                string printerName = string.Empty;
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
                            }

                        }
                    });
                    if (printerName == string.Empty || printerName == "")
                    {
                        await DisplayAlert("Alert", "Unable to find Bluetooth device", "Ok");
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "ImgbtnPrint_Clicked");
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