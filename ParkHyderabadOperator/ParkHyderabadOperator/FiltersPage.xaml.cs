using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiltersPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        DALHome dal_Home;
        string SelectedVehicle;
        ParkedVehiclesFilter objFilter;
        List<ApplicationType> objSelectedAppTypes;
        List<Status> objSelectedStatus;
        private ObservableCollection<VehicleType> filterVehicleType;
        public FiltersPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            objFilter = new ParkedVehiclesFilter();
            objSelectedAppTypes = new List<ApplicationType>();
            objSelectedStatus = new List<Status>();
            dal_Home = new DALHome();
            GetAllApplicationTypes();
            GetAllStatus();
            GetAllVehicleType();
        }

        private async void BtnApply_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                MasterHomePage filerpage = null;
                if (App.Current.Properties.ContainsKey("LoginUser") )
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    await Task.Run(() =>
                    {
                        objFilter.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                        objFilter.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                        objFilter.ApplicationTypeCode = objSelectedAppTypes;
                        objFilter.StatusCode = objSelectedStatus;
                        objFilter.VehicleTypeCode = SelectedVehicle;
                        filerpage = new MasterHomePage(objFilter);
                    });
                    await Navigation.PushAsync(filerpage);
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        #region ApplicationTypes
        public void GetAllApplicationTypes()
        {

            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    List<ApplicationType> lstApplicationType = dal_Home.GetAllApplicationTypes(Convert.ToString(App.Current.Properties["apitoken"]));
                    LstApplicationTypes.ItemsSource = lstApplicationType;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "GetAllApplicationTypes");
            }
        }
        private void SwitchApplicationType_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                var swithch = (Switch)sender;
                ApplicationType ob = swithch.BindingContext as ApplicationType;
                if (swithch.IsToggled)
                {
                    objSelectedAppTypes.Add(ob);
                }
                else
                {
                    if (objSelectedAppTypes.Count > 0)
                    {
                        var item = objSelectedAppTypes.SingleOrDefault(x => x.ApplicationTypeID == ob.ApplicationTypeID);
                        if (item != null)
                        {
                            objSelectedAppTypes.Remove(ob);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "SwitchApplicationType_Toggled");
            }
        }
        #endregion

        #region Status
        public void GetAllStatus()
        {

            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    List<Status> lstApplicationType = dal_Home.GetAllStatus(Convert.ToString(App.Current.Properties["apitoken"]));
                    LstStatus.ItemsSource = lstApplicationType;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "GetAllApplicationTypes");
            }
        }
        private void SwitchStatus_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                var swithch = (Switch)sender;
                Status obj = swithch.BindingContext as Status;
                if (swithch.IsToggled)
                {
                    if (obj.StatusCode.ToUpper() == "C")
                    {
                        objFilter.IsClamped = true;
                    }
                    else
                    {
                        objSelectedStatus.Add(obj);
                    }

                }
                else
                {
                    if (obj.StatusCode.ToUpper() == "C")
                    {
                        objFilter.IsClamped = false;
                    }
                    else
                    {
                        if (objSelectedStatus.Count > 0)
                        {
                            var item = objSelectedStatus.SingleOrDefault(x => x.StatusID == obj.StatusID);
                            if (item != null)
                            {
                                objSelectedStatus.Remove(obj);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "FiltersPage.xaml.cs", "", "SwitchStatus_Toggled");
            }

        }

        #endregion

        #region Dynamic Vehicle Type List
        private void collstviewVehicleTye_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.CurrentSelection;
                var selectedvehicle = item[0] as VehicleType;
                if (selectedvehicle.VehicleTypeID != 0)
                {
                    SelectedVehicle = selectedvehicle.VehicleTypeCode;
                    UpdateCollectionViewSelectedItem(selectedvehicle);
                }

            }
            catch (Exception ex)
            {

            }
        }
        public void UpdateCollectionViewSelectedItem(VehicleType selectedVehicle)
        {
            try
            {
                for (var item = 0; item < filterVehicleType.Count; item++)
                {

                    if (filterVehicleType[item].VehicleTypeID == selectedVehicle.VehicleTypeID)
                    {

                        filterVehicleType[item].VehicleDisplayImage = filterVehicleType[item].VehicleActiveImage;
                    }
                    else
                    {
                        filterVehicleType[item].VehicleDisplayImage = filterVehicleType[item].VehicleInActiveImage;
                    }

                    filterVehicleType[item] = filterVehicleType[item];
                }
                collstviewVehicleTye.ItemsSource = filterVehicleType;
            }
            catch (Exception ex)
            {

            }
        }
        public async void GetAllVehicleType()
        {
            try
            {

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType.Count > 0)
                    {
                        lstVehicleType = lstVehicleType.OrderBy(i => i.VehicleTypeID).ToList();
                        filterVehicleType = new ObservableCollection<VehicleType>(lstVehicleType);
                        collstviewVehicleTye.ItemsSource = filterVehicleType;
                        collstviewVehicleTye.SelectedItem = filterVehicleType[0];
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absFilterPage.Opacity = 0.5;
            }
            else
            {
                absFilterPage.Opacity = 1;
            }

        }
    }
}