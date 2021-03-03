using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DynamicVehiclePage : ContentPage
    {
        DALHome dal_Home;
        private ObservableCollection<VehicleType> _vehicleType;
        public DynamicVehiclePage()
        {
            dal_Home = new DALHome();
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private void collstviewVehicleTye_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.CurrentSelection;
                var selectedvehicle = item[0] as VehicleType;
                if (selectedvehicle.VehicleTypeID!=0)
                {
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

            }
        }
        public async void GetAllVehicleType()
        {
            try
            {
                 DALPass objDALPass = new DALPass();
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    _vehicleType = new ObservableCollection<VehicleType>(lstVehicleType);
                    if (_vehicleType.Count > 0)
                    {
                        collstviewVehicleTye.ItemsSource = _vehicleType;
                        collstviewVehicleTye.SelectedItem = _vehicleType[0];
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                GetAllVehicleType();
            }
            catch (Exception ex)
            {

            }


        }
        public void GetViolationVehcileInformation(int customerParkingSlotID)
        {
           
            if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
            {
                var objresult = dal_Home.GetSelectedParkedVehicleDetails(Convert.ToString(App.Current.Properties["apitoken"]), customerParkingSlotID);
                if (objresult.CustomerParkingSlotID != 0)
                {

                    List<VehicleType> lstVehicleType = new List<VehicleType>();
                    var vehType = new VehicleType()
                    {
                        VehicleTypeID = 1,
                        VehicleInActiveImage =  objresult.CustomerVehicleID.VehicleTypeID.VehicleIcon,
                        VehicleTypeCode = "2W",
                        VehicleTypeName = "Two Wheeler"

                    };
                    lstVehicleType.Add(vehType);
                    _vehicleType = new ObservableCollection<VehicleType>(lstVehicleType);

                    if (_vehicleType.Count > 0)
                    {
                        collstviewVehicleTye.ItemsSource = _vehicleType;
                        //collstviewVehicleTye.SelectedItem = _vehicleType[0];
                    }
                }


            }
        }
        public byte[] ConvertImageToByteArray()
        {
            byte[] imgByteArray = null;
            try
            {


                byte[] buffer = null;
                var assembly = this.GetType().GetTypeInfo().Assembly;
                ImageSource.FromFile("bike_blue.png");
                using (var s = assembly.GetManifestResourceStream("bike_blue.png"))
                {
                    if (s != null)
                    {
                        var length = s.Length;
                        imgByteArray = new byte[length];
                        s.Read(buffer, 0, (int)length);
                    }
                }

                // here imageByteArray will have the bytes from the image file or it will be null if the file was not loaded.

                if (imgByteArray != null)
                {
                    //use your data here.
                }
            }
            catch (Exception ex)
            {

            }
            return imgByteArray;
        }
       
    }
}