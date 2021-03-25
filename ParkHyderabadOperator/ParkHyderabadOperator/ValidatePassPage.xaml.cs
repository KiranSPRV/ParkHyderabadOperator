using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
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
    public partial class ValidatePassPage : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        string[] receiptlines = new string[17]; // Receipt Lines
        List<CustomerVehicle> lstCustomerVehicle = null;
        CustomerVehiclePass objResultCustomerVehiclePass;
        DALExceptionManagment dal_Exceptionlog;
        public ValidatePassPage()
        {
            InitializeComponent();
            imageOperatorProfile.IsVisible = false;
            slValidateReceipt.IsVisible = false;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            objResultCustomerVehiclePass = new CustomerVehiclePass();
            dal_Exceptionlog = new DALExceptionManagment();
            GetAllPassedVehicles();

        }

        #region Searh Box Related Code
        public void GetAllPassedVehicles()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALReNewPass dal_ReNewPass = new DALReNewPass();
                    lstCustomerVehicle = dal_ReNewPass.GetAllPassedVehicles(Convert.ToString(App.Current.Properties["apitoken"]));
                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle;
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "GetAllPassedVehicles");
            }
        }
        public void GetSelectedVehicleDetails(CustomerVehicle selectedVehicle)
        {
            string Locations = string.Empty;
            string vehicleType = string.Empty;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALPass dal_Pass = new DALPass();

                    List<CustomerVehiclePass> lstResultPasses = dal_Pass.GetCustomerValidateVehiclePassesDetailsByVehicle(Convert.ToString(App.Current.Properties["apitoken"]), selectedVehicle);
                    if (lstResultPasses.Count > 0)
                    {
                        objResultCustomerVehiclePass = lstResultPasses[0];
                        if (lstResultPasses[0].IsMultiLot)
                        {
                            for (var i = 0; i < lstResultPasses.Count; i++)
                            {
                                if (i == 0)
                                {
                                    Locations = lstResultPasses[i].LocationID.LocationName;
                                }
                                else
                                {

                                    Locations = Locations + "," + lstResultPasses[i].LocationID.LocationName;
                                }

                            }
                        }
                        else
                        {

                            Locations = objResultCustomerVehiclePass.LocationID.LocationName;
                        }

                        labelParkingReceiptTitle.Text = "InstaParking-" + objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeName; ;
                        labelParkingLot.Text = Locations + "-" + objResultCustomerVehiclePass.PassPriceID.StationAccess;
                        labelValidFrom.Text = Convert.ToDateTime(objResultCustomerVehiclePass.StartDate).ToString("dd MMM yyyy");
                        labelValidTo.Text = Convert.ToDateTime(objResultCustomerVehiclePass.ExpiryDate).ToString("dd MMM yyyy ");
                        vehicleType = objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeDisplayName;
                        imageVehicleImage.Source = objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleIcon;
                        labelCustomerName.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name;
                        labelVehicleDetails.Text = objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber;
                        labelParkingFeesDetails.Text = objResultCustomerVehiclePass.TotalAmount.ToString("N2") + "/-";
                        if (objResultCustomerVehiclePass.IssuedCard)
                        {
                            labelParkingPaymentType.Text = "Paid (Including " + objResultCustomerVehiclePass.CardTypeID.CardTypeName + ") - By " + objResultCustomerVehiclePass.PaymentTypeID.PaymentTypeName;
                        }
                        else
                        {
                            labelParkingPaymentType.Text = "Paid - By " + objResultCustomerVehiclePass.PaymentTypeID.PaymentTypeName;
                        }

                        if (objResultCustomerVehiclePass.CreatedBy.UserName != "")
                        {
                            imageOperatorProfile.IsVisible = true;
                            labelOperatorName.Text = objResultCustomerVehiclePass.CreatedBy.UserName;
                            labelOperatorID.Text = "- #" + Convert.ToString(objResultCustomerVehiclePass.CreatedBy.UserCode);
                        }
                        else
                        {
                            imageOperatorProfile.IsVisible = false;
                        }

                        if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode == "MP")
                        {
                            lableCardType.Text = "TAG: ";
                            labelNFCCard.Text = objResultCustomerVehiclePass.CardNumber;
                        }
                        try
                        {

                            if (receiptlines != null && receiptlines.Length > 0)
                            {
                                if (objResultCustomerVehiclePass.PassPriceID.StationAccess == "All Station" || objResultCustomerVehiclePass.PassPriceID.StationAccess == "All Stations")
                                {
                                    Locations = "All Metro Stations";
                                }

                                receiptlines[0] = "\x1B\x21\x08" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                                receiptlines[1] = "\x1B\x21\x01" + "          " + objResultCustomerVehiclePass.CreatedBy.LocationParkingLotID.LocationID.LocationName + "\x1B\x21\x00\n";
                                receiptlines[3] = "" + "\n";
                                receiptlines[4] = "\x1B\x21\x08" + vehicleType + "     " + objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00\n";
                                receiptlines[5] = "\x1B\x21\x01" + "Valid From:" + Convert.ToDateTime(objResultCustomerVehiclePass.StartDate).ToString("dd MMM yyyy") + "\x1B\x21\x00" + "\n";
                                receiptlines[6] = "\x1B\x21\x01" + "Valid Till:" + Convert.ToDateTime(objResultCustomerVehiclePass.ExpiryDate).ToString("dd MMM yyyy") + "\x1B\x21\x00" + "\n";
                                receiptlines[7] = "\x1B\x21\x01" + "(Pass Type :" + objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeName + ")" + "\x1B\x21\x00\n";
                                receiptlines[8] = "\x1B\x21\x01" + "Station(s):" + Locations + "\x1B\x21\x01" + "\n";
                                receiptlines[9] = "\x1B\x21\x01" + "Paid: Rs" + (objResultCustomerVehiclePass.IssuedCard ? objResultCustomerVehiclePass.TotalAmount.ToString("N2") + "(TAG Rs" + objResultCustomerVehiclePass.PassPriceID.CardPrice.ToString("N2") + ")" : objResultCustomerVehiclePass.Amount.ToString("N2")) + "\x1B\x21\x01" + "\n";
                                receiptlines[10] = "\x1B\x21\x06" + "Operator Id:" + objResultCustomerVehiclePass.CreatedBy.UserCode + "\x1B\x21\x00\n";
                                receiptlines[11] = "\x1B\x21\x01" + "(Supervisor Mobile:" + objResultCustomerVehiclePass.SuperVisorID.PhoneNumber + ")" + "\x1B\x21\x00\n";
                                receiptlines[12] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,       wallet,helmet etc." + "\x1B\x21\x00\n";
                                receiptlines[13] = "\x1B\x21\x06" + "GST Number 36AACFZ1015E1ZL" + "\x1B\x21\x00\n";
                                receiptlines[14] = "\x1B\x21\x06" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                                receiptlines[15] = "" + "\n";
                                receiptlines[16] = "" + "\n";

                            }
                        }
                        catch (Exception ex)
                        {
                            dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "receiptlines");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "GetSelectedVehicleDetails");
            }
        }
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            listViewVehicleRegistrationNumbers.IsVisible = true;
            try
            {
                listViewVehicleRegistrationNumbers.BeginRefresh();
                var dataEmpty = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    listViewVehicleRegistrationNumbers.IsVisible = false;
                    ClearFields();
                }
                else
                {
                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                }
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "SearchBar_OnTextChanged");

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            try
            {
                CustomerVehicle selecteditems = (CustomerVehicle)e.Item;
                if (selecteditems != null)
                {
                    searchBar.Text = selecteditems.RegistrationNumber;
                    slValidateReceipt.IsVisible = true;
                    GetSelectedVehicleDetails(selecteditems);
                }
                listViewVehicleRegistrationNumbers.IsVisible = false;
                ((ListView)sender).SelectedItem = null;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "listViewVehicleRegistrationNumbers_OnItemTapped");
            }

        }
        #endregion
        private async void BtnDone_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage masterHomePage = null;
                await Task.Run(() =>
                {
                    masterHomePage = new MasterHomePage();
                });
                await Navigation.PushAsync(masterHomePage);
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "BtnDone_Clicked");
            }
        }
        private async void BtnPrint_Clicked(object sender, EventArgs e)
        {
            try
            {

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
                    else
                    {
                        await DisplayAlert("Alert", "Unable to find Bluetooth device", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "BtnPrint_Clicked");
                await DisplayAlert("Alert", "Unable to Print,Please contact Admin", "Ok");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;

            if (show)
            {
                absLayoutValidatePasspage.Opacity = 0.5;
            }
            else
            {
                absLayoutValidatePasspage.Opacity = 1;
            }

        }
        public void ClearFields()
        {

            try
            {

                labelParkingReceiptTitle.Text = string.Empty;
                labelParkingLot.Text = string.Empty;
                labelValidFrom.Text = string.Empty;
                labelValidTo.Text = string.Empty;
                imageVehicleImage.Source = null;
                labelCustomerName.Text = string.Empty;
                labelVehicleDetails.Text = string.Empty;
                labelParkingFeesDetails.Text = string.Empty;
                labelParkingPaymentType.Text = string.Empty;
                labelOperatorName.Text = string.Empty;
                labelOperatorID.Text = string.Empty;
                lableCardType.Text = string.Empty;
                labelNFCCard.Text = string.Empty;
                imageOperatorProfile.IsVisible = false;
                slValidateReceipt.IsVisible = false;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "ClearFields");
            }
        }
    }
}