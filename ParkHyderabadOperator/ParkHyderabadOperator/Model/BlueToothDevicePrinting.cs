
using ParkHyderabadOperator.DependencyServices;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace ParkHyderabadOperator.Model
{
    public class BlueToothDevicePrinting
    {
        private readonly IBlueToothService _blueToothService;
        public BlueToothDevicePrinting()
        {
            _blueToothService = DependencyService.Get<IBlueToothService>();
        }
        public IList<string> BlueToothDeviceList { get; set; }
        public string DefaultSelectedDevice { get; set; }
        public string GetBlueToothDevices()
        {
            DefaultSelectedDevice = string.Empty;
            try
            {
                IList<string> list = _blueToothService.GetDeviceList();
                if (list.Count > 0)
                {
                    DefaultSelectedDevice = Convert.ToString(list[0]);
                }
            }
            catch (Exception ex)
            {

            }
            return DefaultSelectedDevice;

        }
        public async void PrintCommand(string printerName,string printText)
        
        {
            try
            {
                await _blueToothService.Print(printerName, printText);
            }
            catch (Exception ex)

            {

            }

        }

        public async void PrintPdfFile(Stream imgfile, string printerName)

        {
            try
            {
                await _blueToothService.PrintPdfFile(imgfile, printerName);
            }
            catch (Exception ex)

            {

            }

        }
    }
}
