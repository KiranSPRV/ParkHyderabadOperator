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
    public class DALPass
    {
        DALExceptionManagment dal_DALExceptionManagment;
        public  DALPass()
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
        }
        public List<PassPrice> GetPassPriceDetails(string accessToken, VehicleLotPassPrice objVehicleType)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();

            List<PassPrice> lstVMPass = null;
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
                    string url = "api/InstaOperator/postOPAPPPassPriceDetails";
                    // make the request

                    var json = JsonConvert.SerializeObject(objVehicleType);
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
                                lstVMPass = JsonConvert.DeserializeObject<List<PassPrice>>(Convert.ToString(apiResult.Object));
                            }

                        }

                    }


                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetPassPriceDetails");
            }
            return lstVMPass;
        }
        public CustomerVehiclePass CreateCustomerPass(string accessToken, CustomerVehiclePass objInsertPass)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();

            CustomerVehiclePass objCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/postSaveCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objInsertPass);
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
                                objCustomerVehiclePass = JsonConvert.DeserializeObject<CustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }

                        }

                    }


                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "CreateCustomerPass");
            }
            return objCustomerVehiclePass;
        }
        public CustomerVehiclePass CreateMultiStationCustomerPass(string accessToken, VMMultiStationCustomerVehiclePass objInsertPass)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();

            CustomerVehiclePass objCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/postSaveMultiStationCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objInsertPass);
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
                                List<CustomerVehiclePass> resultobj = JsonConvert.DeserializeObject<List<CustomerVehiclePass>>(Convert.ToString(apiResult.Object));
                                if(resultobj.Count>0)
                                {
                                    objCustomerVehiclePass = resultobj[0];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "CreateCustomerPass");
            }
            return objCustomerVehiclePass;
        }
        public bool CheckVehiclePassDuplication(string accessToken, CustomerVehiclePass objInsertPass)
        {
            bool IsVehiclehasPass = false;
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
                    string url = "api/InstaOperator/postVerifyVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objInsertPass);
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
                                IsVehiclehasPass = apiResult.Result;
                            }

                        }

                    }


                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DailyPass.xaml.cs", "", "VerifyVehiclePassDuration");
            }
            return IsVehiclehasPass;

        }
        public CustomerVehiclePass GetCustomerVehicleDetailsByVehicle(string accessToken, CustomerVehicle objvehicle)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
            CustomerVehiclePass objCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/postOPAPPGetCustomerVehicleDetailsByVehicle";
                    // make the request

                    var json = JsonConvert.SerializeObject(objvehicle);
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
                                objCustomerVehiclePass = JsonConvert.DeserializeObject<CustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetCustomerVehicleDetailsByVehicle");
            }
            return objCustomerVehiclePass;
        }
        public CustomerVehiclePass GetCustomerVehiclePassDetails(string accessToken, CustomerVehiclePass objvehiclepass)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
            CustomerVehiclePass objCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/getCustomerVehiclePassDetails";
                    // make the request
                    var json = JsonConvert.SerializeObject(objvehiclepass);
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
                                objCustomerVehiclePass = JsonConvert.DeserializeObject<CustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetCustomerVehicleDetailsByVehicle");
            }
            return objCustomerVehiclePass;
        }
        public CustomerVehiclePass ActivateCustomerVehiclePass(string accessToken, CustomerVehiclePass objvehicle)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
            CustomerVehiclePass objActivatedCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/postOPAPPActivateCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objvehicle);
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
                                objActivatedCustomerVehiclePass = JsonConvert.DeserializeObject<CustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetCustomerVehicleDetailsByVehicle");
            }
            return objActivatedCustomerVehiclePass;
        }
        public CustomerVehiclePass SaveCustomerVehiclePassNewNFCCard(string accessToken, CustomerVehiclePass objvehicle)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
            CustomerVehiclePass objActivatedCustomerVehiclePass = null;
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
                    string url = "api/InstaOperator/postSaveCustomerVehiclePassNewNFCCard";
                    // make the request

                    var json = JsonConvert.SerializeObject(objvehicle);
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
                                objActivatedCustomerVehiclePass = JsonConvert.DeserializeObject<CustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetCustomerVehicleDetailsByVehicle");
            }
            return objActivatedCustomerVehiclePass;
        }
        public List<CustomerVehiclePass> GetCustomerValidateVehiclePassesDetailsByVehicle(string accessToken, CustomerVehicle objvehicle)
        {
            dal_DALExceptionManagment = new DALExceptionManagment();
            CustomerVehiclePass objCustomerVehiclePass = null;
            List<CustomerVehiclePass> lstCustomerVehiclePass = new List<CustomerVehiclePass>();
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
                    string url = "api/InstaOperator/postValidateVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objvehicle);
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
                                lstCustomerVehiclePass = JsonConvert.DeserializeObject<List<CustomerVehiclePass>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dal_DALExceptionManagment.InsertException(accessToken, "OperatarAPP", ex.Message, "DALPass", "", "GetCustomerVehicleDetailsByVehicle");
            }
            return lstCustomerVehiclePass;
        }
    }
}
