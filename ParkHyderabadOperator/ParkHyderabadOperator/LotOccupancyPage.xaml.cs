using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALReport;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.LotOccupancy;
using ParkHyderabadOperator.ViewModel.VMHome;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LotOccupancyPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        List<VMLocationLots> lstlots = null;
        List<User> lstOperators = null;
        User objReportUser = null;

        public LotOccupancyPage()
        {
            InitializeComponent();
            objReportUser = new User();
            dal_Exceptionlog = new DALExceptionManagment();
            lstlots = new List<VMLocationLots>();
            LoadLoginUserLocationLots();

        }
        #region Picker Location Lot
        private void LoadLoginUserLocationLots()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    User objLoginUser = (User)App.Current.Properties["LoginUser"];
                    objLoginUser.LocationParkingLotID.LocationParkingLotID = 0;
                    lstlots = dal_Home.GetUserAllocatedLocationAndLots(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);
                    // Include ALL 
                    VMLocationLots objlotAll = new VMLocationLots();
                    if (lstlots.Count > 0)
                    {
                        pickerLocationLot.ItemsSource = lstlots;
                        for (int x = 0; x < lstlots.Count; x++)
                        {
                            if (objLoginUser.LocationParkingLotID.LocationParkingLotID == null || objLoginUser.LocationParkingLotID.LocationParkingLotID == 0)
                            {
                                if (lstlots[x].LocationID != null && lstlots[x].LocationID != 0)
                                {
                                    if (lstlots[x].LocationID == objLoginUser.LocationParkingLotID.LocationID.LocationID)
                                    {
                                        pickerLocationLot.SelectedIndex = x;
                                    }
                                }

                            }
                            else
                            {
                                if (lstlots[x].LocationParkingLotID == objLoginUser.LocationParkingLotID.LocationParkingLotID)
                                {
                                    pickerLocationLot.SelectedIndex = x;

                                }
                            }

                        }
                    }


                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LotOccupancy.xaml.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private void LoadLocationLotActiveOperators(VMLocationLots objVMLocationLot, int UserID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();

                    objReportUser.UserID = UserID;
                    objReportUser.LocationParkingLotID.LocationID.LocationID = objVMLocationLot.LocationID;
                    objReportUser.LocationParkingLotID.LocationParkingLotID = objVMLocationLot.LocationParkingLotID;
                    lstOperators = dal_Home.GetLocationLotActiveOperators(Convert.ToString(App.Current.Properties["apitoken"]), objReportUser);
                    var loginuser = (User)App.Current.Properties["LoginUser"];
                    if ((loginuser.UserTypeID.UserTypeName).ToUpper() == "OPERATOR".ToUpper())
                    {
                        lstOperators.RemoveAll(item => item.UserID != loginuser.UserID);
                    }
                    if (lstOperators.Count > 0)
                    {
                        pickerOperator.ItemsSource = lstOperators;
                        pickerOperator.SelectedIndex = 1;
                        GetLotOccupancy(objReportUser);
                    }

                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LotOccupancy.xaml.cs", "", "LoadLocationLotActiveOperators");
            }
        }
        private async void PickerLocationLot_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    if (pickerLocationLot.SelectedItem != null)
                    {
                        User objLoginUser = (User)App.Current.Properties["LoginUser"];
                        VMLocationLots objVMLocations = (VMLocationLots)pickerLocationLot.SelectedItem;
                        objLoginUser.LocationParkingLotID.LocationParkingLotID = objVMLocations.LocationParkingLotID;
                        LoadLocationLotActiveOperators(objVMLocations, objLoginUser.UserID);
                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Unable to proceed,login user and token details are not avialable", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LotOccupancy.xaml.cs", "", "PickerLocationLot_SelectedIndexChanged");
            }
        }
        private void PickerOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {

            }

        }
        #endregion
        public async void GetLotOccupancy(User objloginUserLot)
        {
            try
            {
                ShowLoading(true);
                List<LocationLotOccupancyReport> lstOccupancy = null;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALReport dal_Report = new DALReport();
                    objloginUserLot.LoginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 06, 00, 00);
                    objloginUserLot.LogoutTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    lstOccupancy = dal_Report.GetLocationLotOccupancyReport(Convert.ToString(App.Current.Properties["apitoken"]), objloginUserLot);
                    if (lstOccupancy != null && lstOccupancy.Count > 0)
                    {
                        lvLotOccupancyReport.ItemsSource = lstOccupancy;
                    }
                    else
                    {
                        lvLotOccupancyReport.ItemsSource = null;
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "LotOccupancy.xaml.cs", "", "GetLotOccupancy");
            }
        }
        private void lvLotOccupancyReport_Refreshing(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    if (objReportUser != null)
                    {
                        GetLotOccupancy(objReportUser);
                        lvLotOccupancyReport.IsRefreshing = false;
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutLotOccupancyPage.Opacity = 0.5;
            }
            else
            {
                absLayoutLotOccupancyPage.Opacity = 1;
            }
        }

    }
}