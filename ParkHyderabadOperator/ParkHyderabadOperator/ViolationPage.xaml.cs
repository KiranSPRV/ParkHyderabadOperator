using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALViolation;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using Plugin.Media;
using System;
using System.Collections.Generic;
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
        string fileName = string.Empty;
        string SelectedVehicle = string.Empty;
        byte[] imgCameraByteData = null;
        private float Latitude;
        private float Longitude;
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
        private void LoadDefaultLotVehicleType()
        {
            try
            {
                SelectedVehicle = "2W";
                imgBtnTwoWheeler.Source = "Twowheeler_circle_ticked.png";
                imgBtnFourWheeler.Source = "Fourwheeler_circle.png";
                if (lstparkingbay.Count > 0)
                {
                    var twowheelerbayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList();
                    if (twowheelerbayNumbers != null)
                    {
                        pickerBayNumers.ItemsSource = twowheelerbayNumbers;
                    }
                }
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
                        if (objCustomerPass.CustomerVehiclePassID != 0)
                        {
                            btnCheckIn.IsVisible = true;
                            BtnViolation.IsVisible = false;
                            await DisplayAlert("Alert", "" + entryRegistrationNumber.Text + " This vehicle has a valid pass", "Ok");
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

        #region Vehicle-Type Selection
        private async void SlTwoWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {

                SelectedVehicle = "2W";
                imgBtnTwoWheeler.Source = "Twowheeler_circle_ticked.png";
                imgBtnFourWheeler.Source = "Fourwheeler_circle.png";
                LoadLocationBayNumbers();
                if (lstparkingbay.Count > 0)
                {
                    var twowheelerbayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList(); ;
                    if (twowheelerbayNumbers != null)
                    {
                        pickerBayNumers.ItemsSource = twowheelerbayNumbers;
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Bay numbers unavailable", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "ImgBtnTwoWheeler_Clicked");
            }
        }
        private async void SlFourWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {

                SelectedVehicle = "4W";
                imgBtnTwoWheeler.Source = ImageSource.FromFile("Twowheeler_circle.png");
                imgBtnFourWheeler.Source = ImageSource.FromFile("Fourwheeler_circle_ticked.png");
                LoadLocationBayNumbers();
                if (lstparkingbay.Count > 0)
                {
                    var twowheelerbayNumbers = lstparkingbay.Where(i => i.VehicleTypeID.VehicleTypeCode.ToUpper() == (SelectedVehicle)).ToList(); ;
                    if (twowheelerbayNumbers != null)
                    {
                        pickerBayNumers.ItemsSource = twowheelerbayNumbers;
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Bay numbers unavailable", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ViolationPage.xaml.cs", "", "ImgBtnFourWheeler_Clicked");
            }
        }

        #endregion Vehicle-Type Selection
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
                                //for (var i = 0; i < 1; i++)
                                //{
                                //    ObjblueToothDevicePrinting.PrintPdfFile(objfilestream, "InnerPrinter");
                                //}

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
                                                            objViolationAndClamp.ViolationStartTime = DateTime.Now;
                                                            objViolationAndClamp.ViolationTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt");
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
        protected override bool OnBackButtonPressed()
        {
            return false;
        }

    }
}