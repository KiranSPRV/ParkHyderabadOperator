using Newtonsoft.Json;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using ParkHyderabadOperator.Model.Pass;
using ParkHyderabadOperator.ViewModel.VMPass;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkHyderabadOperator.DAL.DALPass
{
   public class DALReNewPass
    {
        DALExceptionManagment dal_DALExceptionManagment;
        public List<CustomerVehicle> GetAllPassedVehicles(string accessToken)
        {
            List<CustomerVehicle> lstCustomerVehicle = new List<CustomerVehicle>();
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
                    string url = "api/InstaOperator/getOPAPPGetAllPassVehicles";
                    // make the request
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                            if (apiResult.Result)
                            {
                                lstCustomerVehicle = JsonConvert.DeserializeObject<List<CustomerVehicle>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstCustomerVehicle;
        }
    }
}
