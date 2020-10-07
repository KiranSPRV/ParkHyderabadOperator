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

        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        DALExceptionManagment dal_Exceptionlog;
        List<VMLocationLots> lstlots = null;
        List<User> lstOperators = null;
        LocationLotParkingReport objlot = null;
        User objReportUser = null;
       
        public ReportPage()
        {
            InitializeComponent();
            lstlots = new List<VMLocationLots>();
            fmReportPrint.IsVisible = false;
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "LoadLocationLotActiveOperators");
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "PickerLocationLot_SelectedIndexChanged");
            }
        }

        #endregion
        private void ButtonGenerateReport_Clicked(object sender, EventArgs e)
        {
            GenerateAppReport();
            fmReportPrint.IsVisible = true;
        }
        public async void GenerateAppReport()
        {
            try
            {

                objlot = null;
                if (DeviceInternet.InternetConnected())
                {

                    objReportUser = new User();
                    VMLocationLots objselectedlot = (VMLocationLots)pickerLocationLot.SelectedItem;
                    if (objselectedlot != null)
                    {
                        objReportUser.LocationParkingLotID.LocationID.LocationID = objselectedlot.LocationID;
                        objReportUser.LocationParkingLotID.LocationParkingLotID = objselectedlot.LocationParkingLotID;
                        objReportUser.LocationParkingLotID.LocationParkingLotName = objselectedlot.LocationParkingLotName;
                        objReportUser.LocationParkingLotID.LocationID.LocationName = objselectedlot.LocationName;

                        if (pickerOperator.SelectedItem != null)
                        {
                            var objselectedOperator = (User)pickerOperator.SelectedItem;
                            if(objselectedOperator.UserCode.Contains("ALL"))
                            {
                                var userobj = (User)App.Current.Properties["LoginUser"];
                                objReportUser.UserID =0;
                                objReportUser.UserCode = userobj.UserCode;
                            }
                            else
                            {
                                objReportUser.UserID = objselectedOperator.UserID;
                                objReportUser.UserCode = objselectedOperator.UserCode;
                            }
                           
                        }
                        else 
                        {
                            var userobj = (User)App.Current.Properties["LoginUser"];
                            objReportUser.UserID = userobj.UserID;
                            objReportUser.UserCode = userobj.UserCode;
                        }

                        if (objselectedlot.LocationID == 0 && objselectedlot.LocationParkingLotID == 0 && objselectedlot.LotName == "ALL")
                        {
                            var userobj = (User)App.Current.Properties["LoginUser"];
                            objReportUser.UserID = userobj.UserID;
                            objReportUser.UserCode = "ALL";
                        }
                    }
                    ShowLoading(true);
                    await Task.Run(() =>
                    {
                        if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                        {

                            DateTime fromDate = new DateTime(datePickerFromDate.Date.Year, datePickerFromDate.Date.Month, datePickerFromDate.Date.Day, datePickerFromTime.Time.Hours, datePickerFromTime.Time.Minutes, 0);
                            objReportUser.LoginTime = fromDate;
                            objReportUser.LogoutTime = new DateTime(datePickerToDate.Date.Year, datePickerToDate.Date.Month, datePickerToDate.Date.Day, datePickerToTime.Time.Hours, datePickerToTime.Time.Minutes, 0); ;
                            objlot = new LocationLotParkingReport(Convert.ToString(App.Current.Properties["apitoken"]), objReportUser);

                        }
                    });
                    if (objlot != null)
                    {
                        lvSummaryReports.BindingContext = objlot;
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
        private async void ButtonReportPrint_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (objlot != null)
                {
                    if (DeviceInternet.InternetConnected())
                    {

                        ReportPrint(objlot);
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please check your internet.", "Ok");
                        ShowLoading(false);
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please generate report.", "Ok");
                    ShowLoading(false);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "ButtonReportPrint_Clicked");
            }
        }
        public async void ReportPrint(LocationLotParkingReport objPrintReport)
        {
            try
            {

                ShowLoading(true);
                string printerName = string.Empty;
                await Task.Run(() =>
                {
                    printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();
                    if (printerName != string.Empty && printerName != "")
                    {
                        List<string> lstPritnText = new List<string>();
                        string reportDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                        lstPritnText.Add("\x1B\x21\x19" + "Parking: " + objReportUser.LocationParkingLotID.LocationParkingLotName + "\x1B\x21\x00" + "\n\n");
                        lstPritnText.Add("\x1B\x21\x09" + "User :" + objReportUser.UserCode + "\x1B\x21\x00" + "\n\n");
                        lstPritnText.Add("\x1B\x21\x07" + "   " + reportDate + "\x1B\x21\x00\n\n");
                        lstPritnText.Add("\x1B\x21\x07" + "From :" + objReportUser.LoginTime + "\x1B\x21\x00\n");
                        lstPritnText.Add("\x1B\x21\x01" + "  To :" + objReportUser.LogoutTime + "\x1B\x21\x00\n\n");

                        if (objPrintReport.LotParkingReportList.Count > 0)
                        {
                            foreach (var item in objPrintReport.LotParkingReportList)
                            {
                                if (item.VehicleType.ToUpper() == "2W")
                                {
                                    lstPritnText.Add("\x1B\x21\x20" + "BIKE " + "\x1B\x21\x00" + "\n");
                                    lstPritnText.Add("\x1B\x21\x07" + " Total CheckIn :" + item.TotalIn + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "Total CheckOut :" + item.TotalOut + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "           FOC :" + item.TotalFOC + "\x1B\x15\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "          Cash :" + item.TotalCash + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "          EPay :" + item.TotalEpay + "\x1B\x21\x00\n");
                                    lstPritnText.Add("" + "\n");
                                    lstPritnText.Add("" + "\n");
                                }
                                if (item.VehicleType.ToUpper() == "4W")
                                {
                                    lstPritnText.Add("\x1B\x21\x20" + "CAR " + "\x1B\x21\x00" + "\n");
                                    lstPritnText.Add("\x1B\x21\x07" + " Total CheckIn :" + item.TotalIn + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "Total CheckOut :" + item.TotalOut + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "           FOC :" + item.TotalFOC + "\x1B\x15\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "          Cash :" + item.TotalCash + "\x1B\x21\x00\n");
                                    lstPritnText.Add("\x1B\x21\x07" + "          EPay :" + item.TotalEpay + "\x1B\x21\x00\n");
                                    lstPritnText.Add("" + "\n");
                                    lstPritnText.Add("" + "\n");
                                }
                            }
                        }

                        if (objPrintReport.GetLocationLotPassesReport() != null)
                        {
                            var passesreport = objPrintReport.GetLocationLotPassesReport();
                            lstPritnText.Add("\x1B\x21\x20" + "PASS " + "\x1B\x21\x00" + "\n");
                            lstPritnText.Add("\x1B\x21\x07" + "Total Sold :" + passesreport.TotalSold+ "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + " Total NFC :" + passesreport.TotalNFC + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "      Cash :" +Convert.ToDecimal(passesreport.TotalCash).ToString("N2") + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "      EPay :" + passesreport.TotalEPay.ToString("N2") + "\x1B\x21\x00\n");
                            lstPritnText.Add("" + "\n");
                            lstPritnText.Add("" + "\n");
                        }

                        if (objPrintReport.GetLocationLotViolationReport() != null)
                        {
                            var violationreport = objPrintReport.GetLocationLotViolationReport();
                            lstPritnText.Add("\x1B\x21\x20" + "Violation " + "\x1B\x21\x00" + "\n");
                            lstPritnText.Add("\x1B\x21\x07" + "Warning Clamp :" + violationreport.TotalWarningClamps + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "       UnPaid :" + violationreport.TotalUnPaidClamps + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "         Paid :" + violationreport.TotalPaidClamps + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "         Cash :" + violationreport.TotalCash.ToString("N2") + "\x1B\x21\x00\n");
                            lstPritnText.Add("\x1B\x21\x07" + "         EPay :" + violationreport.TotalEPay.ToString("N2") + "\x1B\x21\x00\n");
                            lstPritnText.Add("" + "\n");
                            lstPritnText.Add("" + "\n");
                        }
                        var objStationVehicles = objlot.GetLocationLotTotalRevenue();
                        lstPritnText.Add("\x1B\x21\x07" + "      Total Cash :" + objStationVehicles.StationVehicleCash.ToString("N2") + "\x1B\x21\x00\n");
                        lstPritnText.Add("\x1B\x21\x07" + "      Total EPay :" + objStationVehicles.StationVehicleEPay.ToString("N2") + "\x1B\x21\x00\n");
                        lstPritnText.Add("\x1B\x21\x08" + "     Total Amount :" + (objStationVehicles.StationVehicleCash + objStationVehicles.StationVehicleEPay).ToString("N2") + "\x1B\x21\x00\n\n");
                        lstPritnText.Add("" + "\n\n");
                        lstPritnText.Add("" + "\n\n");

                        if (lstPritnText.Count > 0)
                        {
                            for (var l = 0; l < lstPritnText.Count; l++)
                            {
                                string printtext = lstPritnText[l].ToString();
                                if (printtext != "")
                                {

                                    ObjblueToothDevicePrinting.PrintCommand(printerName, printtext);
                                }

                            }
                        }
                    }
                });
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ReportPage.xaml.cs", "", "ReportPrint");
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