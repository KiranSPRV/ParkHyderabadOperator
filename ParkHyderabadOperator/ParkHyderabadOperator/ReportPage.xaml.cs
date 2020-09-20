using ParkHyderabadOperator.DAL;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Report;
using ParkHyderabadOperator.ViewModel.Reports;
using ParkHyderabadOperator.ViewModel.VMHome;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {

        
        DALExceptionManagment dal_Exceptionlog;
        List<VMLocationLots> lstlots = null;
        List<User> lstOperators = null;
        public ReportPage()
        {
            InitializeComponent();
            lstlots = new List<VMLocationLots>();
            datePickerToTime.Time = new TimeSpan(23, 55, 0);
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
                    
                    lstlots = dal_Home.GetUserAllocatedLocationAndLots(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);
                    // Include ALL 
                    VMLocationLots objlotAll = new VMLocationLots();
                    objlotAll.LocationParkingLotID = 0;
                    objlotAll.LotName = "ALL";
                    objlotAll.LocationParkingLotName = "ALL";
                    objlotAll.LocationID = 0;
                    objlotAll.LocationName = "ALL";
                    objlotAll.IsActive = true;
                    lstlots.Insert(0,objlotAll);

                    if (lstlots.Count > 0)
                    {
                        pickerLocationLot.ItemsSource = lstlots;
                        for (int x = 0; x < lstlots.Count; x++)
                        {
                            if (objLoginUser.LocationParkingLotID.LocationParkingLotID == null || objLoginUser.LocationParkingLotID.LocationParkingLotID == 0)
                            {
                                if(lstlots[x].LocationID!=null && lstlots[x].LocationID!=0)
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
                    if ((objLoginUser.UserTypeID.UserTypeName).ToUpper() == "OPERATOR".ToUpper())
                    {
                        pickerOperator.Title = objLoginUser.UserName + "-" + Convert.ToString(objLoginUser.UserCode);
                    }
                   
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private void LoadAllOperators(User objLoginUser)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    lstOperators = dal_Home.GetAllOperatorsOfSupervisor(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);

                    if (lstOperators.Count > 0)
                    {
                        pickerOperator.ItemsSource = lstOperators;
                        pickerOperator.SelectedIndex = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private void LoadLocationLotActiveOperators(VMLocationLots objVMLocationLot)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    LocationParkingLot objselectedlocationlot = new LocationParkingLot();
                    objselectedlocationlot.LocationID.LocationID = objVMLocationLot.LocationID;
                    objselectedlocationlot.LocationParkingLotID = objVMLocationLot.LocationParkingLotID;
                    lstOperators = dal_Home.GetLocationLotActiveOperators(Convert.ToString(App.Current.Properties["apitoken"]), objselectedlocationlot);

                    if (lstOperators.Count > 0)
                    {
                        pickerOperator.ItemsSource = lstOperators;
                        pickerOperator.SelectedIndex = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "LoadLoginUserLocationLots");
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
                        if (objLoginUser.UserTypeID.UserTypeName.ToUpper() == "Supervisor".ToUpper())
                        {
                            LoadLocationLotActiveOperators(objVMLocations);
                        }

                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Unable to proceed,login user and token details are not avialable", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private async void PickerOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                }
                else
                {

                    await DisplayAlert("Alert", "Unable to proceed,login user and token details are not avialable", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "PickerOperator_SelectedIndexChanged");
            }
        }
        #endregion
        private async void ButtonGenerateReport_Clicked(object sender, EventArgs e)
        {
            try
            {

                if (DeviceInternet.InternetConnected())
                {
                    LocationLotParkingReport objlot = null;
                    User objLoginUser = new User();
                    VMLocationLots objselectedlot = (VMLocationLots)pickerLocationLot.SelectedItem;
                    if (objselectedlot != null)
                    {
                        objLoginUser.LocationParkingLotID.LocationID.LocationID = objselectedlot.LocationID;
                        objLoginUser.LocationParkingLotID.LocationParkingLotID = objselectedlot.LocationParkingLotID;
                        if (pickerOperator.SelectedItem != null)
                        {
                            var objselectedOperator = (User)pickerOperator.SelectedItem;
                            objLoginUser.UserID = objselectedOperator.UserID;
                        }
                        else
                        {
                            var userobj = (User)App.Current.Properties["LoginUser"];
                            objLoginUser.UserID = userobj.UserID;
                        }

                        if(objselectedlot.LocationID==0 && objselectedlot.LocationParkingLotID == 0 && objselectedlot.LotName=="ALL")
                        {
                            var userobj = (User)App.Current.Properties["LoginUser"];
                            objLoginUser.UserID = userobj.UserID;
                        }
                    }
                    ShowLoading(true);
                    await Task.Run(() =>
                    {
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                        {

                            DateTime fromDate = new DateTime(datePickerFromDate.Date.Year, datePickerFromDate.Date.Month, datePickerFromDate.Date.Day, datePickerFromTime.Time.Hours, datePickerFromTime.Time.Minutes, 0);
                            objLoginUser.LoginTime = fromDate;
                            objLoginUser.LogoutTime = new DateTime(datePickerToDate.Date.Year, datePickerToDate.Date.Month, datePickerToDate.Date.Day, datePickerToTime.Time.Hours, datePickerToTime.Time.Minutes, 0); ;
                            objlot = new LocationLotParkingReport(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);

                        }
                    });
                    if (objlot != null)
                    {
                        BindingContext = objlot;
                        lstvwStationPassesReport.BindingContext = objlot.GetLocationLotPassesReport();
                        lstvwStationViolationReport.BindingContext = objlot.GetLocationLotViolationReport();
                        StationVehicles objStationVehicles = objlot.GetLocationLotTotalRevenue();
                        TotalStationVehicleCash.Text = objStationVehicles.StationVehicleCash.ToString("N2");
                        TotalStationVehicleEpay.Text = objStationVehicles.StationVehicleEPay.ToString("N2");
                    }
                    ShowLoading(false);
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                    ShowLoading(false);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "ButtonGenerateReport_Clicked");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;


        }

    }

}