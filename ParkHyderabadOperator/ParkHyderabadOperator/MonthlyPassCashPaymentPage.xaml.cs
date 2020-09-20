
using ParkHyderabadOperator.DAL.DALCheckIn;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.ViewModel.VMPass;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonthlyPassCashPaymentPage : ContentPage
    {
        CustomerVehiclePass objInputMonthlyPass;
        DALExceptionManagment dal_Exceptionlog;
        DALPass dal_CustomerPass;
        string selectedPassStationType = string.Empty;
        DALCheckIn dal_DALCheckIn;

        #region NFC Card Properties
        public const string ALERT_TITLE = "NFC";
        public const string MIME_TYPE = "application/com.companyname.nfcsample";
        NFCNdefTypeFormat _type;
        bool _makeReadOnly = false;
        private bool _nfcIsEnabled;
        public bool NfcIsEnabled { get; set; }
        public bool NfcIsDisabled => !NfcIsEnabled;
        #endregion

        public MonthlyPassCashPaymentPage()
        {
            InitializeComponent();
            slNFCCard.IsVisible = false;
            slCashPaymentGeneratePass.IsVisible = false;
            slBARCode.IsVisible = false;
            dal_CustomerPass = new DALPass();
            dal_DALCheckIn = new DALCheckIn();
        }
        public MonthlyPassCashPaymentPage(CustomerVehiclePass objmonthlyPass)
        {
            InitializeComponent();
            dal_Exceptionlog = new DALExceptionManagment();
            dal_CustomerPass = new DALPass();
            slNFCCard.IsVisible = false;
            slBARCode.IsVisible = false;
            dal_DALCheckIn = new DALCheckIn();
            slCashPaymentGeneratePass.IsVisible = false;

            try
            {
                objInputMonthlyPass = objmonthlyPass;
                if (objInputMonthlyPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("bike_black.png");
                }
                else if (objInputMonthlyPass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                {
                    ImgVehicleType.Source = ImageSource.FromFile("car_black.png");
                }
                labelVehicleRegNumber.Text = objInputMonthlyPass.CustomerVehicleID.RegistrationNumber;
                labelParkingLocation.Text = objInputMonthlyPass.LocationID.LocationName + "-" + objInputMonthlyPass.PassPriceID.StationAccess;
                labelPassAmount.Text = objInputMonthlyPass.TotalAmount.ToString("N2") + "/-";
                if (objInputMonthlyPass.IssuedCard)
                {

                    labelAmountDetails.Text = "( " + objInputMonthlyPass.Amount.ToString("N2") + " Rs Pass + " + objInputMonthlyPass.CardAmount.ToString("N2") + " Rs Card )";

                }
                else
                {
                    labelAmountDetails.Text = "( " + objInputMonthlyPass.Amount.ToString("N2") + " Rs Pass )";
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
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "MonthlyPassCashPaymentPage");
            }
        }
        private async void BtnYes_Clicked(object sender, EventArgs e)
        {
            try
            {
                decimal passAmount = (objInputMonthlyPass.TotalAmount == null || objInputMonthlyPass.TotalAmount == 0) ? objInputMonthlyPass.Amount : objInputMonthlyPass.TotalAmount;
                if (Convert.ToDecimal(entryCashReceived.Text) >= passAmount)
                {
                    stlayoutYESNO.IsVisible = false;
                    slBARCode.IsVisible = true;
                    slCashPaymentGeneratePass.IsVisible = true;
                    if (objInputMonthlyPass.IssuedCard)
                    {
                        slNFCCard.IsVisible = true;
                    }
                    else
                    {
                        slNFCCard.IsVisible = false;
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please enter valid pass amount.", "Ok");
                }
            }
            catch (Exception ex)
            {

            }

        }
        private async void BtnNo_Clicked(object sender, EventArgs e)
        {
            try
            {
                stlayoutYESNO.IsVisible = false;
                slNFCCard.IsVisible = false;
                slCashPaymentGeneratePass.IsVisible = false;
                var passPage = new PassPage();
                await Navigation.PushAsync(passPage);
            }
            catch (Exception ex)
            {

            }
        }
        private void ImgBtnNFCCard_Clicked(object sender, EventArgs e)
        {
            try
            {
                labelNFCCard.Text = "#12345";
                labelNFCSuccessMsg.Text = "NFC card added successfully";
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "EntryCashReceived_TextChanged");
            }
        }
        private async void BtnGeneratePass_Clicked(object sender, EventArgs e)
        {
            try
            {
                CustomerVehiclePass resultPass = null;
                ShowLoading(true);
                if (entryCashReceived.Text != null && entryCashReceived.Text != "0")
                {
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        if (!IsVehiclehasPass())
                        {
                            if (App.Current.Properties.ContainsKey("MultiSelectionLocations"))
                            {

                                List<Location> lstMultiLication = (List<Location>)App.Current.Properties["MultiSelectionLocations"];
                                objInputMonthlyPass.CardNumber = labelNFCCard.Text;
                                objInputMonthlyPass.IsMultiLot = true;
                                VMMultiStationCustomerVehiclePass objvmMultiStations = new VMMultiStationCustomerVehiclePass();
                                objvmMultiStations.CustomerVehiclePassID = objInputMonthlyPass;
                                objvmMultiStations.LocationID = lstMultiLication;
                                await Task.Run(() =>
                                {
                                    resultPass = dal_CustomerPass.CreateMultiStationCustomerPass(Convert.ToString(App.Current.Properties["apitoken"]), objvmMultiStations);
                                });
                                if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                                {
                                    await DisplayAlert("Alert", "Customer vehicle pass created successfully", "Ok");
                                    await Navigation.PushAsync(new PassPaymentReceiptPage(resultPass));
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Fail,Please contact admin", "Ok");
                                }
                            }
                            else
                            {
                                await Task.Run(() =>
                                {
                                    objInputMonthlyPass.CardNumber = labelNFCCard.Text;
                                    objInputMonthlyPass.BarCode = labelBARCode.Text;
                                    resultPass = dal_CustomerPass.CreateCustomerPass(Convert.ToString(App.Current.Properties["apitoken"]), objInputMonthlyPass);
                                });
                                if (resultPass != null && resultPass.CustomerVehiclePassID != 0)
                                {
                                    StopNFCListening();
                                    await DisplayAlert("Alert", "Customer vehicle pass created successfully", "Ok");
                                    await Navigation.PushAsync(new PassPaymentReceiptPage(resultPass));
                                }
                                else
                                {
                                    await DisplayAlert("Alert", "Fail,Please contact admin", "Ok");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", labelNFCCard.Text + ": This NFC card already in use for another vehicle.", "Ok");
                        }
                    }

                }
                ShowLoading(false);

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "BtnGeneratePassReceipt_Clicked");
            }
        }

        #region Payment Calculation
        private async void EntryCashReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entryCashReceived.Text.Length > 0 && entryCashReceived.Text != null)
                {
                    decimal passAmount = (objInputMonthlyPass.TotalAmount == null || objInputMonthlyPass.TotalAmount == 0) ? objInputMonthlyPass.Amount : objInputMonthlyPass.TotalAmount;
                    decimal returnAmount = Math.Abs((Convert.ToDecimal(entryCashReceived.Text) - passAmount));
                    entryCashReturn.Text = returnAmount.ToString("N2");
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "MonthlyPassCashPaymentPage.xaml.cs", "", "EntryCashReceived_TextChanged");
            }
        }
        #endregion

        #region NFC Card Related Code

        public async void CheckNFCSupported()
        {

            try
            {
                if (CrossNFC.IsSupported)
                {
                    if (!CrossNFC.Current.IsAvailable)
                        await DisplayAlert("Alert", "NFC is not available", "Ok");

                    NfcIsEnabled = CrossNFC.Current.IsEnabled;
                    if (!NfcIsEnabled)
                        await DisplayAlert("Alert", "NFC is disabled", "Ok");
                    CrossNFC.Current.StopListening();
                    SubscribeEvents();
                    await StartListeningIfNotiOS();
                }
                else
                {
                    await DisplayAlert("Alert", "NFC is not supported,Please contact admin", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        async void SubscribeEvents()
        {
            try
            {
                CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        async void UnsubscribeEvents()
        {
            try
            {
                CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            try
            {
                if (tagInfo == null)
                {
                    await DisplayAlert("Alert", "No tag found", "Ok");
                    return;
                }
                // Customized serial number
                var identifier = tagInfo.Identifier;
                var serialNumber = tagInfo.SerialNumber;
                if (serialNumber != "")
                {
                    labelNFCCard.Text = serialNumber;
                }
                else
                {

                    await DisplayAlert("Alert", "NFC Card serialNumber unable to found.", "Ok");
                    return;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            try
            {
                if (!CrossNFC.Current.IsWritingTagSupported)
                {
                    await DisplayAlert("Alert", "Writing tag is not supported on this device", "Ok");
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
                    await DisplayAlert("Alert", ex.Message, "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        /// <summary>
        /// Task to start listening for NFC tags if the user's device platform is not iOS
        /// </summary>
        /// <returns>Task to be performed</returns>
        async Task StartListeningIfNotiOS()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                    return;
                await BeginListening();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Task to safely start listening for NFC Tags
        /// </summary>
        /// <returns>The task to be performed</returns>
        async Task BeginListening()
        {
            try
            {
                CrossNFC.Current.StartListening();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "Ok");
            }
        }
        public void StopNFCListening()
        {
            #region  NFC Code 
            try
            {

                CrossNFC.Current.StopListening();
                UnsubscribeEvents();

            }
            catch (Exception ex)
            {

            }

            #endregion

        }
        #endregion
        private async void ImgbtnScanBarCode_Clicked(object sender, EventArgs e)
        {
            try
            {
                var scanner = new MobileBarcodeScanner();
                scanner.UseCustomOverlay = true;
                ZXing.Result result = await scanner.Scan();
                if (result == null)
                    return;
                labelBARCode.Text = result.Text;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "Ok");
            }
        }
        public bool IsVehiclehasPass()
        {
            bool IsVehiclehasPass = false;
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    Location objLoginUserLocation = objloginuser.LocationParkingLotID.LocationID;
                    var objCustomerPass = dal_DALCheckIn.GetVerifyVehicleHasPass(Convert.ToString(App.Current.Properties["apitoken"]), objInputMonthlyPass.CustomerVehicleID.RegistrationNumber, 0, 0, objloginuser.UserID, labelNFCCard.Text);
                    if (objCustomerPass.CustomerVehiclePassID != 0)
                    {
                        if (objCustomerPass.CustomerVehicleID.RegistrationNumber != objInputMonthlyPass.CustomerVehicleID.RegistrationNumber)
                        {
                            IsVehiclehasPass = true;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "CheckInPage.xaml.cs", "", "EntryRegistrationNumber_TextChanged");
            }
            return IsVehiclehasPass;
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;


        }
    }
}