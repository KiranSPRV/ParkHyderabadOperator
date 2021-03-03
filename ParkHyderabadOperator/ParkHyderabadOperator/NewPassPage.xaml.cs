using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Pass;
using ParkHyderabadOperator.ViewModel.VMPass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPassPage : ContentPage
    {
        private ObservableCollection<VehicleType> _vehicleType=null;
        public string PassCategory = string.Empty;
        string SelectedVehicle = string.Empty;
        DALExceptionManagment dal_Exceptionlog;
        CustomerVehiclePass objReNewVehicle = null;
        public NewPassPage(string pass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            PassCategory = pass;
            if (PassCategory == "New Pass")
            {
                PassTypePageHeading.Text = "NEW PASS";
                GetAllVehicleType().Wait();
            }
            else if (PassCategory == "ReNew Pass")
            {
                PassTypePageHeading.Text = "CHOOSE PASS";
                PassTypePageHeading.Margin = new Thickness(0, 0, 100, 0);
                
                if (App.Current.Properties.ContainsKey("ReNewPassCustomerVehicle"))
                {
                    objReNewVehicle = (CustomerVehiclePass)App.Current.Properties["ReNewPassCustomerVehicle"];
                    if (objReNewVehicle != null && objReNewVehicle.CustomerVehiclePassID != 0)
                    {
                        GetSelectedVehicleType(objReNewVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode);
                        LoadPasseTypesAndPriceDetails(objReNewVehicle.CustomerVehicleID.VehicleTypeID.VehicleTypeCode);
                    }

                }


            }

        }
        private void LoadPasseTypesAndPriceDetails(string SelectedVehicle)
        {
            try
            {
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    VMPass vm_Pass = new VMPass();
                    DALPass dal_Pass = new DALPass();
                    VehicleLotPassPrice objVehicleLotPassPrice = new VehicleLotPassPrice();
                    objVehicleLotPassPrice.VehicleTypeCode = SelectedVehicle;
                    objVehicleLotPassPrice.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    List<PassPrice> lstVMPass = dal_Pass.GetPassPriceDetails(Convert.ToString(App.Current.Properties["apitoken"]), objVehicleLotPassPrice);
                    if (objReNewVehicle != null && objReNewVehicle.CustomerVehiclePassID != 0)
                    {
                        List<PassPrice> renewSelectedPass = lstVMPass.Where(p => (p.PassTypeID.PassTypeCode.ToUpper() == objReNewVehicle.PassPriceID.PassTypeID.PassTypeCode.ToUpper()) && p.StationAccess.ToUpper() == objReNewVehicle.PassPriceID.StationAccess.ToUpper()).ToList();
                        LstVehiclePasses.ItemsSource = renewSelectedPass;
                    }
                    else
                    {
                        LstVehiclePasses.ItemsSource = lstVMPass;
                    }


                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "LoadPasseTypesAndPriceDetails");
            }
        }
        private async void LstVehiclePasses_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                PassPrice selectedPass = (PassPrice)e.SelectedItem;
                if (selectedPass != null)
                {
                    if ( selectedPass.PassTypeID.PassTypeCode == "EP"|| selectedPass.PassTypeID.PassTypeCode == "DP")//Event/Day Pass
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new DailyPass(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "WP")//Weekly Pass
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new WeeklyPassPage(PassCategory, selectedPass));


                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "Single Station".ToUpper()))//Montly Pass -Single Station
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new MonthlyPassPage(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "Multi Station".ToUpper() || selectedPass.StationAccess.ToUpper() == "Multi Stations".ToUpper()))//Montly Pass -Multiple Station
                    {
                        await Navigation.PushAsync(new MultiStationPassPage(PassCategory, selectedPass));
                    }
                    else if (selectedPass.PassTypeID.PassTypeCode == "MP" && (selectedPass.StationAccess.ToUpper() == "All Station".ToUpper() || selectedPass.StationAccess.ToUpper() == "All Stations".ToUpper()))//Montly Pass -Multiple Station
                    {
                        if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                        {
                            App.Current.Properties.Remove("MultiSelectionLocations");
                        }
                        await Navigation.PushAsync(new MonthlyPassPage(PassCategory, selectedPass));
                    }
                }
                  ((ListView)LstVehiclePasses).SelectedItem = null;

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "LstVehiclePasses_ItemSelected");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutNewPassPage.Opacity = 0.5;
            }
            else
            {
                absLayoutNewPassPage.Opacity = 1;
            }

        }

        #region Dynamic VehicleType
        public async Task GetAllVehicleType()
        {
            try
            {
                DALPass objDALPass = new DALPass();
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType!=null&& lstVehicleType.Count > 0)
                    {
                        lstVehicleType = lstVehicleType.OrderBy(i => i.VehicleTypeID).ToList();
                        _vehicleType = new ObservableCollection<VehicleType>(lstVehicleType);
                        collstviewVehicleTye.WidthRequest = 300;
                        collstviewVehicleTye.ItemsSource = _vehicleType;
                        collstviewVehicleTye.SelectedItem = _vehicleType[0];
                        SelectedVehicle = _vehicleType[0].VehicleTypeCode;
                    }
                }
                
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "GetAllVehicleType");
            }
        }
        public async void GetSelectedVehicleType(string VehicleTypeCode)
        {
            try
            {

                DALPass objDALPass = new DALPass();
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType.Count>0)
                    {
                        var resultvehihcle = lstVehicleType.Where(v => v.VehicleTypeCode==VehicleTypeCode).ToList();
                        if(resultvehihcle!=null& resultvehihcle.Count>0)
                        {
                            _vehicleType = new ObservableCollection<VehicleType>(resultvehihcle);
                            if (_vehicleType.Count > 0)
                            {
                                for (var item = 0; item < _vehicleType.Count; item++)
                                {
                                    _vehicleType[item].VehicleDisplayImage = _vehicleType[item].VehicleActiveImage;
                                    _vehicleType[item] = _vehicleType[item];
                                }
                                collstviewVehicleTye.WidthRequest = 90;
                                collstviewVehicleTye.ItemsSource = _vehicleType;
                                SelectedVehicle = _vehicleType[0].VehicleTypeCode;
                            }
                        }
                       
                    }
                   
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "GetSelectedVehicleType");
            }
        }
        private void collstviewVehicleTye_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PassCategory == "New Pass")
                {
                    var item = e.CurrentSelection;
                    var selectedvehicle = item[0] as VehicleType;
                    if (!string.IsNullOrEmpty(selectedvehicle.VehicleImage))
                    {
                        UpdateCollectionViewSelectedItem(selectedvehicle);
                    }
                    LoadPasseTypesAndPriceDetails(selectedvehicle.VehicleTypeCode);
                }
                
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "collstviewVehicleTye_SelectionChanged");
            }
        }
        public void UpdateCollectionViewSelectedItem(VehicleType selectedVehicle)
        {
            try
            {
                for (var item = 0; item < _vehicleType.Count; item++)
                {

                    if (_vehicleType[item].VehicleTypeID == selectedVehicle.VehicleTypeID)
                    {

                        _vehicleType[item].VehicleDisplayImage = _vehicleType[item].VehicleActiveImage;
                    }
                    else
                    {
                        _vehicleType[item].VehicleDisplayImage = _vehicleType[item].VehicleInActiveImage;
                    }

                    _vehicleType[item] = _vehicleType[item];
                }
                collstviewVehicleTye.ItemsSource = _vehicleType;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "NewPassPage.xaml.cs", "", "UpdateCollectionViewSelectedItem");
            }
        }

        #endregion

    }
}