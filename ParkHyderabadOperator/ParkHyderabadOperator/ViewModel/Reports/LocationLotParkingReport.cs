using ParkHyderabadOperator.DAL.DALReport;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParkHyderabadOperator.ViewModel.Reports
{
    public class LocationLotParkingReport
    {
        VMLocationLotPassReport objVMVehiclePassReport = null;
        VMLocationLotViolations objVMClampedVehiclesSummary = null;
        StationVehicles objStationVehicleReport = null;
        public bool IsRefreshList { get; set; }
        public string LotTotalCheckIn { get; set; }
        public string LotTotalCheckOut { get; set; }
        public string LotTotalFOC { get; set; }
        public string LotRevenueCash { get; set; }
        public string LotRevenueEpay { get; set; }
        public ObservableCollection<LotParkingReport> LotParkingReportList { get; set; }
        public LotParkingReport PreviousSelectedRowItem { get; set; }
        private LotParkingReport _selectedRowItem { get; set; }
        public LotParkingReport SelectedRowItem
        {
            get { return _selectedRowItem; }
            set
            {
                if (_selectedRowItem != value)
                {
                    _selectedRowItem = value;
                    ExpandOrCollapseSelectedItem();
                }
            }
        }
        private void ExpandOrCollapseSelectedItem()
        {

            if (PreviousSelectedRowItem != null || SelectedRowItem == null)
            {
                LotParkingReportList.Where(t => t.Id == PreviousSelectedRowItem.Id).FirstOrDefault().IsVisible = false;
                LotParkingReportList.Where(t => t.Id == PreviousSelectedRowItem.Id).FirstOrDefault().SelectedImageType = "plus.png";
            }

            LotParkingReportList.Where(t => t.Id == SelectedRowItem.Id).FirstOrDefault().IsVisible = true;

            PreviousSelectedRowItem = SelectedRowItem;


        }
        public LocationLotParkingReport(string apitoken,User objLoginUser)
        {

            SelectedRowItem = null;
            LotParkingReportList = null;
            try
            {
                DALReport dal_Report = new DALReport();
                VMReportSummary result = dal_Report.GetLocationLotReport(apitoken, objLoginUser);
                var VMLocationLotParkingReportID = result.VMLocationLotParkingReportID;

                #region Parking Report
                if (VMLocationLotParkingReportID != null)
                {
                    LotTotalCheckIn = VMLocationLotParkingReportID.LotTotalCheckIn;
                    LotTotalCheckOut = VMLocationLotParkingReportID.LotTotalCheckOut;
                    LotTotalFOC = VMLocationLotParkingReportID.LotTotalFOC;
                    LotRevenueCash = VMLocationLotParkingReportID.LotRevenueCash;
                    LotRevenueEpay = VMLocationLotParkingReportID.LotRevenueEpay;
                    LotParkingReportList = VMLocationLotParkingReportID.LotParkingReportList;
                }


                #endregion

                #region Pass Report

                objVMVehiclePassReport = result.VMLocationLotPassReportID;

                #endregion

                #region Violation Report

                objVMClampedVehiclesSummary = result.VMLocationLotViolationsID;

                #endregion

                #region Total Cash and Epay
                objStationVehicleReport = new StationVehicles();
                objStationVehicleReport.StationVehicleCash = result.Cash;
                objStationVehicleReport.StationVehicleEPay = result.EPay;
                #endregion

            }
            catch (Exception ex)
            {
            }
        }
        public void GetLotParkingRevenueList(string apitoken)
        {
            
        }
        public VMLocationLotPassReport GetLocationLotPassesReport()
        {

            return objVMVehiclePassReport;
        }
        public VMLocationLotViolations GetLocationLotViolationReport()
        {
            return objVMClampedVehiclesSummary;
        }
        public StationVehicles GetLocationLotTotalRevenue()
        {
            return objStationVehicleReport;
        }
    }
    public class LotParkingReport : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public LotParkingReport()
        {
            LocationLotOperations = new List<LocationLotOperations>();
        }
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string TotalIn { get; set; }
        public string TotalOut { get; set; }
        public List<LocationLotOperations> LocationLotOperations { get; set; }
        public string Currency { get; set; }
        public string TotalFOC { get; set; }
        public string TotalCash { get; set; }
        public string TotalEpay { get; set; }
        public string OtherIn { get; set; }
        public string OtherOut { get; set; }
        public string _selectedImageType { get; set; }
        public string SelectedImageType { get; set; }

        private bool _isExpandVisible { get; set; }
        private bool _isVisible { get; set; }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isExpandVisible = value;
                    _isVisible = value;

                    OnPropertyChanged();
                }

            }
        }
    }
}
