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
        public List<CustomerVehicle> GetAllVehicleRegistrationNumbersBySearch(string accessToken,string RegistrationNumber)
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
                    string url = "api/InstaOperator/getAllVehicleRegistrationNumbersBySearch?RegistrationNumber=" + Convert.ToString(RegistrationNumber);
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
        public List<CustomerParkingSlot> GetVehicleRecentCheckOutDetails(string accessToken, int CustomerParkingSlotID)
        {
            List<CustomerParkingSlot> lstCustomerParkingSlot = new List<CustomerParkingSlot>();
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
                    string url = "api/InstaOperator/getRecentCheckOutVehicleDetails?CustomerParkingSlotID=" + Convert.ToString(CustomerParkingSlotID);
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
                                lstCustomerParkingSlot = JsonConvert.DeserializeObject<List<CustomerParkingSlot>>(Convert.ToString(apiResult.Object));

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstCustomerParkingSlot;
        }


        #region Customer Vehicle Due Amount History
        public List<CustomerParkingSlot> GetVehicleDueAmountHistory(string accessToken, string RegistrationNumber, string VehicleTypeCode)
        {
            List<CustomerParkingSlot> lstDueAmounthistory = new List<CustomerParkingSlot>();
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
                    string url = "api/InstaOperator/getVehicleDueAmountHistory?RegistrationNumber=" + Convert.ToString(RegistrationNumber)+"&VehicleTypeCode=" + Convert.ToString(VehicleTypeCode);
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
                                lstDueAmounthistory = JsonConvert.DeserializeObject<List<CustomerParkingSlot>>(Convert.ToString(apiResult.Object));

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstDueAmounthistory;
        }
        public decimal GetVehicleDueAmount(string accessToken, string RegistrationNumber, string VehicleTypeCode)
        {
            decimal DueAmount = 0;
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
                    string url = "api/InstaOperator/getVehicleDueAmount?RegistrationNumber=" + Convert.ToString(RegistrationNumber) + "&VehicleTypeCode=" + Convert.ToString(VehicleTypeCode);
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

                                DueAmount = apiResult.Object==null?0:Convert.ToDecimal( apiResult.Object);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return DueAmount;
        }

        #endregion


    }
}
