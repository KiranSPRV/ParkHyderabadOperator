using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentCheckOutDetailsPage : ContentPage
    {
        List<CustomerVehicle> lstVehicle;
        List<CustomerParkingSlot> lstVehicleHistory;
        DALMenubar dal_Menubar = null;
        DALExceptionManagment dal_Exceptionlog;
        public RecentCheckOutDetailsPage()
        {
            InitializeComponent();
        }
        public RecentCheckOutDetailsPage(int CustomerParkingSlotID)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_Menubar = new DALMenubar();
            lstVehicle = new List<CustomerVehicle>();
            lstVehicleHistory = new List<CustomerParkingSlot>();
            GetVehicleDeatils(CustomerParkingSlotID);
        }
        public void GetVehicleDeatils(int CustomerParkingSlotID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    lstVehicleHistory = dal_Menubar.GetVehicleRecentCheckOutDetails(Convert.ToString(App.Current.Properties["apitoken"]), CustomerParkingSlotID);
                    LstVWRecentCheckOutDetails.ItemsSource = lstVehicleHistory;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOutDetailsPage.xaml.cs", "", "GetVehicleDeatils");
            }
        }
    }
}