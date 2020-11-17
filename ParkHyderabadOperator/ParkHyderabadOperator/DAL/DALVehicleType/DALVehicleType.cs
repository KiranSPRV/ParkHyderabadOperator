using ParkHyderabadOperator.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkHyderabadOperator.DAL.DALVehicleType
{
    class DALVehicleType
    {
        public List<VehicleType> GetLocationLotActiveVehicleTypes(string accessToken, User objloginuserlot)
        {
            List<VehicleType> lstVehicleType = new List<VehicleType>();
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
                    string url = "api/InstaOperator/postOPAPPLocationLotActiveVehicleTypes";
                    // make the request
                    var json = JsonConvert.SerializeObject(objloginuserlot);
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
                                lstVehicleType = JsonConvert.DeserializeObject<List<VehicleType>>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return lstVehicleType;
        }
    }
}
