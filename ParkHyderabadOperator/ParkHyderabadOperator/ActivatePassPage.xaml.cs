using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIOutPutModel;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivatePassPage : ContentPage
    {
        List<CustomerVehicle> lstCustomerVehicle = null;
        CustomerVehiclePass objResultCustomerVehiclePass;
        DALExceptionManagment dal_Exceptionlog;
        DALPass dal_Pass;

        #region NFC Card Properties
        public const string ALERT_TITLE = "NFC";
        NFCNdefTypeFormat _type;
        bool _makeReadOnly = false;
        private bool _nfcIsEnabled;
        public bool NfcIsEnabled { get; set; }
        public bool NfcIsDisabled => !NfcIsEnabled;
        #endregion

        public ActivatePassPage()
        {
            InitializeComponent();
            BtnContinue.IsEnabled = false;
            dal_Pass = new DALPass();
            dal_Exceptionlog = new DALExceptionManagment();
            GetAllPassedVehicles();
            try
            {
                CheckNFCSupported();
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "ActivatePassPage()");
                DisplayAlert("Alert", "Unable to proceed,Please contact admin" + ex.Message, "Ok");
            }
        }
        #region Searh Box Related Code
        public async void GetAllPassedVehicles()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALReNewPass dal_ReNewPass = new DALReNewPass();
                    lstCustomerVehicle = dal_ReNewPass.GetAllPassedVehicles(Convert.ToString(App.Current.Properties["apitoken"]));
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("", "" + ex, "Ok");
            }
        }
        public async Task GetSelectedVehicleDetails(CustomerVehicle selectedVehicle)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    DALPass dal_Pass = new DALPass();
                    objResultCustomerVehiclePass = dal_Pass.GetCustomerVehicleDetailsByVehicle(Convert.ToString(App.Current.Properties["apitoken"]), selectedVehicle);
                    if (objResultCustomerVehiclePass != null && objResultCustomerVehiclePass.CustomerVehiclePassID != 0)
                    {

                        if (objResultCustomerVehiclePass.IssuedCard)
                        {


                            entryCustomerName.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name;
                            entryPhoneNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber;
                            entryRegistrationNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber;
                            // Verify Customer Vehicle Type
                            if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "2W")
                            {
                                imgCustomerVehcileType.Source = ImageSource.FromFile("bike_black.png");
                            }
                            else if (objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleTypeCode == "4W")
                            {
                                imgCustomerVehcileType.Source = ImageSource.FromFile("car_black.png");
                            }

                            if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                            {
                                slNFC.IsVisible = false;
                                slContinue.IsVisible = false;
                                await DisplayAlert("Alert", "Please check your pass type.", "Ok");
                            }
                            else
                            {
                                slNFC.IsVisible = true;
                                slContinue.IsVisible = true;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Alert", "NFC not available for this Vehicle", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "No records found,Please contact admin.", "Ok");
                    }


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("", "" + ex, "Ok");
            }
        }
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            listViewVehicleRegistrationNumbers.IsVisible = true;
            listViewVehicleRegistrationNumbers.BeginRefresh();

            try
            {
                var dataEmpty = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                    listViewVehicleRegistrationNumbers.IsVisible = false;
                else
                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private async void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            CustomerVehicle selecteditems = (CustomerVehicle)e.Item;
            if (selecteditems != null)
            {
                searchBar.Text = selecteditems.RegistrationNumber;
                GetSelectedVehicleDetails(selecteditems);
            }
            listViewVehicleRegistrationNumbers.IsVisible = false;
            ((ListView)sender).SelectedItem = null;
        }
        #endregion

        private async void BtnContinue_Clicked(object sender, EventArgs e)
        {
            try
            {
                objResultCustomerVehiclePass.IssuedCard = true;
                objResultCustomerVehiclePass.CardNumber = labelNFCCard.Text;
                objResultCustomerVehiclePass.TotalAmount = objResultCustomerVehiclePass.PassPriceID.Price + objResultCustomerVehiclePass.PassPriceID.NFCCardPrice;
                objResultCustomerVehiclePass.BarCode = labelBARCode.Text;
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    objResultCustomerVehiclePass = dal_Pass.ActivateCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), objResultCustomerVehiclePass);
                    await Navigation.PushAsync(new PassPaymentReceiptPage(objResultCustomerVehiclePass));
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region NFC Card Related Code

        public async void CheckNFCSupported()
        {

            try
            {
                if (CrossNFC.IsSupported)
                {
                    if (CrossNFC.Current.IsAvailable)
                    {
                        NfcIsEnabled = CrossNFC.Current.IsEnabled;
                        if (NfcIsEnabled)
                        {
                            CrossNFC.Current.StopListening();
                            SubscribeEvents();
                            await StartListeningIfNotiOS();
                        }
                        else
                        {
                            await DisplayAlert("Alert", "NFC is disabled", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "NFC is not available", "Ok");
                    }

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
                    if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                    {
                        slNFC.IsVisible = false;
                        BtnContinue.IsVisible = false;
                    }
                    else
                    {
                        labelNFCCard.Text = serialNumber;
                        BtnContinue.IsEnabled = true;
                    }

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
                                MimeType = MonthlyPassCashPaymentPage.MIME_TYPE,
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
                                MimeType = MonthlyPassCashPaymentPage.MIME_TYPE,
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
    }
}