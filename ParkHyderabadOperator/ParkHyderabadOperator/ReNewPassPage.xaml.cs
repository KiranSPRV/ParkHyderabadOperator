using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIInputModel;
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
    public partial class ReNewPassPage : ContentPage
    {

        List<CustomerVehicle> lstCustomerVehicle = null;
        CustomerVehiclePass objResultCustomerVehiclePass;
        DALExceptionManagment dal_Exceptionlog;
        DALCheckIn dal_DALCheckIn;
        public ReNewPassPage()
        {
            InitializeComponent();

            lstCustomerVehicle = new List<CustomerVehicle>();
            objResultCustomerVehiclePass = new CustomerVehiclePass();
            dal_DALCheckIn = new DALCheckIn();
            dal_Exceptionlog = new DALExceptionManagment();
            if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
            {
                App.Current.Properties.Remove("ReNewPassCustomerVehicle");
            }
            GetAllPassedVehicles();
            BtnChoosePass.IsEnabled = false;
        }
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
                await DisplayAlert("", "" + ex, "Ok");
            }
        }
        public async Task GetSelectedVehicleDetails(CustomerVehicle selectedVehicle)
        {
            try
            {
                string IsPassInOverstay = string.Empty;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALPass dal_Pass = new DALPass();

                    objResultCustomerVehiclePass = dal_Pass.GetCustomerVehicleDetailsByVehicle(Convert.ToString(App.Current.Properties["apitoken"]), selectedVehicle);
                    if (objResultCustomerVehiclePass != null)
                    {
                        IsPassInOverstay = VerifyPassVehicleCheckInStatus(objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode, objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber);
                        if(IsPassInOverstay==string.Empty)
                        {
                            entryCustomerName.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name;
                            entryPhoneNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber;
                            entryRegistrationNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber;
                            labelNFCCardAmount.Text = objResultCustomerVehiclePass.PassPriceID.NFCCardPrice == 0 ? "0.00" : " ( ₹ " + objResultCustomerVehiclePass.PassPriceID.NFCCardPrice.ToString("N2") + " EXTRA )";
                            BtnChoosePass.IsEnabled = true;
                            // Verify Customer Vehicle Type
                            if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                            {
                                imgCustomerVehcileType.Source = ImageSource.FromFile("bike_black.png");
                            }
                            else if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                            {
                                imgCustomerVehcileType.Source = ImageSource.FromFile("car_black.png");
                            }
                            if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                            {
                                slNFC.IsVisible = false;
                            }
                            else
                            {
                                slNFC.IsVisible = true;
                            }
                        }
                        else
                        {
                            
                            await DisplayAlert("Alert", "Please clear due amount to Buy/Renew Pass", "Ok");
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("", "" + ex, "Ok");
            }
        }
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            listViewVehicleRegistrationNumbers.IsVisible = true;
            listViewVehicleRegistrationNumbers.BeginRefresh();

            try
            {
                var dataEmpty = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                    listViewVehicleRegistrationNumbers.IsVisible = false;
                else
                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private async void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            CustomerVehicle selecteditems = (CustomerVehicle)e.Item;
            if (selecteditems != null)
            {
                searchBar.Text = selecteditems.RegistrationNumber;
                GetSelectedVehicleDetails(selecteditems);
            }
            listViewVehicleRegistrationNumbers.IsVisible = false;
            ((ListView)sender).SelectedItem = null;
        }
        private async void BtnChoosePass_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber != null || objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber != "")
                {
                    objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber = entryPhoneNumber.Text;
                }
                if (objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name != null || objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name != "")
                {
                    objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name = entryCustomerName.Text;
                }
                
                if (checkBoxLostNFC.IsChecked)
                {
                    objResultCustomerVehiclePass.IssuedCard = true;
                }
                else
                {
                    objResultCustomerVehiclePass.IssuedCard = false;
                }
                if (!App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                {
                    App.Current.Properties["ReNewPassCustomerVehicle"] = objResultCustomerVehiclePass;
                }
                else
                {
                    App.Current.Properties["ReNewPassCustomerVehicle"] = objResultCustomerVehiclePass;
                }
                var newPassPage = new NewPassPage("ReNew Pass");
                await Navigation.PushAsync(newPassPage);
            }
            catch (Exception ex)
            {
            }
        }
        public string VerifyPassVehicleCheckInStatus(string vehicleTypeCode,string registrationNumber) // Verify Vehicle already parked
        {
            string alreadyCheckIn = string.Empty;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                    objPassVehicle.RegistrationNumber = registrationNumber;
                    objPassVehicle.VehicleTypeCode = vehicleTypeCode;
                    CustomerParkingSlot resultobj = dal_DALCheckIn.VerifyVehicleChekcInStatus(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                    if (resultobj.CustomerParkingSlotID != 0 && resultobj.StatusID.StatusCode=="O")
                    {
                        alreadyCheckIn = resultobj.LocationParkingLotID.LocationID.LocationName + "-" + resultobj.LocationParkingLotID.LocationParkingLotName;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReNewPassPage.xaml.cs", "", "VerifyPassVehicleCheckInStatus");
            }
            return alreadyCheckIn;
        }

    }
}