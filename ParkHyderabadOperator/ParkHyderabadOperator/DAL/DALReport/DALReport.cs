using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using ParkHyderabadOperator.Model.LotOccupancy;
using ParkHyderabadOperator.Model.RecentCheckOuts;
using ParkHyderabadOperator.ViewModel;
using ParkHyderabadOperator.ViewModel.Reports;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkHyderabadOperator.DAL.DALReport
{
    public class DALReport
    {
        public VMReportSummary GetLocationLotReport(string accessToken, User  objSelectedUser)
        {
            VMReportSummary result = null;
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
                    string url = "api/InstaOperator/postLocationLotReport";
                    // make the request
                    var json = JsonConvert.SerializeObject(objSelectedUser);
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
                                result = JsonConvert.DeserializeObject<VMReportSummary>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public RecentCheckOutReport GetRecentCheckOutReport(string accessToken, RecentCheckOutFilter objfilter)
        {
            RecentCheckOutReport result = null;
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
                    string url = "api/InstaOperator/postRecentCheckOutReport";
                    // make the request
                    var json = JsonConvert.SerializeObject(objfilter);
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
                                result = JsonConvert.DeserializeObject<RecentCheckOutReport>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public List<LocationLotOccupancyReport> GetLocationLotOccupancyReport(string accessToken, User objSelectedUser)
        {
            List<LocationLotOccupancyReport> result = null;
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
                    string url = "api/InstaOperator/postLotOccupancyReport";
                    // make the request
                    var json = JsonConvert.SerializeObject(objSelectedUser);
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
                                result = JsonConvert.DeserializeObject<List<LocationLotOccupancyReport>>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public VMLocationLotOccupancyReport VMGetLocationLotOccupancyReport(string accessToken, User objSelectedUser)
        {
            VMLocationLotOccupancyReport result = null;
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
                    string url = "api/InstaOperator/postLotOccupancyReport";
                    // make the request
                    var json = JsonConvert.SerializeObject(objSelectedUser);
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
                                result = JsonConvert.DeserializeObject<VMLocationLotOccupancyReport>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}
