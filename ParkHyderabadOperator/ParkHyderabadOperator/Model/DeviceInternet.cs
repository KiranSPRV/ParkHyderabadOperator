using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace ParkHyderabadOperator.Model
{
   public class DeviceInternet
    {
       
        public static bool InternetConnected()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;

        }
    }
}
