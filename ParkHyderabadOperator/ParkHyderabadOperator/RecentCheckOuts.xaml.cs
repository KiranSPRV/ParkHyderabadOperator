﻿using ParkHyderabadOperator.DAL.DALExceptionLog;
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
            LoadCheckOutPickerDays();
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
                    var objLoginUser = (User)App.Current.Properties["LoginUser"];
                    lstlots = dal_Home.GetUserAllocatedLocationAndLots(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);
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

                    await DisplayAlert("Alert", "Token details  unavailable", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "PickerLocationLot_SelectedIndexChanged");
            }
        }
        private void LoadLocationLotActiveOperators(VMLocationLots objVMLocationLot, int UserID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    User objselectedlocationlot = new User();
                    objselectedlocationlot.UserID = UserID;
                    objselectedlocationlot.LocationParkingLotID.LocationID.LocationID = objVMLocationLot.LocationID;
                    objselectedlocationlot.LocationParkingLotID.LocationParkingLotID = objVMLocationLot.LocationParkingLotID;
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
        #endregion

        #region Picker Day
        public void LoadCheckOutPickerDays()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALMenubar dal_menubar = new DALMenubar();
                    var ddlcheckOuts = dal_menubar.GetRecentCheckOutDays();
                    if (ddlcheckOuts.Count > 0)
                    {
                        pickerDay.ItemsSource = ddlcheckOuts;

                    }



                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "LoadLoginUserLocationLots");
            }
        }
        private void PickerDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (objFilter != null)
                {
                    if (objFilter.Ins == false && objFilter.Outs == false)
                    {
                        objFilter.Outs = true;
                        btnOuts.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    }


                    GetRecentCheckOuts();

                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnIns_Clicked");
            }

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
                            var objSelectedLocation = (VMLocationLots)pickerLocationLot.SelectedItem;

                            var objselectedOperator = (User)pickerOperator.SelectedItem;
                            var objRecentCheckOutDays = (RecentCheckOutDays)pickerDay.SelectedItem;
                            objFilter.LocationID = objSelectedLocation.LocationID;
                            objFilter.LocationParkingLotID = objSelectedLocation.LocationParkingLotID;
                            objFilter.UserID = objselectedOperator.UserID;

                            if (objRecentCheckOutDays.Day == "Today")
                            {
                                objFilter.SelectedDay = DateTime.Now;
                            }
                            if (objRecentCheckOutDays.Day == "Yesterday")
                            {
                                objFilter.SelectedDay = DateTime.Now.AddDays(-1);
                            }
                            if (objRecentCheckOutDays.Day == "Daybefore Yesterday")
                            {
                                objFilter.SelectedDay = DateTime.Now.AddDays(-2);
                            }
                            await Task.Run(() =>
                            {

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
                                    if (switchViolation.IsToggled)
                                    {
                                        if (VMRecentCheckOutsID.Count >= 1)
                                        {
                                            var lstviolations = VMRecentCheckOutsID.Where(i => (i.StatusID.StatusCode.ToUpper().Contains("FOC")));
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


                                }

                            }
                        }
                    }
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "GetRecentCheckOuts");
                ShowLoading(false);

            }
        }
        private async void BtnTwoWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.VehicleTypeCode = "2W";
                    btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnThreeWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnHeavyWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnTwoWheeler_Clicked");
            }
        }
        private async void BtnFourWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.VehicleTypeCode = "4W";
                    btnFourWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnThreeWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnHeavyWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnFourWheeler_Clicked");
            }
        }
        private async void btnThreeWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.VehicleTypeCode = "3W";
                    btnThreeWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnHeavyWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnFourWheeler_Clicked");
            }
        }

        private async void btnHeavyWheeler_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.VehicleTypeCode = "HW";
                    btnHeavyWheeler.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnThreeWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnFourWheeler_Clicked");
            }
        }
        private async void BtnIns_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.Ins = true;
                    objFilter.Outs = false;
                    btnIns.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnOuts.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();

                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xamls.cs", "", "BtnIns_Clicked");
            }
        }
        private async void BtnOuts_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (pickerDay.SelectedItem != null)
                {
                    objFilter.Outs = true;
                    objFilter.Ins = false;
                    btnOuts.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
                    btnIns.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                    GetRecentCheckOuts();
                }
                else
                {
                    await DisplayAlert("Alert", "Please select Day", "Ok");
                }
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

                    if (objFilter != null)
                    {
                        objFilter.VehicleTypeCode = "";
                        btnFourWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                        btnTwoWheeler.Style = (Style)App.Current.Resources["ButtonRegularMercuryStyle"];
                        GetRecentCheckOuts();
                    }
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
        private async void LvCheckInChkOutReport_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);

                    if (e.SelectedItem == null) return;
                    RecentCheckOutDetailsPage objrecentChekcOutPage = null;
                    VMRecentCheckOuts objVMRecentCheckOuts = (VMRecentCheckOuts)e.SelectedItem;
                    if (objVMRecentCheckOuts.CustomerParkingSlotID != 0)
                    {
                        await Task.Run(() =>
                        {
                            objrecentChekcOutPage = new RecentCheckOutDetailsPage(objVMRecentCheckOuts.CustomerParkingSlotID);
                        });
                        await Navigation.PushAsync(objrecentChekcOutPage);
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Vehicle details unvailable,Please contact Admin", "Ok");
                    }
                    try
                    {
                        if (((ListView)lvCheckInChkOutReport).SelectedItem != null)
                        {
                            ((ListView)lvCheckInChkOutReport).SelectedItem = null;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    ShowLoading(false);
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                }

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "RecentCheckOuts.xaml.cs", "", "LvCheckInChkOutReport_ItemSelected");
            }
            ShowLoading(false);
        }
        private async void SwitchViolation_Toggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (switchViolation.IsToggled)
                {
                    if (objFilter != null && objFilter.Outs == true)
                    {
                        if (switchViolation.IsToggled)
                        {
                            if (VMRecentCheckOutsID.Count >= 1)
                            {
                                var lstviolations = VMRecentCheckOutsID.Where(i => (i.StatusID.StatusCode.ToUpper().Contains("FOC")));
                                if (lstviolations.Count() > 0)
                                {
                                    lvCheckInChkOutReport.ItemsSource = lstviolations;
                                }
                                else
                                {
                                    VMRecentCheckOutsID = null;
                                    lvCheckInChkOutReport.ItemsSource = VMRecentCheckOutsID;
                                }

                            }
                        }
                        else
                        {
                            lvCheckInChkOutReport.ItemsSource = VMRecentCheckOutsID;
                        }
                    }
                    else
                    {
                        switchViolation.IsToggled = false;
                        await DisplayAlert("Alert", "Please click Outs for FOC records", "Ok");
                    }
                }
                else
                {
                    GetRecentCheckOuts();
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