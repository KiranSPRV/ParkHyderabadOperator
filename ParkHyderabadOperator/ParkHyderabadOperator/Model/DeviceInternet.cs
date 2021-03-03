using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<string> CheckInternetSpeed()
        {
            DateTime dt1 = DateTime.Now;
            string internetSpeed;
            try
            {
                var client = new HttpClient();
                byte[] data = await client.GetByteArrayAsync("http://xamarinmonkeys.blogspot.com/");
                DateTime dt2 = DateTime.Now;
                internetSpeed = "ConnectionSpeed: (kb/s) " + Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
            }
            catch (Exception ex)
            {
                internetSpeed = "ConnectionSpeed:Unknown Exception-" + ex.Message;
            }
            return internetSpeed;
        }
       
    }
}
