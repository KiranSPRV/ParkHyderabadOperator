using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
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
    public partial class HistoryPage : ContentPage
    {
        List<CustomerVehicle> lstVehicle;
        List<CustomerParkingSlot> lstVehicleHistory;
        DALMenubar dal_Menubar = null;
        DALExceptionManagment dal_Exceptionlog;
        bool IsRun=false;
        public HistoryPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Menubar = new DALMenubar();
            lstVehicle = new List<CustomerVehicle>();
            lstVehicleHistory = new List<CustomerParkingSlot>();
            ListVehicles();
        }

        #region Search BAR
        public void ListVehicles()
        {
            try
            {
              
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    lstVehicle = dal_Menubar.GetAllVehicleRegistrationNumbers(Convert.ToString(App.Current.Properties["apitoken"]));
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
                if (listsd.VehicleTypeID.VehicleTypeCode.ToUpper() == "2W")
                {
                    ImgSelectedVehicle.Source = "bike_black.png";
                }
                else if (listsd.VehicleTypeID.VehicleTypeCode.ToUpper() == "4W")
                {
                    ImgSelectedVehicle.Source = "car_black.png";
                }
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
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstVehicleHistory = dal_Menubar.GetVehicleParkingHistory(Convert.ToString(App.Current.Properties["apitoken"]), objregistraionnumber);
                    LstVWParkingVehicle.ItemsSource = lstVehicleHistory;
                }
            }
            catch (Exception ex)
            {
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
                        var lstviolations = lstVehicleHistory.Where(i => (i.ApplicationTypeID.ApplicationTypeCode.ToUpper().Contains("V")) || (i.StatusID.StatusCode.ToUpper().Contains("FOC") )|| (i.IsWarning) || (i.IsClamp));
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
    }
}