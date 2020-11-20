using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using ParkHyderabadOperator.Model;

namespace ParkHyderabadOperator.DAL.DALCheckIn
{
  public  class DALCheckIn
    {
        public List<string> GetParkingHours()
        {
            List<string> lstParkingHours = new List<string>();

            try
            {
                //lstParkingHours.Add("00");
                lstParkingHours.Add(6.ToString("D2"));
                lstParkingHours.Add(5.ToString("D2"));
                lstParkingHours.Add(3.ToString("D2"));
                lstParkingHours.Add(2.ToString("D2"));
            }
            catch (Exception ex)
            {
            }
            return lstParkingHours;
        }
        public List<string> GetParkingExtendHours()
        {
            List<string> lstParkingHours = new List<string>();

            try
            {
                lstParkingHours.Add("00");
                for (var i = 1; i <= 5; i++)
                {
                    lstParkingHours.Add(i.ToString("D2"));
                }
            }
            catch (Exception ex)
            {
            }
            return lstParkingHours;
        }
        public List<string> GetParkingMinutes()
        {
            List<string> lstParkingMinutes = new List<string>();

            try
            {
                lstParkingMinutes.Add("00");
                lstParkingMinutes.Add("30");
            }
            catch (Exception ex)
            {
            }
            return lstParkingMinutes;
        }
        public List<ParkingBay> GetLocationParkingBay(string accessToken, LocationParkingLot LocationParkingLotID)
        {
            List<ParkingBay> lstParkingBay = new List<ParkingBay>();

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
                    string url = "api/InstaOperator/postOPAPPLocationByNumbers";
                    // make the request

                    var json = JsonConvert.SerializeObject(LocationParkingLotID);
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
                                lstParkingBay = JsonConvert.DeserializeObject<List<ParkingBay>>(Convert.ToString(apiResult.Object));
                            }

                        }

                    }


                }
            }
            catch (Exception ex)
            {
            }
            return lstParkingBay;
        }
        public VehicleParkingFee GetVehicleParkingFees(string accessToken, string VehicleTypeCode,int ParkingHours,int LocationParkingLotID,string parkingStartTime)
        {
            VehicleParkingFee objVehicleParkingFee = new VehicleParkingFee(); ;

            try
            {
                objVehicleParkingFee.VehicleTypeCode = VehicleTypeCode;
                objVehicleParkingFee.ParkingHours = ParkingHours;
                objVehicleParkingFee.LocationParkingLotID = LocationParkingLotID;
                objVehicleParkingFee.ParkingStartTime = parkingStartTime;

                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPGetVehicleParkingFee";
                    // make the request
                    var json = JsonConvert.SerializeObject(objVehicleParkingFee);
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
                                objVehicleParkingFee = JsonConvert.DeserializeObject<VehicleParkingFee>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return objVehicleParkingFee;
        }
        public List<VehicleParkingFee> GetLocationParkingLotVehicleParkingFees(string accessToken, string VehicleTypeCode, int ParkingHours, int LocationParkingLotID,decimal paidParkingFees)
        {
            VehicleParkingFee objVehicleParkingFee = new VehicleParkingFee(); ;
            List<VehicleParkingFee> lstParkingFeesDetails = new List<VehicleParkingFee>();
            try
            {
                objVehicleParkingFee.VehicleTypeCode = VehicleTypeCode;
                objVehicleParkingFee.ParkingHours = ParkingHours;
                objVehicleParkingFee.LocationParkingLotID = LocationParkingLotID;
                objVehicleParkingFee.PaidAmount = paidParkingFees;
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPGetLocaitonParkingLotVehicleParkingFee";
                    // make the request
                    var json = JsonConvert.SerializeObject(objVehicleParkingFee);
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
                                lstParkingFeesDetails = JsonConvert.DeserializeObject<List<VehicleParkingFee>>(Convert.ToString(apiResult.Object));
                            }

                        }
                    }


                }
            }
            catch (Exception ex)
            {
            }
            return lstParkingFeesDetails;
        }
        public CustomerVehiclePass GetVerifyVehicleHasPass(string accessToken, string RegistrationNumber,int LocationID,int LocationParkingLotID, int UserID,string NFCCardNumber)
        {
            CheckInVehiclePass objVehiclePass = new CheckInVehiclePass();
            CustomerVehiclePass objCustomerVehiclePass=new CustomerVehiclePass();
            try
            {

                objVehiclePass.RegistrationNumber = RegistrationNumber;
                objVehiclePass.LocationID = LocationID;
                objVehiclePass.UserID = UserID;
                objVehiclePass.LocationParkingLotID = LocationParkingLotID;
                objVehiclePass.NFCCardNumber = NFCCardNumber;

                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPVerifyVehicleHasPass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objVehiclePass);
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
            }
            return objCustomerVehiclePass;

        }
        public string SavePassVehicleCheckIn(string accessToken,VehicleCheckIn objcheckin)
        {
            string apirespone = string.Empty;

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
                    string url = "api/InstaOperator/postOPAPPPassVehicleCheckIn";
                    // make the request

                    var json = JsonConvert.SerializeObject(objcheckin);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            apirespone = apiResult.Message;

                        }

                    }


                }
            }
            catch (Exception ex)
            {
            }
            return apirespone;
        }
        public string SaveNFCCardVehicleCheckIn(string accessToken, VehicleCheckIn objcheckin)
        {
            string apirespone = string.Empty;

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
                    string url = "api/InstaOperator/postOPAPPNFCCardVehicleCheckIn";
                    // make the request

                    var json = JsonConvert.SerializeObject(objcheckin);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                            apirespone = apiResult.Message;

                        }

                    }


                }
            }
            catch (Exception ex)
            {
            }
            return apirespone;
        }
        public string SaveGovernmentVehicleCheckIn(string accessToken, VehicleCheckIn objcheckin)
        {
            string apirespone = string.Empty;

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
                    string url = "api/InstaOperator/postOPAPPGovernmentVehicleCheckIn";
                    // make the request

                    var json = JsonConvert.SerializeObject(objcheckin);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            apirespone = apiResult.Message;

                        }

                    }


                }
            }
            catch (Exception ex)
            {
            }
            return apirespone;
        }
        public CustomerParkingSlot SaveVehicleNewCheckIn(string accessToken, VehicleCheckIn objcheckin)
        {
            CustomerParkingSlot objResultCustomerParkingSlot = new CustomerParkingSlot();
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
                    string url = "api/InstaOperator/postOPAPPVehicleNewCheckIn";
                    // make the request

                    var json = JsonConvert.SerializeObject(objcheckin);
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
                                objResultCustomerParkingSlot = JsonConvert.DeserializeObject<CustomerParkingSlot>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objResultCustomerParkingSlot;
        }
        public CustomerParkingSlot VerifyVehicleChekcInStatus(string accessToken, VehicleCheckIn objcheckin)
        {
            CustomerParkingSlot objResultCustomerParkingSlot = new CustomerParkingSlot();
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
                    string url = "api/InstaOperator/postOPAPPVerifyVehicleInCheckInStatus";
                    // make the request

                    var json = JsonConvert.SerializeObject(objcheckin);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                            if(apiResult.Result)
                            {
                                objResultCustomerParkingSlot = JsonConvert.DeserializeObject<CustomerParkingSlot>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objResultCustomerParkingSlot;
        }


    }
}
