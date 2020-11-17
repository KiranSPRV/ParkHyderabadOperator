using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel.VMHome;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailHomePage : ContentPage
    {

        DALExceptionManagment dal_Exceptionlog;
        public bool IsPresented { get; private set; }
        bool isAppearing = false;
        List<LocationLotParkedVehicles> lstdayvehicles = null;
        List<VMLocationLots> lstlots = null;
        bool IsTodayHoliday = false;
        public string todayLotOpenTime = string.Empty;
        public string todayLotCloseTime = string.Empty;

        #region NFC Card Properties
        public const string ALERT_TITLE = "NFC";
        public const string MIME_TYPE = "application/com.companyname.nfcsample";
        NFCNdefTypeFormat _type;
        bool _makeReadOnly = false;

        public bool NfcIsEnabled { get; set; }
        public bool NfcIsDisabled => !NfcIsEnabled;
        #endregion

        public MasterDetailHomePage()
        {
            InitializeComponent();
            LoadLoginUserLocationLots();
            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                LoadParkedVehicle(null);
                LstVWParkingVehicle.IsRefreshing = false;
                return true;
            });
        }
        protected override void OnAppearing()
        {
            try
            {
                if (!isAppearing)
                {
                    try
                    {
                        CheckNFCSupported();
                        SubscribeEvents();
                        isAppearing = true;
                    }
                    catch (Exception ex)
                    {
                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "CheckNFCSupported");
                    }
                }

            }
            catch (Exception ex)
            {
                //  await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        public MasterDetailHomePage(ParkedVehiclesFilter lstFilters)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            LoadLoginUserLocationLots();
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    LoadParkedVehicle(lstFilters);
                }

            }
            catch (Exception ex)
            {

            }
            try
            {
                CheckNFCSupported();
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "CheckNFCSupported");
                DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }

        #region Picker Location Lot
        private async void LoadLoginUserLocationLots()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();
                    User objLoginUser = (User)App.Current.Properties["LoginUser"];
                    lstlots = dal_Home.GetUserAllocatedLocationAndLots(Convert.ToString(App.Current.Properties["apitoken"]), objLoginUser);
                    if (lstlots.Count > 0)
                    {
                        pickerLocationLot.ItemsSource = lstlots;
                        for (int x = 0; x < lstlots.Count; x++)
                        {
                            if (objLoginUser.LocationParkingLotID.LocationParkingLotID == null || objLoginUser.LocationParkingLotID.LocationParkingLotID == 0)
                            {
                                if (lstlots[x].LocationID == objLoginUser.LocationParkingLotID.LocationID.LocationID)
                                {
                                    IsTodayHoliday = lstlots[x].IsActive;
                                    todayLotOpenTime = lstlots[x].LotOpenTime;
                                    todayLotCloseTime = lstlots[x].LotCloseTime;
                                    pickerLocationLot.SelectedIndex = x;

                                }
                            }
                            else
                            {
                                if (lstlots[x].LocationParkingLotID == objLoginUser.LocationParkingLotID.LocationParkingLotID)
                                {
                                    IsTodayHoliday = lstlots[x].IsActive;
                                    todayLotOpenTime = lstlots[x].LotOpenTime;
                                    todayLotCloseTime = lstlots[x].LotCloseTime;
                                    pickerLocationLot.SelectedIndex = x;
                                   
                                }
                            }

                        }
                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Login user details are not able to find,Please try to relogin.", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MasterDetailHomePage.xaml.cs", "", "LoadLoginUserLocationLots");
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
                        if (objVMLocations != null)
                        {
                            string lotName = objVMLocations.LocationParkingLotName;
                            ParkedVehiclesFilter objloclot = new ParkedVehiclesFilter();
                            objloclot.LocationParkingLotID = objVMLocations.LocationParkingLotID;
                            objloclot.LocationID = objVMLocations.LocationID;
                            objLoginUser.LocationParkingLotID.LocationParkingLotID = objVMLocations.LocationParkingLotID;
                            objLoginUser.LocationParkingLotID.LocationParkingLotName = objVMLocations.LotName;
                            objLoginUser.LocationParkingLotID.LocationID.LocationID = objVMLocations.LocationID;
                            objLoginUser.LocationParkingLotID.LocationID.LocationName = objVMLocations.LocationName;
                            IsTodayHoliday = objVMLocations.IsActive;
                            todayLotOpenTime = objVMLocations.LotOpenTime;
                            todayLotCloseTime = objVMLocations.LotCloseTime;
                            LoadParkedVehicle(objloclot);
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MasterDetailHomePage.xaml.cs", "", "LoadLoginUserLocationLots");
            }

        }
        #endregion

        #region List View Parked Vehicles
        public void LoadParkedVehicle(ParkedVehiclesFilter objinput)
        {
            try
            {
                LstVWParkingVehicle.ItemsSource = null;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    DALHome dal_Home = new DALHome();

                    if (objinput == null)
                    {
                        User objLoginUser = (User)App.Current.Properties["LoginUser"];
                        objinput = new ParkedVehiclesFilter();

                        objinput.LocationParkingLotID = objLoginUser.LocationParkingLotID.LocationParkingLotID;
                        objinput.LocationID = objLoginUser.LocationParkingLotID.LocationID.LocationID;
                    }

                    VMLocationLotParkedVehicles vmVehicles = dal_Home.GetAllParkedVehicles(Convert.ToString(App.Current.Properties["apitoken"]), objinput);
                    lstdayvehicles = vmVehicles.CustomerParkingSlotID;
                    LstVWParkingVehicle.ItemsSource = vmVehicles.CustomerParkingSlotID;
                    labelTotalTwoWheeler.Text = Convert.ToString(vmVehicles.TotalTwoWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutTwoWheeler) + ")";
                    labelTotalFourWheeler.Text = Convert.ToString(vmVehicles.TotalFourWheeler) + "(" + Convert.ToString(vmVehicles.TotalOutFourWheeler) + ")";
                }

            }
            catch (Exception ex)
            {

            }
        }
        private void SrbSearchVehicle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowLoading(true);
                if (lstdayvehicles.Count > 0)
                {
                    if (!string.IsNullOrEmpty(e.NewTextValue))
                    {
                        LstVWParkingVehicle.ItemsSource = lstdayvehicles.Where(x => x.RegistrationNumber.ToUpper().Contains(e.NewTextValue.ToUpper()));
                    }
                    else
                    {
                        LstVWParkingVehicle.ItemsSource = lstdayvehicles;
                    }
                }

                ShowLoading(false);

            }
            catch (Exception ex) { }
        }
        private async void LstVWParkingVehicle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    ShowLoading(true);

                    ViolationVehicleInformation violationPageInfo = null;
                    OverstayVehicleInformation overstayPageInfo = null;
                    PassCheckInVehicleInformation chekInPageInfo = null;

                    if (e.SelectedItem == null) return;
                    LocationLotParkedVehicles objSelecteditem = (LocationLotParkedVehicles)e.SelectedItem;

                    if (objSelecteditem.StatusCode == "V" || objSelecteditem.StatusCode == "C")
                    {
                        StopNFCListening();
                        if (objSelecteditem.CustomerParkingSlotID != 0)
                        {
                            await Task.Run(() =>
                            {
                                violationPageInfo = new ViolationVehicleInformation(objSelecteditem.CustomerParkingSlotID);
                            });
                            await Navigation.PushAsync(violationPageInfo);
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Selected vehicle details are unable to get,Please contact admin.", "Ok");
                        }

                    }
                    else if (objSelecteditem.StatusCode == "O")
                    {
                        StopNFCListening();
                        if (objSelecteditem.CustomerParkingSlotID != 0)
                        {

                            await Task.Run(() =>
                            {
                                overstayPageInfo = new OverstayVehicleInformation(objSelecteditem.CustomerParkingSlotID);
                            });
                            await Navigation.PushAsync(overstayPageInfo);



                        }
                        else
                        {
                            await DisplayAlert("Alert", "Selected vehicle details are unable to get,Please contact admin.", "Ok");
                        }
                    }
                    else if (objSelecteditem.StatusCode == "P" || objSelecteditem.StatusCode == "A" || objSelecteditem.StatusCode == "CHKIN" || objSelecteditem.StatusCode == "G")
                    {
                        StopNFCListening();
                        if (objSelecteditem.CustomerParkingSlotID != 0)
                        {
                            await Task.Run(() =>
                            {
                                chekInPageInfo = new PassCheckInVehicleInformation(objSelecteditem.CustomerParkingSlotID);
                            });
                            await Navigation.PushAsync(chekInPageInfo);
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Selected vehicle details are unable to get,Please contact admin.", "Ok");
                        }
                    }
                    try
                    {
                        if (((ListView)LstVWParkingVehicle).SelectedItem != null)
                        {
                            ((ListView)LstVWParkingVehicle).SelectedItem = null;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    ShowLoading(false);
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MasterDetailHomePage.xaml.cs", "", "LstVWParkingVehicle");
            }
            ShowLoading(false);

        }
        private void LstVWParkingVehicle_Refreshing(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    LoadParkedVehicle(null);
                    LstVWParkingVehicle.IsRefreshing = false;
                }
                else
                {
                    LstVWParkingVehicle.IsRefreshing = false;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Filter
        private async void ToolbarFilter_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DeviceInternet.InternetConnected())
                {
                    StopNFCListening();
                    var filtersPage = new FiltersPage();
                    await Navigation.PushAsync(filtersPage);
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
        }




        #endregion

        #region Footer Buttons
        private async void BtnCheckIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                BtnCheckIn.IsVisible = false;
                CheckIn checkInPage = null;
                if (DeviceInternet.InternetConnected())
                {
                    if (!IsTodayHoliday)
                    {
                        bool doesPageExists = Navigation.NavigationStack.Any(p => p is CheckIn);
                        if (!doesPageExists)
                        {
                            await Task.Run(() =>
                            {
                                StopNFCListening();
                                checkInPage = new CheckIn();
                            });
                            await Navigation.PushAsync(checkInPage);
                        }
                    }
                    else
                    {
                        ShowLoading(false);
                        BtnCheckIn.IsVisible = true;
                        await DisplayAlert("Alert", "Please check lot closed today.", "Ok");
                    }


                }
                else
                {
                    ShowLoading(false);
                    BtnCheckIn.IsVisible = true;
                    await DisplayAlert("Alert", "Please check your internet.", "Ok");

                }
                BtnCheckIn.IsVisible = true;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                BtnCheckIn.IsVisible = true;
                ShowLoading(false);
            }
        }
        private async void BtnPass_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                BtnPass.IsVisible = false;
                PassPage passPage = null;
                if (!IsTodayHoliday)
                {
                    bool doesPageExists = Navigation.NavigationStack.Any(p => p is PassPage);
                    if (!doesPageExists)
                    {
                        await Task.Run(() =>
                        {
                            StopNFCListening();
                            passPage = new PassPage();
                        });
                        await Navigation.PushAsync(passPage);
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check lot closed today.", "Ok");
                }
                BtnPass.IsVisible = true;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                BtnPass.IsVisible = true;
                ShowLoading(false);
            }
        }
        private async void BtnViolation_Clicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                BtnViolation.IsVisible = false;
                if (!IsTodayHoliday)
                {
                    var opentime=Convert.ToDateTime(todayLotOpenTime);
                    if (DateTime.Now> Convert.ToDateTime(todayLotOpenTime) && DateTime.Now<Convert.ToDateTime( todayLotCloseTime))
                    {
                        bool doesPageExists = Navigation.NavigationStack.Any(p => p is ViolationPage);
                        if (!doesPageExists)
                        {
                            ViolationPage violationPage = null;
                            await Task.Run(() =>
                            {
                                StopNFCListening();
                                violationPage = new ViolationPage();
                            });
                            await Navigation.PushAsync(violationPage);

                        }

                    }
                   else
                    {
                        await DisplayAlert("Alert", "Please check parking timings from " + todayLotOpenTime + " to " + todayLotCloseTime, "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please check lot closed today.", "Ok");
                }
                BtnViolation.IsVisible = true;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                BtnViolation.IsVisible = true;
                ShowLoading(false);
            }
        }
        #endregion
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

        #region NFC Card Related Code
        public void CheckNFCSupported()
        {
            try
            {
                if (CrossNFC.IsSupported)
                {
                    if (CrossNFC.Current.IsAvailable)
                    {
                        StartListeningIfNotiOS();
                    }
                }
                else
                {
                    DisplayAlert("Alert", "NFC is not supported,Please contact admin", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        void SubscribeEvents()
        {
            try
            {
                CrossNFC.Current.OnMessageReceived += CurrentHome_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
            }
            catch (Exception ex)
            {

            }
        }
        void UnsubscribeEvents()
        {
            try
            {

                CrossNFC.Current.OnMessageReceived -= CurrentHome_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;

            }
            catch (Exception ex)
            {

            }
        }
        void CurrentHome_OnMessageReceived(ITagInfo tagInfo)
        {
            string checkInmsg = string.Empty;
            try
            {
                if (tagInfo == null)
                {
                    DisplayAlert("Alert", "No tag found", "Ok");
                    return;
                }
                // Customized serial number
                var identifier = tagInfo.Identifier;
                var serialNumber = tagInfo.SerialNumber;
                if (serialNumber != "")
                {
                    DALCheckIn dal_ChckeIn = new DALCheckIn();
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        VehicleCheckIn objNFCCardVehicle = new VehicleCheckIn();
                        User objloginuser = (User)App.Current.Properties["LoginUser"];
                        objNFCCardVehicle.UserID = objloginuser.UserID;
                        objNFCCardVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                        objNFCCardVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                        objNFCCardVehicle.NFCCardNumber = serialNumber;
                        checkInmsg = dal_ChckeIn.SaveNFCCardVehicleCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objNFCCardVehicle);
                        if (checkInmsg != "Success")
                        {
                            DisplayAlert("Alert", checkInmsg, "Ok");

                        }
                        else
                        {
                            LoadParkedVehicle(null);
                        }


                    }
                    else
                    {

                        DisplayAlert("Alert", "NFC Card serialNumber unable to found.", "Ok");
                        return;

                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            try
            {
                if (!CrossNFC.Current.IsWritingTagSupported)
                {
                    DisplayAlert("Alert", "Writing tag is not supported on this device", "Ok");
                    return;
                }

                try
                {
                    NFCNdefRecord record = null;
                    switch (_type)
                    {
                        case NFCNdefTypeFormat.WellKnown:
                            record = new NFCNdefRecord
                            {
                                TypeFormat = NFCNdefTypeFormat.WellKnown,
                                MimeType = MIME_TYPE,
                                Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!")
                            };
                            break;
                        case NFCNdefTypeFormat.Uri:
                            record = new NFCNdefRecord
                            {
                                TypeFormat = NFCNdefTypeFormat.Uri,
                                Payload = NFCUtils.EncodeToByteArray("https://github.com/franckbour/Plugin.NFC")
                            };
                            break;
                        case NFCNdefTypeFormat.Mime:
                            record = new NFCNdefRecord
                            {
                                TypeFormat = NFCNdefTypeFormat.Mime,
                                MimeType = MIME_TYPE,
                                Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!")
                            };
                            break;
                        default:
                            break;
                    }

                    if (!format && record == null)
                        throw new Exception("Record can't be null.");

                    tagInfo.Records = new[] { record };

                    if (format)
                        CrossNFC.Current.ClearMessage(tagInfo);
                    else
                    {
                        CrossNFC.Current.PublishMessage(tagInfo, _makeReadOnly);
                    }
                }
                catch (System.Exception ex)
                {
                    DisplayAlert("Alert", ex.Message, "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        /// <summary>
        /// Task to start listening for NFC tags if the user's device platform is not iOS
        /// </summary>
        /// <returns>Task to be performed</returns>
        void StartListeningIfNotiOS()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    return;
                }
                else
                {
                    CrossNFC.Current.StartListening();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void StopNFCListening()
        {
            #region  NFC Code 
            try
            {
                isAppearing = false;
                CrossNFC.Current.StopListening();
                UnsubscribeEvents();

            }
            catch (Exception ex)
            {

            }

            #endregion

        }
        #endregion
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}