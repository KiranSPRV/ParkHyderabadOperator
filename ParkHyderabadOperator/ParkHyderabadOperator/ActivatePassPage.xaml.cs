using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.DAL.DALPass;
using ParkHyderabadOperator.Model.APIOutPutModel;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.IO;
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
            slContinue.IsVisible = false;
            stlayoutNFCCardPayment.IsVisible = false;
            slNFCBarCodeSection.IsVisible = false;
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
                DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
            }
        }

        #region Searh Box Related Code
        public void GetAllPassedVehicles()
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
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "GetAllPassedVehicles");
            }
        }
        public async Task GetSelectedVehicleDetails(CustomerVehicle selectedVehicle)
        {
            try
            {
                ShowLoading(true);
                if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                {
                    User objloginuser = (User)App.Current.Properties["LoginUser"];
                    dal_Pass = new DALPass();
                    await Task.Run(() =>
                    {
                        objResultCustomerVehiclePass = dal_Pass.GetCustomerVehicleDetailsByVehicle(Convert.ToString(App.Current.Properties["apitoken"]), selectedVehicle);

                    });
                    if (objResultCustomerVehiclePass != null && objResultCustomerVehiclePass.CustomerVehiclePassID != 0)
                    {
                        entryCustomerName.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.Name;
                        entryPhoneNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.CustomerID.PhoneNumber;
                        entryRegistrationNumber.Text = objResultCustomerVehiclePass.CustomerVehicleID.RegistrationNumber;

                        // Verify Customer Vehicle Type
                        imgCustomerVehcileType.Source = objResultCustomerVehiclePass.CustomerVehicleID.VehicleTypeID.VehicleIcon;
                        if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                        {
                            slContinue.IsVisible = false;
                            stlayoutNFCCardPayment.IsVisible = false;
                            await DisplayAlert("Alert", "NFC card not valid  for this Pass type.", "Ok");
                        }
                        if (objResultCustomerVehiclePass.CardNumber != null && objResultCustomerVehiclePass.CardNumber != "")
                        {
                            await DisplayAlert("Alert", "This vehicle already has NFC card.", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "No records found,Please contact Admin.", "Ok");
                    }


                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "GetSelectedVehicleDetails)");
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
                {
                    listViewVehicleRegistrationNumbers.IsVisible = false;
                    ClearFileds();
                }
                else
                    listViewVehicleRegistrationNumbers.ItemsSource = lstCustomerVehicle.Where(i => i.RegistrationNumber.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                listViewVehicleRegistrationNumbers.IsVisible = false;
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "SearchBar_OnTextChanged");

            }
            listViewVehicleRegistrationNumbers.EndRefresh();

        }
        private async void listViewVehicleRegistrationNumbers_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            CustomerVehicle selecteditems = (CustomerVehicle)e.Item;
            if (selecteditems != null)
            {
                searchBar.Text = selecteditems.RegistrationNumber;
                await GetSelectedVehicleDetails(selecteditems);
            }
            listViewVehicleRegistrationNumbers.IsVisible = false;
            ((ListView)sender).SelectedItem = null;
        }

        #endregion

        private async void BtnContinue_Clicked(object sender, EventArgs e)
        {
            try
            {
                string existingnfcCardVehcile = string.Empty;
                string cardNumber = string.Empty;
                ActivatePassReciptPage passPaymentReceiptPage = null;
                ShowLoading(true);
                slContinue.IsVisible = false;
                objResultCustomerVehiclePass.IssuedCard = true;
                if (!string.IsNullOrEmpty(labelNFCCard.Text))
                {
                    cardNumber = labelNFCCard.Text;
                }
                if (!string.IsNullOrEmpty(labelBARCode.Text))
                {
                    if (labelBARCode.Text.Contains(":"))
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }
                    else
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }

                    objResultCustomerVehiclePass.BarCode = cardNumber;
                }
                if (!string.IsNullOrEmpty(cardNumber))
                {
                    objResultCustomerVehiclePass.CardNumber = cardNumber;
                    objResultCustomerVehiclePass.TotalAmount = objResultCustomerVehiclePass.PassPriceID.Price + objResultCustomerVehiclePass.PassPriceID.CardPrice;
                    objResultCustomerVehiclePass.BarCode = labelBARCode.Text;
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        existingnfcCardVehcile = dal_Pass.IsValidNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), labelNFCCard.Text, entryRegistrationNumber.Text);
                        if (existingnfcCardVehcile == string.Empty)
                        {
                            await Task.Run(() =>
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                objResultCustomerVehiclePass.CreatedBy.UserID = objloginuser.UserID;
                                objResultCustomerVehiclePass = dal_Pass.ActivateCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), objResultCustomerVehiclePass);
                                passPaymentReceiptPage = new ActivatePassReciptPage(objResultCustomerVehiclePass);
                            });
                            await Navigation.PushAsync(passPaymentReceiptPage);
                            slContinue.IsVisible = true;
                            ShowLoading(false);

                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("Alert", "NFC Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                        }
                    }
                }
                else
                {
                    ShowLoading(false);
                    await DisplayAlert("Alert", "Please tap card", "Ok");
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "BtnContinue_Clicked");
                ShowLoading(false);
                slContinue.IsVisible = true;

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
                    await DisplayAlert("Alert", "NFC  not supported,Please contact Admin", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "CheckNFCSupported");
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
                await DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
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
                await DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
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
                    }
                    else
                    {
                        labelNFCCard.Text = serialNumber;

                    }

                }
                else
                {

                    await DisplayAlert("Alert", "NFC Card serial number unable to found.", "Ok");
                    return;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
            }
        }
        async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            try
            {
                if (!CrossNFC.Current.IsWritingTagSupported)
                {
                    await DisplayAlert("Alert", "Writing tag not supported on this device", "Ok");
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
                await DisplayAlert("Alert", "Unable to proceed,Please contact Admin" + ex.Message, "Ok");
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
                ShowLoading(true);
                var scanner = new MobileBarcodeScanner();
                scanner.UseCustomOverlay = true;
                ZXing.Result result = await scanner.Scan();
                if (result != null)
                {
                    labelBARCode.Text = result.Text;
                    if (!string.IsNullOrEmpty(labelBARCode.Text) && labelBARCode.Text.Contains(":"))
                    {
                        labelBARCode.Text = labelBARCode.Text.Replace(":", "");
                    }
                }
                else
                {
                    labelBARCode.Text = string.Empty;
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                await DisplayAlert("Alert", ex.Message, "Ok");
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "ImgbtnScanBarCode_Clicked");
            }
        }

        #region New  Card Payment 
        private async void ChkNewCard_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                string existingnfcCardVehcile = string.Empty;
                if (chkNewCard.IsChecked)
                {
                    if (chkhasCard.IsChecked)
                    {
                        chkhasCard.IsChecked = false;
                    }
                    if (objResultCustomerVehiclePass != null && objResultCustomerVehiclePass.CustomerVehiclePassID != 0)
                    {
                        if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                        {
                            slContinue.IsVisible = false;
                            stlayoutNFCCardPayment.IsVisible = false;
                            await DisplayAlert("Alert", "NFC card not valid  for this Pass type.", "Ok");
                        }
                        else
                        {
                            existingnfcCardVehcile = dal_Pass.IsValidNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), labelNFCCard.Text, entryRegistrationNumber.Text);
                            if (existingnfcCardVehcile == string.Empty)
                            {
                                slContinue.IsVisible = false;
                                stlayoutNFCCardPayment.IsVisible = true;
                                slNFCBarCodeSection.IsVisible = true;
                                if (objResultCustomerVehiclePass.LocationID.LocationCardTypeID.CardTypeName.ToUpper() == "NFC Card".ToUpper())
                                {
                                    slNFC.IsVisible = true;
                                    slBARCodeReader.IsVisible = false;
                                }
                                if (objResultCustomerVehiclePass.LocationID.LocationCardTypeID.CardTypeName.ToUpper() == "BlueTooth".ToUpper())
                                {
                                    slNFC.IsVisible = false;
                                    slBARCodeReader.IsVisible = true;
                                }

                            }
                            else
                            {
                                ShowLoading(false);
                                await DisplayAlert("Alert", "NFC Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please select Vehicle", "Ok");
                    }
                }
                else
                {
                    slNFC.IsVisible = false;
                    slBARCodeReader.IsVisible = false;
                    stlayoutNFCCardPayment.IsVisible = false;
                }

            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "ChkNewCard_CheckedChanged");
            }
        }
        private async void ChkhasCard_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                string existingnfcCardVehcile = string.Empty;
                if (chkhasCard.IsChecked)
                {
                    if (chkNewCard.IsChecked)
                    {
                        chkNewCard.IsChecked = false;
                    }
                    if (objResultCustomerVehiclePass != null && objResultCustomerVehiclePass.CustomerVehiclePassID != 0)
                    {

                        if (objResultCustomerVehiclePass.PassPriceID.PassTypeID.PassTypeCode.ToUpper() == "WP")
                        {
                            slContinue.IsVisible = false;
                            stlayoutNFCCardPayment.IsVisible = false;
                            await DisplayAlert("Alert", "NFC card not valid  for this Pass type.", "Ok");
                        }
                        else
                        {
                            existingnfcCardVehcile = dal_Pass.IsValidNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), labelNFCCard.Text, entryRegistrationNumber.Text);
                            if (existingnfcCardVehcile == string.Empty)
                            {
                                slContinue.IsVisible = true;
                                stlayoutNFCCardPayment.IsVisible = false;
                                slNFCBarCodeSection.IsVisible = true;
                                if (objResultCustomerVehiclePass.LocationID.LocationCardTypeID.CardTypeName.ToUpper() == "NFC Card".ToUpper())
                                {
                                    slNFC.IsVisible = true;
                                    slBARCodeReader.IsVisible = false;
                                }
                                if (objResultCustomerVehiclePass.LocationID.LocationCardTypeID.CardTypeName.ToUpper() == "BlueTooth".ToUpper())
                                {
                                    slNFC.IsVisible = false;
                                    slBARCodeReader.IsVisible = true;
                                }

                            }
                            else
                            {
                                ShowLoading(false);
                                await DisplayAlert("Alert", "NFC Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Please select Vehicle", "Ok");
                    }
                }
                else
                {
                    slContinue.IsVisible = false;
                    slNFC.IsVisible = false;
                    slBARCodeReader.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "ChkhasCard_CheckedChanged");
            }
        }
        private async void SlCashPayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                string existingnfcCardVehcile = string.Empty;
                string cardNumber = string.Empty;
                if (!string.IsNullOrEmpty(labelNFCCard.Text))
                {
                    cardNumber = labelNFCCard.Text;
                }
                if (!string.IsNullOrEmpty(labelBARCode.Text))
                {
                    if (labelBARCode.Text.Contains(":"))
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }
                    else
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }

                    objResultCustomerVehiclePass.BarCode = cardNumber;
                }
                if (!string.IsNullOrEmpty(cardNumber))
                {
                    ShowLoading(true);
                    NFCCardCashPaymentPage nfccashPage = null;
                    existingnfcCardVehcile = dal_Pass.IsValidNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), labelNFCCard.Text, entryRegistrationNumber.Text);
                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        if (existingnfcCardVehcile == string.Empty)
                        {
                            await Task.Run(() =>
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                objResultCustomerVehiclePass.CardNumber = cardNumber;
                                objResultCustomerVehiclePass.NFCCardPaymentID.PaymentTypeCode = "Cash";
                                objResultCustomerVehiclePass.NFCCardSoldByID.UserID = objloginuser.UserID;
                                objResultCustomerVehiclePass.NFCSoldLotID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                objResultCustomerVehiclePass.NFCSoldLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                objResultCustomerVehiclePass.CreatedBy.LocationParkingLotID.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                objResultCustomerVehiclePass.CardTypeID = objResultCustomerVehiclePass.LocationID.LocationCardTypeID;
                                nfccashPage = new NFCCardCashPaymentPage(objResultCustomerVehiclePass);

                            });
                            await Navigation.PushAsync(nfccashPage);
                            ShowLoading(false);
                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("Alert", "Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                        }
                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please tap card", "Ok");
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "SlCashPayment_Tapped");
            }
        }
        private async void SlEpayment_Tapped(object sender, EventArgs e)
        {
            try
            {
                NFCCardEPaymentPage nfcEpayPage = null;
                string existingnfcCardVehcile = string.Empty;
                string cardNumber = string.Empty;
                if (!string.IsNullOrEmpty(labelNFCCard.Text))
                {
                    cardNumber = labelNFCCard.Text;
                }
                if (!string.IsNullOrEmpty(labelBARCode.Text))
                {
                    if (labelBARCode.Text.Contains(":"))
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }
                    else
                    {
                        cardNumber = labelBARCode.Text.Replace(":", "");
                    }

                    objResultCustomerVehiclePass.BarCode = cardNumber;
                }
                if (!string.IsNullOrEmpty(cardNumber))
                {
                    ShowLoading(true);

                    if (App.Current.Properties.ContainsKey("LoginUser") && App.Current.Properties.ContainsKey("apitoken"))
                    {
                        existingnfcCardVehcile = dal_Pass.IsValidNFCCard(Convert.ToString(App.Current.Properties["apitoken"]), labelNFCCard.Text, entryRegistrationNumber.Text);
                        if (existingnfcCardVehcile == string.Empty)
                        {
                            await Task.Run(() =>
                            {
                                User objloginuser = (User)App.Current.Properties["LoginUser"];
                                objResultCustomerVehiclePass.CardNumber = labelNFCCard.Text;
                                objResultCustomerVehiclePass.BarCode = labelBARCode.Text;
                                objResultCustomerVehiclePass.NFCCardPaymentID.PaymentTypeCode = "EPay";
                                objResultCustomerVehiclePass.NFCCardSoldByID.UserID = objloginuser.UserID;
                                objResultCustomerVehiclePass.CreatedBy.LocationParkingLotID.LocationID.LocationName = objloginuser.LocationParkingLotID.LocationID.LocationName;
                                objResultCustomerVehiclePass.NFCSoldLotID.LocationID.LocationID = objloginuser.LocationParkingLotID.LocationID.LocationID;
                                objResultCustomerVehiclePass.NFCSoldLotID.LocationParkingLotID = objloginuser.LocationParkingLotID.LocationParkingLotID;
                                objResultCustomerVehiclePass.CardTypeID = objResultCustomerVehiclePass.LocationID.LocationCardTypeID;
                                nfcEpayPage = new NFCCardEPaymentPage(objResultCustomerVehiclePass);
                            });
                            await Navigation.PushAsync(nfcEpayPage);
                            ShowLoading(false);
                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("Alert", "Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                        }
                    }
                    else
                    {
                        ShowLoading(false);
                        await DisplayAlert("Alert", "Card already assigned to " + existingnfcCardVehcile + "", "Ok");

                    }
                }
                else
                {
                    await DisplayAlert("Alert", "Please tap card", "Ok");
                }

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "SlEpayment_Tapped");
            }
        }

        #endregion

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
        }
        private void ClearFileds()
        {
            try
            {
                ShowLoading(true);

                entryCustomerName.Text = string.Empty;
                entryPhoneNumber.Text = string.Empty;
                entryRegistrationNumber.Text = string.Empty;
                imgCustomerVehcileType.Source =null;
                chkhasCard.IsChecked = false;
                chkNewCard.IsChecked = false;
                slContinue.IsVisible = false;
                stlayoutNFCCardPayment.IsVisible = false;
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "ActivatePassPage.xaml.cs", "", "ClearFileds)");
            }
        }
    }
}