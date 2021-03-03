using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel.VMHome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeSyncRecordsPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        List<LocationLotParkedVehicles> lstofflinevehicles = null;
        public DeSyncRecordsPage()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            LoadDeSyncVehicle();
        }
        public void LoadDeSyncVehicle()
        {
            try
            {

                LstDeSyncVehicle.ItemsSource = null;
                DALHome dal_Home = new DALHome();
                VMLocationLotParkedVehicles vmVehicles = dal_Home.GetAllDeSyncVehiclesOffline();
                if (vmVehicles.CustomerParkingSlotID != null && vmVehicles.CustomerParkingSlotID.Count > 0)
                {
                    lstofflinevehicles = vmVehicles.CustomerParkingSlotID;
                    LstDeSyncVehicle.ItemsSource = vmVehicles.CustomerParkingSlotID;

                    labelTotalTwoWheeler.Text = Convert.ToString(vmVehicles.TotalTwoWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutTwoWheeler) + ")";
                    labelTotalFourWheeler.Text = Convert.ToString(vmVehicles.TotalFourWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutFourWheeler) + ")";
                    labelTotalHVWheeler.Text = Convert.ToString(vmVehicles.TotalHVWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutHVWheeler) + ")";
                    labelTotalThreeWheeler.Text = Convert.ToString(vmVehicles.TotalThreeWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutThreeWheeler) + ")";
                }
                else
                {
                    labelTotalTwoWheeler.Text = labelTotalFourWheeler.Text = labelTotalHVWheeler.Text = labelTotalThreeWheeler.Text = string.Empty;

                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MasterDetailHomePage.xaml.cs", "", "LoadParkedVehicle");
            }
        }
        private void SrbSearchVehicle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowLoading(true);
                if (lstofflinevehicles != null && lstofflinevehicles.Count > 0)
                {
                    if (!string.IsNullOrEmpty(e.NewTextValue))
                    {
                        LstDeSyncVehicle.ItemsSource = lstofflinevehicles.Where(x => x.RegistrationNumber.ToUpper().Contains(e.NewTextValue.ToUpper()));
                    }
                    else
                    {
                        LstDeSyncVehicle.ItemsSource = lstofflinevehicles;
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MasterDetailHomePage.xaml.cs", "", "SrbSearchVehicle_TextChanged");
            }
        }
        private void LstDeSyncVehicle_Refreshing(object sender, EventArgs e)
        {
            try
            {
                LoadDeSyncVehicle();
                LstDeSyncVehicle.IsRefreshing = false;
            }
            catch (Exception ex) { }
        }
        private async void frmClearDeSynchGesutre_Tapped(object sender, EventArgs e)
        {
            StringBuilder sbUnMoved = new StringBuilder();
            string delMsg = "Records deleted successfully";
            try
            {
                ShowLoading(true);
                var loguser = (User)App.Current.Properties["LoginUser"];
                frmClear.BorderColor = Color.FromHex("#3293FA");
                var lstchekIns = await App.SQLiteDb.GetDeSyncCheckInAsync();
                if (lstchekIns != null)
                {
                    if (lstchekIns.Count > 0)
                    {
                        foreach (var items in lstchekIns)
                        {
                            App.SQLiteDb.DeleteDesycItemAsync(items).Wait();
                        }
                    }
                }
                LoadDeSyncVehicle();
                ShowLoading(false);
                await DisplayAlert("Alert", delMsg, "Ok");

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "frmOnlineSynchGesutre_Tapped");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutMasterDetailHomePage.Opacity = 0.5;
            }
            else
            {
                absLayoutMasterDetailHomePage.Opacity = 1;
            }

        }


    }
}