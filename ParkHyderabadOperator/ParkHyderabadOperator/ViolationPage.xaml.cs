using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALHome;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel.VMPass;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViolationPage : ContentPage
    {
        DALExceptionManagment dal_Exceptionlog;
        DALViolationandClamp dal_ViolationClamp;
        DALCheckIn dal_DALCheckIn;
        CustomerVehiclePass objCustomerPass = null;
        BlueToothDevicePrinting ObjblueToothDevicePrinting;
        List<ParkingBay> lstparkingbay = null;
        private ObservableCollection<VehicleType> obsvehicleType = null;
        string fileName = string.Empty;
        string SelectedVehicle = string.Empty;
        byte[] imgCameraByteData = null;
        private float Latitude;
        private float Longitude;
        DateTime violationTime;
        public ViolationPage()
        {
            InitializeComponent();
            ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_ViolationClamp = new DALViolationandClamp();
            dal_DALCheckIn = new DALCheckIn();
            lstparkingbay = new List<ParkingBay>();
            btnCheckIn.IsVisible = false;
            checkBoxClampVehicle.IsChecked = true;
            chkWarning.IsVisible = true;
            chkWarning.IsChecked = true;

            LoadLocationBayNumbers();
            LoadGetViolationReasons();
            GetAllVehicleType();
        }
        protected async override void OnAppearing()
        {
            try
            {
                await GetCurrentLocation();
            }
            catch (Exception ex)
            {

            }
        }
        public void LoadLocationBayNumbers()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    lblcheckInLocation.Text = objloginuser.LocationParkingLotID.LocationParkingLotName;
                    lstparkingbay = dal_DALCheckIn.GetLocationParkingBay(Convert.ToString(App.Current.Properties["apitoken"]), objloginuser.LocationParkingLotID);
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "LoadLocationBayNumbers");
            }
        }
        private void PickerBayNumers_SelectedIndexChanged(object sender, EventArgs e)
        {

            ViolationVehicleValidations();
            violationTime = DateTime.Now;
        }
        public async void ViolationVehicleValidations()
        {
            try
            {
                chkWarning.IsChecked = true;
                if (entryRegistrationNumber.Text.Length >= 6 && SelectedVehicle != string.Empty)
                {
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        User objloginuser = (User)App.Current.Properties["LoginUser"];
                        objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), entryRegistrationNumber.Text, objloginuser.LocationParkingLotID.LocationID.LocationID, objloginuser.LocationParkingLotID.LocationParkingLotID, objloginuser.UserID, "");
                        if ((objCustomerPass.CustomerVehiclePassID != 0) && ((Convert.ToDateTime(objCustomerPass.StartDate).Date <= DateTime.Now.Date) && (Convert.ToDateTime(objCustomerPass.ExpiryDate).Date >= DateTime.Now.Date)) )
                        {
                            if (SelectedVehicle == objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper())
                            {
                                if ((objCustomerPass.PassPriceID.StationAccess == "All Stations" || objCustomerPass.PassPriceID.StationAccess == "All Station"))
                                {
                                    btnCheckIn.IsVisible = true;
                                    BtnViolation.IsVisible = false;
                                    await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                }
                                else if (objCustomerPass.IsMultiLot)
                                {
                                    DALHome dal_Home = new DALHome();
                                    List<VMMultiLocations> passLocations = dal_Home.GetAllPassLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), SelectedVehicle, objCustomerPass.CustomerVehiclePassID);
                                    var isvalid = passLocations.Where(p => p.LocationID == objloginuser.LocationParkingLotID.LocationID.LocationID);
                                    if (isvalid != null)
                                    {
                                        btnCheckIn.IsVisible = true;
                                        BtnViolation.IsVisible = false;
                                        await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                    }

                                }
                                else if (objCustomerPass.LocationID.LocationID == objloginuser.LocationParkingLotID.LocationID.LocationID)
                                {
                                    btnCheckIn.IsVisible = true;
                                    BtnViolation.IsVisible = false;
                                    await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
                                }
                            }
                            else
                            {
                                string passvehicletype = string.Empty;
                                btnCheckIn.IsVisible = true;
                                BtnViolation.IsVisible = false;
                                if (objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper() == "2W")
                                {
                                    passvehicletype = "two wheeler";
                                }
                                else if (objCustomerPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode.ToUpper() == "4W")
                                {
                                    passvehicletype = "four wheeler";
                                }
                                await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a " + passvehicletype + " pass,Please select valid vehicle type", "Ok");
                            }
                        }
                        else
                        {
                            btnCheckIn.IsVisible = false;
                            BtnViolation.IsVisible = true;
                        }
                        if (!IsVehicleViolationsCompleted())
                        {
                            slVehicleWarning.IsVisible = true;
                        }
                        else
                        {
                            slVehicleWarning.IsVisible = false;
                            chkWarning.IsChecked = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "ViolationVehicleValidations");
            }
        }
        private void LoadGetViolationReasons()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    pickerVehicleReason.ItemsSource = dal_ViolationClamp.GetViolationReasons(Convert.ToString(App.Current.Properties["apitoken"]), "V");
                }
            }
            catch (Exception ex) { }
        }
        private void CheckBoxClampVehicle_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {


        }
        private async void BtnCamera_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }
                else
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Custom,
                        Directory = "Violation",
                        SaveToAlbum = false,
                        CustomPhotoSize = 20,//Resize to 90% of original
                        CompressionQuality = 92
                    });
                    if (file != null)
                    {
                        if (file.Path.Contains("/"))
                        {
                            try
                            {
                                Stream objfilestream = file.GetStream();
                                string[] getfilename = file.Path.Split('/');
                                fileName = getfilename[getfilename.Length - 1];
                                var memoryStream = new MemoryStream();
                                file.GetStream().CopyTo(memoryStream);
                                imgCameraByteData = memoryStream.ToArray();
                                btnCamera.Source = file.Path;


                            }
                            catch (Exception ex)
                            {

                                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "Violation.xaml.cs", "", "BtnCamera_Clicked");
                                await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("No Camera", "Unable to open:", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "Violation.xaml.cs", "", "BtnCamera_Clicked");
                await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
            }
        }
        private async void BtnViolation_Clicked(object sender, EventArgs e)
        {
            string resultmsg = string.Empty;
            string existingCheckIn = string.Empty;
            MasterHomePage masterPage = null;
            BtnViolation.IsVisible = false;
            ShowLoading(true);
            try
            {
                int number;
                if (DeviceInternet.InternetConnected())
                {
                    if (SelectedVehicle != string.Empty)
                    {
                        if (entryRegistrationNumber.Text != null && (!entryRegistrationNumber.Text.Contains(" ") && (entryRegistrationNumber.Text.Length >= 6 && entryRegistrationNumber.Text.Length <= 10)))
                        {
                            string regNumber = entryRegistrationNumber.Text;
                            string regFormat = regNumber.Substring(regNumber.Length - 4);
                            if (int.TryParse(regFormat, out number))
                            {
                                if (pickerBayNumers.SelectedItem != null)
                                {
                                    if ((checkBoxClampVehicle.IsChecked) || (chkWarning.IsChecked))
                                    {
                                        if (pickerVehicleReason.SelectedItem != null)
                                        {
                                            ParkingBay objselectedbay = (ParkingBay)pickerBayNumers.SelectedItem;
                                            ViolationReason objselectedreason = (ViolationReason)pickerVehicleReason.SelectedItem;
                                            if (imgCameraByteData != null)
                                            {
                                                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                                                {
                                                    await Task.Run(() =>
                                                    {
                                                        existingCheckIn = VerifyVehicleCheckInStatus();
                                                        if (existingCheckIn == string.Empty)
                                                        {
                                                            ViolationAndClamp objViolationAndClamp = new ViolationAndClamp();
                                                            User objloginuser = (User)App.Current.Properties["LoginUser"];
                                                            objViolationAndClamp.UserID = objloginuser.UserID;
                                                            objViolationAndClamp.UserTypeID = objloginuser.UserTypeID.UserTypeID;
                                                            objViolationAndClamp.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                                            objViolationAndClamp.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                                            objViolationAndClamp.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                                            objViolationAndClamp.RegistrationNumber = entryRegistrationNumber.Text;
                                                            objViolationAndClamp.BayNumberID = objselectedbay.ParkingBayID;
                                                            objViolationAndClamp.BayNumber = objselectedbay.ParkingBayName;
                                                            objViolationAndClamp.IsClamp = checkBoxClampVehicle.IsChecked;
                                                            objViolationAndClamp.ReasonID = objselectedreason.ViolationReasonID;
                                                            objViolationAndClamp.ReasonName = objselectedreason.Reason;
                                                            objViolationAndClamp.VehicleTypeCode = SelectedVehicle;
                                                            DateTime openTime = DateTime.Parse(objloginuser.LocationParkingLotID.LotOpenTime);
                                                            if (DateTime.Now < openTime)
                                                            {
                                                                objViolationAndClamp.ViolationStartTime = openTime;
                                                                objViolationAndClamp.ViolationTime = openTime.ToString("MM/dd/yyyy HH:mm tt");
                                                            }
                                                            else
                                                            {
                                                                objViolationAndClamp.ViolationStartTime = violationTime;
                                                                objViolationAndClamp.ViolationTime = violationTime.ToString("MM/dd/yyyy HH:mm tt");
                                                            }

                                                            objViolationAndClamp.IsWarning = Convert.ToBoolean(chkWarning.IsChecked);
                                                            objViolationAndClamp.VehicleImageLottitude = Convert.ToDecimal(Latitude);
                                                            objViolationAndClamp.VehicleImageLongitude = Convert.ToDecimal(Longitude);
                                                            if (imgCameraByteData != null)
                                                            {
                                                                objViolationAndClamp.ViolationImage = ByteArrayCompressionUtility.Compress(imgCameraByteData);
                                                            }
                                                            resultmsg = dal_ViolationClamp.SaveViolationAndClamp(Convert.ToString(App.Current.Properties["apitoken"]), objViolationAndClamp);
                                                            if (resultmsg == "Success")
                                                            {
                                                                masterPage = new MasterHomePage();
                                                            }
                                                        }
                                                    });
                                                    if (existingCheckIn == string.Empty)
                                                    {
                                                        if (resultmsg == "Success")
                                                        {
                                                            await Navigation.PushAsync(masterPage);
                                                        }
                                                        else
                                                        {
                                                            await DisplayAlert("Alert", "Violation Check In failed,Please contact Admin.", "Ok");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await DisplayAlert("Alert", "Vehicle Checked In as Violation at " + existingCheckIn, "Ok");
                                                    }
                                                }
                                                else
                                                {
                                                    await DisplayAlert("Alert", "User details and API token unavailable. Login again", "Ok");
                                                }
                                            }
                                            else
                                            {
                                                await DisplayAlert("Alert", "Please select Vehicle Image.", "Ok");
                                            }

                                        }
                                        else
                                        {
                                            await DisplayAlert("Alert", "Please select Reason.", "Ok");
                                        }
                                    }
                                    else
                                    {
                                        await DisplayAlert("Alert", "Please check Clamp or Warning ", "Ok");
                                    }
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Please select Bay Number.", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Please enter valid Registration Number", "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please enter valid Registration Number.", "Ok");
                        }

                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
                    }
                    BtnViolation.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Alert", "Please check your Internet connection", "Ok");
                    ShowLoading(false);
                    BtnViolation.IsVisible = true;
                }

            }
            catch (Exception ex)
            {

            }
            ShowLoading(false);
            BtnViolation.IsVisible = true;

        }

        #region Pass Verification and Check-In
        private async void BtnCheckIn_Clicked(object sender, EventArgs e)
        {
            MasterHomePage masterPage = null;
            string msg = string.Empty;
            string existingCheckIn = string.Empty;
            btnCheckIn.IsVisible = false;
            try
            {
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken") && objCustomerPass != null)
                {
                    if (SelectedVehicle != string.Empty)
                    {
                        // Pass holder vehicle Check In
                        ParkingBay objselectedbay = (ParkingBay)pickerBayNumers.SelectedItem;
                        if (objselectedbay != null)
                        {
                            await Task.Run(() =>
                            {
                                existingCheckIn = VerifyVehicleCheckInStatus();
                                if (existingCheckIn == string.Empty)
                                {
                                    VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                                    objPassVehicle.UserID = objloginuser.UserID;
                                    objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                    objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                    objPassVehicle.VehicleTypeCode = SelectedVehicle;
                                    objPassVehicle.BayNumberID = objselectedbay.ParkingBayID;
                                    objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                                    objPassVehicle.CustomerID = objCustomerPass.CustomerVehicleID.CustomerID.CustomerID;
                                    msg = dal_DALCheckIn.SavePassVehicleCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                                    if (msg == "Success")
                                    {
                                        masterPage = new MasterHomePage();
                                    }
                                }
                            });
                            if (existingCheckIn == string.Empty)
                            {
                                if (msg == "Success")
                                {
                                    await Navigation.PushAsync(masterPage);

                                }
                                else
                                {

                                    await DisplayAlert("Alert", "Unable to Check In, Please contact Admin", "Ok");
                                }
                            }
                            else
                            {
                                await DisplayAlert("Alert", "Vehicle Checked In as Violation at " + existingCheckIn, "Ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please select Bay Number", "Ok");
                        }
                    }
                    else

                    {
                        await DisplayAlert("Alert", "Please select vehicle type and Registration Number- minimum 6 digits", "Ok");
                    }
                }
                else
                {

                    await DisplayAlert("Alert", "Unable to Check In, Please contact Admin", "Ok");
                }
                ShowLoading(false);
                btnCheckIn.IsVisible = true;
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "BtnCheckIn_Clicked");
                ShowLoading(false);
                btnCheckIn.IsVisible = true;
            }
        }
        #endregion

        #region Violation Warning Count 
        public bool IsVehicleViolationsCompleted()
        {
            bool warningCountCompleted = false;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken") && objCustomerPass != null)
                {
                    CustomerVehicle objCustVehicle = new CustomerVehicle();
                    objCustVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                    objCustVehicle.VehicleTypeID.VehicleTypeCode = SelectedVehicle;
                    warningCountCompleted = dal_ViolationClamp.GetVehicleViolationWaring(Convert.ToString(App.Current.Properties["apitoken"]), objCustVehicle);
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "IsVehicleViolationsCompleted");
            }
            return warningCountCompleted;
        }

        #endregion
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayoutViolationpage.Opacity = 0.5;
            }
            else
            {
                absLayoutViolationpage.Opacity = 1;
            }

        }
        public string VerifyVehicleCheckInStatus() // Verify Vehicle already parked
        {
            string alreadyCheckIn = string.Empty;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {

                    VehicleCheckIn objPassVehicle = new VehicleCheckIn();
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    objPassVehicle.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                    objPassVehicle.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                    objPassVehicle.VehicleTypeCode = SelectedVehicle;
                    objPassVehicle.RegistrationNumber = entryRegistrationNumber.Text;
                    CustomerParkingSlot resultobj = dal_DALCheckIn.VerifyVehicleChekcInStatus(Convert.ToString(App.Current.Properties["apitoken"]), objPassVehicle);
                    if (resultobj.CustomerParkingSlotID != 0)
                    {
                        alreadyCheckIn = resultobj.LocationParkingLotID.LocationID.LocationName + "-" + resultobj.LocationParkingLotID.LocationParkingLotName;
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "VerifyVehicleCheckInStatus");
            }
            return alreadyCheckIn;
        }
        public void EntryRegistrationNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(new Action(async () =>
            {
                try
                {
                    if (entryRegistrationNumber.Text != null && entryRegistrationNumber.Text != "")
                    {
                        if (SelectedVehicle != string.Empty)
                        {
                            if (entryRegistrationNumber.Text.Length < 6 && SelectedVehicle != string.Empty && SelectedVehicle != "")
                            {
                                pickerBayNumers.SelectedItem = null;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "Please select Vehicle type", "Ok");
                        }
                    }
                }
                catch (Exception ex)
                {
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "EntryRegistrationNumber_TextChanged");
                }
            }));

        }
        public async Task GetCurrentLocation()
        {
            try
            {

                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Latitude = (float)location.Latitude;
                    Longitude = (float)location.Longitude;
                }
                else
                {
                    await DisplayAlert("Alert", "Please enable your Device location.", "Ok");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Alert", "Please enable your Device location." + fnsEx.Message, "Ok");
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Alert", "Please enable your Device location." + fneEx.Message, "Ok");
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Alert", "Please enable your Device location." + pEx.Message, "Ok");
                // Handle permission exception
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Please enable your Device location." + ex.Message, "Ok");
                // Unable to get location
            }
        }

        #region Dynamic VehicleType
        public async void GetAllVehicleType()
        {
            try
            {

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType.Count > 0)
                    {


                        lstVehicleType = lstVehicleType.OrderBy(i => i.VehicleTypeID).ToList();
                        obsvehicleType = new ObservableCollection<VehicleType>(lstVehicleType);
                        if (obsvehicleType.Count > 0)
                        {
                            collstviewVehicleTye.WidthRequest = 300;
                            collstviewVehicleTye.ItemsSource = obsvehicleType.OrderBy(i => i.VehicleTypeID);
                            collstviewVehicleTye.SelectedItem = obsvehicleType[0];

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "GetAllVehicleType");
            }
        }
        public async void GetSelectedVehicleType(string VehicleTypeCode)
        {
            try
            {


                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var lstVehicleType = await App.SQLiteDb.GetAllVehicleTypesInSQLLite();
                    if (lstVehicleType.Count > 0)
                    {
                        var resultvehihcle = lstVehicleType.Where(v => v.VehicleTypeCode == VehicleTypeCode).ToList();
                        if (resultvehihcle != null & resultvehihcle.Count > 0)
                        {
                            obsvehicleType = new ObservableCollection<VehicleType>(resultvehihcle);
                            if (obsvehicleType.Count > 0)
                            {
                                for (var item = 0; item < obsvehicleType.Count; item++)
                                {
                                    obsvehicleType[item].VehicleDisplayImage = obsvehicleType[item].VehicleActiveImage;
                                    obsvehicleType[item] = obsvehicleType[item];
                                }
                                collstviewVehicleTye.WidthRequest = 90;
                                collstviewVehicleTye.ItemsSource = obsvehicleType;
                                SelectedVehicle = obsvehicleType[0].VehicleTypeCode;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "GetSelectedVehicleType");
            }
        }
        private void collstviewVehicleTye_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                var item = e.CurrentSelection;
                var selectedvehicle = item[0] as VehicleType;
                if (!string.IsNullOrEmpty(selectedvehicle.VehicleImage))
                {
                    SelectedVehicle = selectedvehicle.VehicleTypeCode;
                    UpdateCollectionViewSelectedItem(selectedvehicle);
                }



            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "collstviewVehicleTye_SelectionChanged");
            }
        }
        public void UpdateCollectionViewSelectedItem(VehicleType selectedVehicle)
        {
            try
            {
                for (var item = 0; item < obsvehicleType.Count; item++)
                {
                    if (obsvehicleType[item].VehicleTypeID == selectedVehicle.VehicleTypeID)
                    {

                        obsvehicleType[item].VehicleDisplayImage = obsvehicleType[item].VehicleActiveImage;
                    }
                    else
                    {
                        obsvehicleType[item].VehicleDisplayImage = obsvehicleType[item].VehicleInActiveImage;
                    }
                    obsvehicleType[item] = obsvehicleType[item];
                }
                collstviewVehicleTye.ItemsSource = obsvehicleType;
                if (!string.IsNullOrEmpty(selectedVehicle.VehicleTypeCode))
                {
                    SelectedVehicleBayNumbers(selectedVehicle.VehicleTypeCode);
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "UpdateCollectionViewSelectedItem");
            }
        }
        public async void SelectedVehicleBayNumbers(string SelectedVehicle)
        {
            var bayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList(); ;
            if (bayNumbers != null)
            {
                pickerBayNumers.ItemsSource = bayNumbers;
            }
            else
            {
                await DisplayAlert("Alert", "Bay numbers unavailable", "Ok");
            }
        }
        #endregion
        protected override bool OnBackButtonPressed()
        {
            return false;
        }

    }
}