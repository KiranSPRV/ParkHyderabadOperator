using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALMenuBar;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel.Reports;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeSheet : ContentPage
    {
        DALMenubar dal_menubar = null;
        DALExceptionManagment dal_Exceptionlog = null;
        public TimeSheet()
        {
            InitializeComponent();
            dal_menubar = new DALMenubar();
            btnPreviousMonth.Text = DateTime.Now.AddMonths(-1).ToString("MMMM").ToUpper();
            btnCurrentMonth.Text = DateTime.Now.ToString("MMMM").ToUpper();
        }
        private void BtnPreviousMonth_Clicked(object sender, EventArgs e)
        {
            btnPreviousMonth.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
            btnCurrentMonth.Style = (Style)App.Current.Resources["ButtonRegularWhiteStyle"];
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            GetUserTimeSheet(firstDayOfMonth, firstDayOfMonth.AddMonths(1).AddDays(-1));
        }
        private void BtnCurrentMonth_Clicked(object sender, EventArgs e)
        {
            btnPreviousMonth.Style = (Style)App.Current.Resources["ButtonRegularWhiteStyle"];
            btnCurrentMonth.Style = (Style)App.Current.Resources["ButtonSubmitStyle"];
            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime presentDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
         
            GetUserTimeSheet(firstDayOfMonth, presentDayOfMonth);
        }
        private async void GetUserTimeSheet(DateTime selectedMonth,DateTime lastdate)
        {
            ShowLoading(true);
            VMUserDailyLogin objvmuserlogin = null;
            DateTime firstDayOfMonth = selectedMonth;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    UserDailyLogin objdailylogin = new UserDailyLogin();
                    objdailylogin.UserID.UserID = objloginuser.UserID;
                    objdailylogin.LocationParkingLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    objdailylogin.HistoryFromDate = firstDayOfMonth;
                    objdailylogin.HistoryToDate = lastdate;
                    await Task.Run(() =>
                    {
                        objvmuserlogin = dal_menubar.GetUserDailyLoginHistory(Convert.ToString(App.Current.Properties["apitoken"]), objdailylogin);
                    });
                    if (objvmuserlogin != null && objvmuserlogin.UserDailyLoginID.Count > 0)
                    {
                        lvTimeSheetSummary.ItemsSource = objvmuserlogin.UserDailyLoginID;
                        spanWorkedDays.Text = Convert.ToString(objvmuserlogin.WorkedDays);
                        spanAbsentDays.Text = Convert.ToString(objvmuserlogin.AbsentDays);
                        spanTotalHours.Text = objvmuserlogin.TotalHours;
                        spanOperatorName.Text = objloginuser.UserName + "-#" + objloginuser.UserCode;
                        spanSupervisorName.Text = objvmuserlogin.SuperVisorName;
                    }
                    else
                    {
                        lvTimeSheetSummary.ItemsSource = null;
                        spanWorkedDays.Text = spanAbsentDays.Text= spanTotalHours.Text = spanOperatorName.Text="";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "TimeSheet.xaml.cs", "", "GetUserTimeSheet");
            }
            ShowLoading(false);
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutTimeSheetpage.Opacity = 0.5;
            }
            else
            {
                absLayoutTimeSheetpage.Opacity = 1;
            }

        }
    }
}