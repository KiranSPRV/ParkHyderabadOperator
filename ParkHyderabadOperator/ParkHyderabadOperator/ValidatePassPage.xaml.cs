using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
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
        string[] receiptlines = new string[15]; // Receipt Lines
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
        public async void GetAllPassedVehicles()
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
        public async Task GetSelectedVehicleDetails(CustomerVehicle selectedVehicle)
        {
            string Locations = string.Empty;
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
                        if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                        {
                            imageVehicleImage.Source = "bike_black.png";
                        }
                        if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                        {
                            imageVehicleImage.Source = "car_black.png";
                        }

                        labelCustomerName.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name;
                        labelVehicleDetails.Text = objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber;
                        labelParkingFeesDetails.Text = objResultCustomerVehiclePass.TotalAmount.ToString("N2") + "/-";
                        if (objResultCustomerVehiclePass.IssuedCard)
                        {
                            labelParkingPaymentType.Text = "Paid (Including NFC) - By " + objResultCustomerVehiclePass.PaymentTypeID.PaymentTypeName;
                        }
                        else
                        {
                            labelParkingPaymentType.Text = "Paid - By " + objResultCustomerVehiclePass.PaymentTypeID.PaymentTypeName;
                        }

                        if (objResultCustomerVehiclePass.CreatedBy.UserName != "")
                        {
                            imageOperatorProfile.IsVisible = true;
                            labelOperatorName.Text = objResultCustomerVehiclePass.CreatedBy.UserName;
                            labelOperatorID.Text ="- #"+Convert.ToString(objResultCustomerVehiclePass.CreatedBy.UserCode);
                        }
                        else
                        {
                            imageOperatorProfile.IsVisible = false;
                        }

                        if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode == "MP")
                        {
                            labelNFCCard.Text = objResultCustomerVehiclePass.CardNumber;
                        }
                        try
                        {

                            if (receiptlines != null && receiptlines.Length > 0)
                            {

                                receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                                receiptlines[1] = "\x1B\x21\x01" + "       " + Locations + "\n";
                                receiptlines[2] = " " + "\n";
                                receiptlines[3] = "\x1B\x21\x08" + objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode + "     " + objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber + "\x1B\x21\x00" + "\n";
                                receiptlines[4] = "\x1B\x21\x08" + Convert.ToDateTime(objResultCustomerVehiclePass.StartDate).ToString("dd MMM yyyy") + "-" + Convert.ToDateTime(objResultCustomerVehiclePass.ExpiryDate).ToString("dd MMM yyyy") + "\x1B\x21\x00\n";
                                receiptlines[5] = "" + "\n";
                                if (objResultCustomerVehiclePass.IssuedCard)
                                {
                                    receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + objResultCustomerVehiclePass.TotalAmount.ToString("N2") + "\x1B\x21\x00\n";
                                }
                                else
                                {
                                    receiptlines[6] = "\x1B\x21\x01" + "Paid Rs" + objResultCustomerVehiclePass.Amount.ToString("N2") + "\x1B\x21\x00\n";
                                }
                                receiptlines[7] = "\x1B\x21\x01" + "OPERATOR ID -" + objResultCustomerVehiclePass.CreatedBy.UserCode + "\x1B\x21\x00\n";
                                receiptlines[8] = "\x1B\x21\x01" + "Security available " + objResultCustomerVehiclePass.PrimaryLocationParkingLotID.LotTimmings + "\x1B\x21\x00\n";
                                receiptlines[9] = "\x1B\x21\x01" + "We are not responsible for your valuable items like laptop,      wallet,helmet etc." + "\x1B\x21\x00\n";
                                receiptlines[10] = "\x1B\x21\x01" + "GST Number 0012" + "\x1B\x21\x00\n";
                                receiptlines[11] = "\x1B\x21\x01" + "Amount includes 18% GST" + "\x1B\x21\x00\n";
                                receiptlines[12] = "" + "\n";
                                receiptlines[13] = "" + "\n";
                                receiptlines[14] = "" + "\n";
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
                    listViewVehicleRegistrationNumbers.IsVisible = false;
                else

                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "SearchBar_OnTextChanged");

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private async void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
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
                var masterPage = new MasterHomePage();
                await Navigation.PushAsync(masterPage);
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
                        await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ValidatePassPage.xaml.cs", "", "BtnPrint_Clicked");
                await DisplayAlert("Alert", "Unable to print,Please contact admin", "Ok");
            }
        }
    }
}