using Android.Bluetooth;
using ParkHyderabadOperator.DependencyServices;

using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Print;
using Plugin.CurrentActivity;
using Android.Content;
using ParkHyderabadOperator.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidBlueToothService))]
namespace ParkHyderabadOperator.Droid.DependencyServices
{
    class AndroidBlueToothService : IBlueToothService
    {
        /// <summary>
        /// We have to use local device Bluetooth adapter.
        /// BondedDevices returns BluetoothDevice collection anyway I need to take just device name.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetDeviceList()
        {
            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                var btdevice = bluetoothAdapter?.BondedDevices.Select(i => i.Name).ToList();
                return btdevice;
            }
        }

        /// <summary>
        /// We have to use local device Bluetooth adapter.
        /// We need to find Bluetooth Device with selected device name.
        /// Now, we use BluetoothSocket class with most common UUID
        /// Try to connect BluetoothSocket then convert your text-message to bytearray
        /// Last step write your bytearray by way of bluetoothSocket
        /// </summary>
        /// <param name="deviceName">Selected deviceName</param>
        /// <param name="text">My printed text-message</param>
        /// <returns></returns>
        public async Task Print(string deviceName, string text)
        {

            using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                          where bd?.Name == deviceName
                                          select bd).FirstOrDefault();
                try
                {
                    using (BluetoothSocket bluetoothSocket = device?.CreateRfcommSocketToServiceRecord(
                        UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                    {
                        bluetoothSocket?.Connect();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
                        bluetoothSocket.OutputStream.Flush();
                        bluetoothSocket.Close();
                    }
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }

           
        }
        public async Task PrintPdfFile(Stream files,string deviceName)
        {
            try
            {
               byte[] SELECT_BIT_IMAGE_MODE = { 0x1B, 0x2A, 33, 255, 3 };
                using (BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter)
                {
                    BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                              where bd?.Name == deviceName
                                              select bd).FirstOrDefault();
                    try
                    {
                        using (BluetoothSocket bluetoothSocket = device?.CreateRfcommSocketToServiceRecord(
                            UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                        {
                            bluetoothSocket?.Connect();
                            //byte[] buffer = file;
                            // byte[] buffer = Encoding.UTF8.GetBytes("Kiran");


                            MemoryStream stream = new MemoryStream();

                            //IMAGE
                            byte[] imageData = GetImageStreamAsBytes(files);
                            stream.Write(imageData, 0, imageData.Length);
                            stream.Write(SELECT_BIT_IMAGE_MODE, 0, SELECT_BIT_IMAGE_MODE.Length);
                            var buffer = stream.ToArray();

                            bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
                            bluetoothSocket.OutputStream.Flush();
                            bluetoothSocket.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
      
    }
}