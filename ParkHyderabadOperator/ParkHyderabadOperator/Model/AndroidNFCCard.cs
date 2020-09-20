using System;
using System.Collections.Generic;
using System.Text;
using Plugin.NFC;
using Xamarin.Forms;

namespace ParkHyderabadOperator.Model
{
   public class AndroidNFCCard
    {
        #region NFC Card Properties
                public const string ALERT_TITLE = "NFC";
                public const string MIME_TYPE = "application/com.companyname.nfcsample";
                NFCNdefTypeFormat _type;
                bool _makeReadOnly = false;
                public bool NfcIsEnabled { get; set; }
                public bool NfcIsDisabled => !NfcIsEnabled;
        #endregion

        
        public void CheckNFCSupported()
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
                            StartListeningIfNotiOS();
                        }
                    }
                }
                else
                {
                   
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #region NFC Card Related EVents
        void SubscribeEvents()
        {
            try
            {
                CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
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

                CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
                CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;

            }
            catch (Exception ex)
            {

            }
        }
        void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            string checkInmsg = string.Empty;
            try
            {
                if (tagInfo == null)
                {
                    
                    return;
                }
                // Customized serial number
                var identifier = tagInfo.Identifier;
                var serialNumber = tagInfo.SerialNumber;
                if (serialNumber != "")
                {
                    
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
        {
            try
            {
                if (!CrossNFC.Current.IsWritingTagSupported)
                {
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
                  
                }
            }
            catch (Exception ex)
            {
                
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
        #endregion
    }
}
