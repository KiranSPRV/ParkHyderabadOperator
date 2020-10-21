using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.DAL.DALReport;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.RecentCheckOuts;
using ParkHyderabadOperator.ViewModel;
using ParkHyderabadOperator.ViewModel.Reports;
using ParkHyderabadOperator.ViewModel.VMHome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentCheckOuts : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        DALExceptionManagment dal_Exceptionlog;
        List<VMLocationLots> lstlots = null;
        List<User> lstOperators = null;
        LocationLotParkingReport objlot = null;
        User objReportUser = null;
        RecentCheckOutFilter objFilter = null;
        List<VMRecentCheckOuts> VMRecentCheckOutsID = null;
        public RecentCheckOuts()
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            lstlots = new List<VMLocationLots>();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            objFilter = new RecentCheckOutFilter();
            LoadLoginUserLocationLots();
            LoadCheckOutPickerDays();
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
                    lstlots.Insert(0, objlotAll);

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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "LoadLoginUserLocationLots");
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "LoadLoginUserLocationLots");
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
                    var loginuser = (User)App.Current.Properties["LoginUser"];
                    if ((loginuser.UserTypeID.UserTypeName).ToUpper() == "OPERATOR".ToUpper())
                    {
                        lstOperators.RemoveAll(item => item.UserID != loginuser.UserID);


                    }
                    if (lstOperators.Count > 0)
                    {
                        pickerOperator.ItemsSource = lstOperators;
                        pickerOperator.SelectedIndex = 1;
                    }

                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "LoadLocationLotActiveOperators");
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
                        //if (objLoginUser.UserTypeID.UserTypeName.ToUpper() == "Supervisor".ToUpper())
                        //{
                        LoadLocationLotActiveOperators(objVMLocations);
                        //}

                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Unable to proceed,login user and token details are not avialable", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "PickerLocationLot_SelectedIndexChanged");
            }
        }

        #endregion

        #region Picker Day
        public void LoadCheckOutPickerDays()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALMenubar dal_menubar = new DALMenubar();
                    pickerDay.ItemsSource = dal_menubar.GetRecentCheckOutDays();
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private void PickerDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetRecentCheckOuts();
        }

        #endregion
        public async void GetRecentCheckOuts()
        {
            try
            {
                ShowLoading(true);
                RecentCheckOutReport objreport = null;
                lvCheckInChkOutReport.ItemsSource = null;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objLoginUser = (User)App.Current.Properties["LoginUser"];
                    if (objLoginUser != null && objLoginUser.UserID != 0)
                    {
                        if (pickerLocationLot.SelectedItem != null && pickerDay.SelectedItem != null && pickerOperator.SelectedItem != null)
                        {
                            await Task.Run(() =>
                            {
                                var objSelectedLocation = (VMLocationLots)pickerLocationLot.SelectedItem;
                                var objRecentCheckOutDays = (RecentCheckOutDays)pickerDay.SelectedItem;
                                var objselectedOperator = (User)pickerOperator.SelectedItem;
                                objFilter.LocationID = objSelectedLocation.LocationID;
                                objFilter.LocationParkingLotID = objSelectedLocation.LocationParkingLotID;
                                objFilter.UserID = objselectedOperator.UserID;
                                switch (objRecentCheckOutDays.Day)
                                {
                                    case "Today":
                                        objFilter.SelectedDay = DateTime.Now;
                                        break;
                                    case "Yesterday":
                                        objFilter.SelectedDay = DateTime.Now.AddDays(-1);
                                        break;
                                    case "Daybefore Yesterday":
                                        objFilter.SelectedDay = DateTime.Now.AddDays(-2);
                                        break;

                                }
                                if (objFilter != null)
                                {
                                    DALReport dal_Report = new DALReport();
                                    objreport = dal_Report.GetRecentCheckOutReport(Convert.ToString(App.Current.Properties["apitoken"]), objFilter);


                                }

                            });
                            if (objreport != null)
                            {
                                if (objreport.RecentCheckOutID.Count > 0)
                                {
                                    VMRecentCheckOutsID = objreport.RecentCheckOutID;
                                    lvCheckInChkOutReport.ItemsSource = VMRecentCheckOutsID;
                                    lblTotalCash.Text = "TOTAL CASH: " + "₹" + objreport.TotalCash.ToString("N2");
                                    lblTotalEPay.Text = "TOTAL EPAY: " + "₹" + objreport.TotalEpay.ToString("N2");
                                }
                                else
                                {
                                    lvCheckInChkOutReport.ItemsSource = null;
                                    lblTotalCash.Text = "TOTAL CASH: " + "₹ 0.00";
                                    lblTotalEPay.Text = "TOTAL EPAY: " + "₹ 0.00";
                                }
                            }
                        }
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "GetRecentCheckOuts");
            }
        }
        private void BtnTwoWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                objFilter.VehicleTypeCode = "2W";
                btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                GetRecentCheckOuts();
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnTwoWheeler_Clicked");
            }
        }
        private void BtnFourWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                objFilter.VehicleTypeCode = "4W";
                btnFourWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                GetRecentCheckOuts();
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnFourWheeler_Clicked");
            }
        }
        private void BtnIns_Clicked(object sender, EventArgs e)
        {
            try
            {
                objFilter.Ins = true;
                objFilter.Outs = false;
                btnIns.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                btnOuts.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                GetRecentCheckOuts();
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnIns_Clicked");
            }
        }
        private void BtnOuts_Clicked(object sender, EventArgs e)
        {
            try
            {
                objFilter.Outs = true;
                objFilter.Ins = false;
                btnOuts.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                btnIns.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                GetRecentCheckOuts();

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnOuts_Clicked");
            }
        }
        private void lvCheckInChkOutReport_Refreshing(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    btnIns.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnOuts.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    objFilter.VehicleTypeCode = "";
                    objFilter.Ins = true;
                    objFilter.Outs = false;
                    GetRecentCheckOuts();
                    lvCheckInChkOutReport.IsRefreshing = false;
                }
                else
                {
                    lvCheckInChkOutReport.IsRefreshing = false;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void SwitchViolation_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (switchViolation.IsToggled)
                {
                    if (VMRecentCheckOutsID.Count >= 1)
                    {
                        var lstviolations = VMRecentCheckOutsID.Where(i => (i.ApplicationTypeID.ApplicationTypeCode.ToUpper().Contains("V")) || (i.StatusID.StatusCode.ToUpper().Contains("FOC")));
                        if (lstviolations.Count() > 0)
                        {
                            lvCheckInChkOutReport.ItemsSource = lstviolations;
                        }
                        else
                        {
                            lvCheckInChkOutReport.ItemsSource = null;
                        }

                    }
                }
                else
                {
                    lvCheckInChkOutReport.ItemsSource = VMRecentCheckOutsID;
                }


            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "HistoryPage.xaml.cs", "", "SwitchViolation_Toggled");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutRecentCheckOutPage.Opacity = 0.5;
            }
            else
            {
                absLayoutRecentCheckOutPage.Opacity = 1;
            }
        }

    }
}