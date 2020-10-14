using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using ParkHyderabadOperator.ViewModel.Reports;
using ParkHyderabadOperator.Model.RecentCheckOuts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkHyderabadOperator.DAL.DALMenuBar
{
    public class DALMenubar
    {

        public List<CustomerVehicle> GetAllVehicleRegistrationNumbers(string accessToken)
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
                    string url = "api/InstaOperator/getAllVehicleRegistrationNumbers";
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
        public List<CustomerParkingSlot> GetVehicleParkingHistory(string accessToken, CustomerVehicle objCustomerVehicle)
        {
            List<CustomerParkingSlot> lsthistory = new List<CustomerParkingSlot>();
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
                    string url = "api/InstaOperator/postVehicleParkingHistory";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
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
                                lsthistory = JsonConvert.DeserializeObject<List<CustomerParkingSlot>>(Convert.ToString(apiResult.Object));

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lsthistory;
        }
        public VMUserDailyLogin GetUserDailyLoginHistory(string accessToken, UserDailyLogin objDailyLogin)
        {
            VMUserDailyLogin objtimesheet = new VMUserDailyLogin();
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
                    string url = "api/InstaOperator/postOPAPPUserDailyLoginHistory";
                    // make the request

                    var json = JsonConvert.SerializeObject(objDailyLogin);
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
                                objtimesheet = JsonConvert.DeserializeObject<VMUserDailyLogin>(Convert.ToString(apiResult.Object));
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objtimesheet;
        }
        public string UpdateUserDailyLogOut(string accessToken, User objUser)
        {
            string resultmsg=string.Empty;
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
                    string url = "api/InstaOperator/postOPAPPUpdateUserDailyLogOut";
                    // make the request

                    var json = JsonConvert.SerializeObject(objUser);
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
                                resultmsg = apiResult.Message;
                            }

                        }

                    }


                }
            }
            catch (Exception ex)
            {
            }
            return resultmsg;
        }

        public List<RecentCheckOutDays> GetRecentCheckOutDays()
        {
            List<RecentCheckOutDays> lstRecentCheckOutDays = new List<RecentCheckOutDays>();
            try
            {
                lstRecentCheckOutDays.Add(new RecentCheckOutDays() { DayID = 1, Day = "Today" });
                lstRecentCheckOutDays.Add(new RecentCheckOutDays() { DayID = 2, Day = "Yesterday" });
                lstRecentCheckOutDays.Add(new RecentCheckOutDays() { DayID = 3, Day = "Daybefore Yesterday" });
            }
            catch (Exception ex)
            {
            }
            return lstRecentCheckOutDays;
        }
    }
}
