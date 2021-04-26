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
using System.Linq;
using System.Threading.Tasks;
using ParkHyderabadOperator.DAL.DALExceptionLog;
using ParkHyderabadOperator.ViewModel;
using System.Diagnostics;

namespace ParkHyderabadOperator.DAL.DALCheckIn
{
    public class DALCheckIn
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
        public List<VehicleParkingFee> GetVehicleParkingFeesDetailsOffline(string vehicleTypeCode, int Hours)
        {
            List<VehicleParkingFee> lstVehicleParkingFee = new List<VehicleParkingFee>();
            try
            {
                lstVehicleParkingFee = Task.Run(async () => await App.SQLiteDb.GetLotVehiclesParkingFeesSQLLite()).Result;
            }
            catch (Exception ex)
            {
            }
            if (!string.IsNullOrEmpty(vehicleTypeCode) && Hours != 0)
            {
                lstVehicleParkingFee = lstVehicleParkingFee.Where(i => i.VehicleTypeCode == vehicleTypeCode && i.Duration == Hours).ToList();

            }
            return lstVehicleParkingFee;
        }
        public List<VehicleParkingFee> GetLotVehiclesParkingFeesDetailOnLogin(string accessToken, int LocationParkingLotID)
        {
            VehicleParkingFee objVehicleParkingFee = new VehicleParkingFee(); ;
            List<VehicleParkingFee> lstParkingFeesDetails = new List<VehicleParkingFee>();
            try
            {
                objVehicleParkingFee.LocationParkingLotID = LocationParkingLotID;
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
        public List<ParkingBay> GetLocationParkingBayOffline()
        {
            List<ParkingBay> lstParkingBay = new List<ParkingBay>();

            try
            {
                lstParkingBay.Add(new ParkingBay() { ParkingBayID = 1, ParkingBayRange = "A1-A10", ParkingBayName = "A1-A10", IsActive = true });
                lstParkingBay.Add(new ParkingBay() { ParkingBayID = 2, ParkingBayRange = "A11-A20", ParkingBayName = "A11-A20", IsActive = true });

            }
            catch (Exception ex)
            {
            }
            return lstParkingBay;
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
        public VehicleParkingFee GetVehicleParkingFees(string accessToken, string VehicleTypeCode, int ParkingHours, int LocationParkingLotID, string parkingStartTime)
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
        public List<VehicleParkingFee> GetLocationParkingLotVehicleParkingFees(string accessToken, string VehicleTypeCode, int ParkingHours, int LocationParkingLotID, decimal paidParkingFees)
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
        public CustomerVehiclePass GetVerifyVehicleHasPass(string accessToken, string RegistrationNumber, int LocationID, int LocationParkingLotID, int UserID, string NFCCardNumber)
        {
            CheckInVehiclePass objVehiclePass = new CheckInVehiclePass();
            CustomerVehiclePass objCustomerVehiclePass = new CustomerVehiclePass();
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

        public VMVehiclePassWithDueAmount GetVerifyVehicleHasPassWithDueAmount(string accessToken, string RegistrationNumber, string VehicleTypeCode, int LocationID, int LocationParkingLotID, int UserID, string NFCCardNumber)
        {
            CheckInVehiclePass objVehiclePass = new CheckInVehiclePass();
            VMVehiclePassWithDueAmount objCustomerVehiclePass = new VMVehiclePassWithDueAmount();
            try
            {

                objVehiclePass.RegistrationNumber = RegistrationNumber;
                objVehiclePass.VehicleTypeCode = VehicleTypeCode;
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
                    string url = "api/InstaOperator/postOPAPPVehicleCheckInVerifyVehicleHasPass";
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
                                objCustomerVehiclePass = JsonConvert.DeserializeObject<VMVehiclePassWithDueAmount>(Convert.ToString(apiResult.Object));
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
        public string SavePassVehicleCheckIn(string accessToken, VehicleCheckIn objcheckin)
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
                throw ex;
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

        #region Offline-Sync
        public async Task<string> CheckInOfflineSync(string apitoken, User loguser)
        {
            int totalCounts = 0;
            StringBuilder sbUnMoved = new StringBuilder();
            DALExceptionManagment dal_Exceptionlog = new DALExceptionManagment();
            try
            {
                var lstchekIns = await App.SQLiteDb.GetAllVehicleAsync();
                if (lstchekIns != null)
                {
                    if (lstchekIns.Count > 0)
                    {
                        foreach (var items in lstchekIns)
                        {
                            if (DeviceInternet.InternetConnected())
                            {
                                var objexlog = new OfflineSyncLog();
                                try
                                {
                                    var resultCustomerID = SaveVehicleNewCheckIn(apitoken, items);
                                    if (resultCustomerID != null)
                                    {
                                        try
                                        {
                                            objexlog.CustomerParkingSlotID = resultCustomerID.CustomerParkingSlotID;
                                            objexlog.RegistrationNumber = items.RegistrationNumber;
                                            objexlog.LocationParkingLotName = loguser.LocationParkingLotID.LocationParkingLotName;
                                            objexlog.LocationParkingLotID = loguser.LocationParkingLotID.LocationParkingLotID;
                                            objexlog.CreatedBy = resultCustomerID.CreatedBy;
                                            if (resultCustomerID.CustomerParkingSlotID != 0)
                                            {
                                                objexlog.ExceptionMessage = "Success";
                                                objexlog.IsSync = true;
                                                if (!string.IsNullOrEmpty(items.ParkingStartTime) && !string.IsNullOrEmpty(items.ParkingEndTime))
                                                {
                                                    objexlog.ExpectedStartTime = Convert.ToDateTime(items.ParkingStartTime);
                                                    objexlog.ExpectedEndTime = Convert.ToDateTime(items.ParkingEndTime);
                                                }
                                            }
                                            else
                                            {
                                                if (sbUnMoved.Length == 0)
                                                {
                                                    sbUnMoved.AppendLine(items.RegistrationNumber);
                                                }
                                                else
                                                {
                                                    sbUnMoved.AppendLine(", " + items.RegistrationNumber);
                                                }
                                                objexlog.ExceptionMessage = "Failed";
                                                objexlog.IsSync = false;
                                                DeSyncVehicleCheckIn objdesync = new DeSyncVehicleCheckIn();
                                                objdesync.VehicleTypeCode = items.VehicleTypeCode;
                                                objdesync.BayRange = items.BayRange;
                                                objdesync.RegistrationNumber = items.RegistrationNumber;
                                                objdesync.LocationParkingLotName = items.LocationParkingLotName;
                                                await App.SQLiteDb.SaveDeSyncCheckInAsync(objdesync);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            objexlog.ExceptionMessage = "Failed";
                                            objexlog.IsSync = false;
                                            dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message + ",At:" + "Item after Online Sync" + resultCustomerID.CustomerParkingSlotID, "DALCheckIn.cs", items.RegistrationNumber + "," + items.ParkingStartTime + "," + items.ParkingEndTime + "," + "", "CheckInOfflineSync");
                                        }
                                        finally
                                        {
                                            dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", "Finally at:" + "Item after Online Sync" + resultCustomerID.CustomerParkingSlotID, "DALCheckIn.cs", items.RegistrationNumber + "," + items.ParkingStartTime + "," + items.ParkingEndTime + "," + "", "CheckInOfflineSync");
                                            dal_Exceptionlog.InsertOfflineSynchException(Convert.ToString(App.Current.Properties["apitoken"]), objexlog);
                                            await App.SQLiteDb.DeleteItemAsync(items);
                                        }
                                    }
                                    else
                                    {
                                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", "No Records found ", "DALCheckIn.cs", "", "CheckInOfflineSync");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    if (sbUnMoved.Length == 0)
                                    {
                                        sbUnMoved.AppendLine(items.RegistrationNumber);
                                    }
                                    else
                                    {
                                        sbUnMoved.AppendLine(", " + items.RegistrationNumber);
                                    }
                                    await App.SQLiteDb.DeleteItemAsync(items);

                                    dal_Exceptionlog.InsertOfflineSynchException(Convert.ToString(App.Current.Properties["apitoken"]), objexlog);
                                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DALCheckIn.cs", items.RegistrationNumber + "," + items.ParkingStartTime + "," + items.ParkingEndTime, "CheckInOfflineSync");

                                }
                            }
                            else
                            {
                                if (sbUnMoved.Length == 0)
                                {
                                    sbUnMoved.AppendLine(items.RegistrationNumber);
                                }
                                else
                                {
                                    sbUnMoved.AppendLine(", " + items.RegistrationNumber);
                                }
                            }
                        }
                    }
                    else
                    {
                        dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", "No Records in VehicleCheckIn", "DALCheckIn.cs", "", "CheckInOfflineSync");
                    }
                }
                else
                {
                    dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", "No Records in VehicleCheckIn", "DALCheckIn.cs", "", "CheckInOfflineSync");
                }
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                //Get the first stack frame
                StackFrame frame = st.GetFrame(0);

                //Get the file name
                string fileName = frame.GetFileName();

                //Get the method name
                string methodName = frame.GetMethod().Name;

                //Get the line number from the stack frame
                int line = frame.GetFileLineNumber();

                string exDetails = fileName + "," + methodName + "," + line;

                dal_Exceptionlog.InsertException(Convert.ToString(App.Current.Properties["apitoken"]), "Operator App", ex.Message, "DALCheckIn.cs", exDetails, "CheckInOfflineSync");
            }
            return sbUnMoved.ToString();
        }
        #endregion

        // Receipt SMS
        public bool SendReceiptToMobile(string ReceiptMsg, string mobile,string template_id)
        {
            bool IsOTPSended = false;
            try
            {

                string baseUrl = "https://www.smsstriker.com/";
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    // create the URL string.

                    string url = "API/sms.php?username=sprvtechnology&password=128391&from=INSPRK&to=" + mobile + " &msg= " + ReceiptMsg + "&type=1&template_id="+ template_id + "";
                    // make the request
                    // make the request
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        IsOTPSended = true;

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return IsOTPSended;
        }
    }
}
