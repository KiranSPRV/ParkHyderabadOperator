using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;

namespace ParkHyderabadOperator.DAL.DALCheckOut
{
   public class DALVehicleCheckOut
    {

        public CustomerParkingSlot VehicleCheckOut(string accessToken, CustomerParkingSlot ObjVehicleCheckOut)
        {
            CustomerParkingSlot objUpdatedVehicle=null;
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPSaveVehcileCheckOut";
                    // make the request

                    var json = JsonConvert.SerializeObject(ObjVehicleCheckOut);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                objUpdatedVehicle =  JsonConvert.DeserializeObject<CustomerParkingSlot>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objUpdatedVehicle;
        }
        public string FOCVehicleCheckOut(string accessToken, CustomerParkingSlot ObjVehicleCheckOut)
        {
           string resultMsg = null;
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPSaveVehcileCheckOut";
                    // make the request

                    var json = JsonConvert.SerializeObject(ObjVehicleCheckOut);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                resultMsg = apiResult.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resultMsg;
        }
    }
}
