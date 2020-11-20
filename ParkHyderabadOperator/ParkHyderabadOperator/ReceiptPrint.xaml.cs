using ParkHyderabadOperator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabadOperator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptPrint : ContentPage
    {
        BlueToothDevicePrinting ObjblueToothDevicePrinting = new BlueToothDevicePrinting();
        public ReceiptPrint()
        {
            InitializeComponent();
        }
        private async void BtnPrintReceipt_Clicked(object sender, EventArgs e)
        {
            string printerName = string.Empty;
            try
            {
                printerName = ObjblueToothDevicePrinting.GetBlueToothDevices();
                labelPrinterName.Text = printerName;
                if (printerName != string.Empty && printerName != "")
                {
                    string[] receiptlines = new string[16];

                    receiptlines[0] = "\x1B\x21\x12" + "          " + "HMRL PARKING" + "\x1B\x21\x00" + "\n";
                    receiptlines[1] = "\x1B\x21\x01" + "       " + "Location And Lot"+ "\x1B\x21\x00" + "\n";
                    receiptlines[2] = "\x1B\x21\x08" + "BIKE:     TS05FL0960" + "\x1B\x21\x00" + "\n";
                    receiptlines[3] = "\x1B\x21\x08" + "(In)11/06/20 7:37" + "\x1B\x21\x00" + "\n";
                    receiptlines[4] = "\x1B\x21\x06" + "Paid: Rs 10.00" + "\x1B\x21\x00" + "\n";
                    receiptlines[5] = "\x1B\x21\x01" + "Parked at - Bays: (40-50)" + "\x1B\x21\x00" + "\n";
                    receiptlines[6] = "\x1B\x21\x01" + "Operator Id :2001" + "\x1B\x21\x00" + "\n";
                    receiptlines[7] = "\x1B\x21\x01" + "We are not responsible for your" + "\x1B\x21\x00" + "\n";
                    receiptlines[8] = "\x1B\x21\x06" + "GST Number 0012" + "\x1B\x21\x00" + "\n";
                    receiptlines[9] = "\x1B\x21\x01" + "Amount includes 18% GST" + "\x1B\x21\x00" + "\n";
                    receiptlines[10] = "\x1B\x21\x03" + "We are not responsible " + "\x1B\x21\x00" + "\n";
                    receiptlines[11] = "\x1B\x21\x05" + "We are not responsible for your" + "\x1B\x21\x00" + "\n";
                    receiptlines[12] = "\x1B\x21\x09" + "We are not responsible for your" + "\x1B\x21\x00" + "\n";
                    receiptlines[13] = "" + "\n";
                    receiptlines[14] = "" + "\n";
                    receiptlines[15] = "" + "\n";



                    if (receiptlines.Length > 0)
                    {
                        for (var l = 0; l < receiptlines.Length; l++)
                        {
                            string printtext = receiptlines[l];
                            if (printtext != "")
                            {
                                ObjblueToothDevicePrinting.PrintCommand(printerName, printtext);
                            }

                        }
                        var masterPage = new MasterHomePage();
                        await Navigation.PushAsync(masterPage);
                    }

                }
                else
                {
                    await DisplayAlert("Alert", "Unable to find bluetooth device", "Ok");
                }
            }
            catch (Exception ex)
            {

            }



        }
    }
}