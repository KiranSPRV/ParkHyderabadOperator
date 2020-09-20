using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ParkHyderabadOperator.DAL
{
    public class GeocodingDetails
    {
        private float Latitude;
        private float Longitude;
        public async Task<Xamarin.Essentials.Location> GetCurrentLocation()
        {
            Xamarin.Essentials.Location objlocation = null;
            try
            {

                objlocation = await Geolocation.GetLastKnownLocationAsync();
                if (objlocation != null)
                {
                    Latitude = (float)objlocation.Latitude;
                    Longitude = (float)objlocation.Longitude;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objlocation;
        }
        public async Task<string> GetGeoCodingPlaceMark(double Latitude, double Longitude)
        {
            string geocodeAddress = string.Empty;
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(Latitude, Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    geocodeAddress =  placemark.SubLocality+"-"+placemark.Locality + " "+ placemark.PostalCode;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geocodeAddress;
        }
    }
}
