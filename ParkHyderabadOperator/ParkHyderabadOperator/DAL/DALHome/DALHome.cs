using Newtonsoft.Json;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using ParkHyderabadOperator.ViewModel.VMHome;
using ParkHyderabadOperator.ViewModel.VMPass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ParkHyderabadOperator.DAL.DALHome
{
    public class DALHome
    {
        public List<VMLocationLots> GetUserAllocatedLocationAndLots(string accessToken, User objUser)
        {
            List<LocationParkingLot> lstParkingLots = new List<LocationParkingLot>();
            List<VMLocationLots> lstVMLocationLots = new List<VMLocationLots>();
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
                    string url = "api/InstaOperator/postOPAPPLoginUserAllocatedLocationAndLots";
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
                                lstParkingLots = JsonConvert.DeserializeObject<List<LocationParkingLot>>(Convert.ToString(apiResult.Object));
                                if (lstParkingLots.Count > 0)
                                {
                                    for (var i = 0; i < lstParkingLots.Count; i++)
                                    {
                                        VMLocationLots objlot = new VMLocationLots();
                                        objlot.LocationParkingLotID = lstParkingLots[i].LocationParkingLotID;
                                        objlot.LotName = lstParkingLots[i].LocationParkingLotName;
                                        objlot.LocationParkingLotName = lstParkingLots[i].LocationID.LocationName + "-" + lstParkingLots[i].LocationParkingLotName;
                                        objlot.LocationID = lstParkingLots[i].LocationID.LocationID;
                                        objlot.LocationName = lstParkingLots[i].LocationID.LocationName;
                                        objlot.IsActive = lstParkingLots[i].IsActive;
                                        objlot.LotOpenTime = lstParkingLots[i].LotOpenTime;
                                        objlot.LotCloseTime = lstParkingLots[i].LotCloseTime;
                                        objlot.LotVehicleAvailabilityName = lstParkingLots[i].LotVehicleAvailabilityName;
                                        lstVMLocationLots.Add(objlot);
                                    }


                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstVMLocationLots;
        }
        public List<VMLocationLots> GetUserAllocatedLocationAndLotsOffline(User objUser)
        {
            List<LocationParkingLot> lstParkingLots = new List<LocationParkingLot>();
            List<VMLocationLots> lstVMLocationLots = new List<VMLocationLots>();
            try
            {

                VMLocationLots objlot = new VMLocationLots();
                objlot.LocationParkingLotID = objUser.LocationParkingLotID.LocationParkingLotID;
                objlot.LotName = objUser.LocationParkingLotID.LocationParkingLotName;
                objlot.LocationParkingLotName = objUser.LocationParkingLotID.LocationID.LocationName + "-" + objUser.LocationParkingLotID.LocationParkingLotName;
                objlot.LocationID = objUser.LocationParkingLotID.LocationID.LocationID;
                objlot.LocationName = objUser.LocationParkingLotID.LocationID.LocationName;
                objlot.IsActive = objUser.LocationParkingLotID.IsActive;
                objlot.LotOpenTime = objUser.LocationParkingLotID.LotOpenTime;
                objlot.LotCloseTime = objUser.LocationParkingLotID.LotCloseTime;
                lstVMLocationLots.Add(objlot);

            }
            catch (Exception ex)
            {
            }
            return lstVMLocationLots;
        }
        public List<User> GetAllOperatorsOfSupervisor(string accessToken, User objSuperVisor)
        {
            List<User> lstOperators = new List<User>();

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
                    string url = "api/InstaOperator/postSupervisorOperators";
                    // make the request

                    var json = JsonConvert.SerializeObject(objSuperVisor);
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
                                lstOperators = JsonConvert.DeserializeObject<List<User>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstOperators;
        }

        public List<User> GetLocationLotActiveOperators(string accessToken, User objLoginUserSelectLocationLot)
        {
            List<User> lstOperators = new List<User>();

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
                    string url = "api/InstaOperator/postOPAPPLocationLotActiveOperartor";
                    // make the request

                    var json = JsonConvert.SerializeObject(objLoginUserSelectLocationLot);
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
                                lstOperators = JsonConvert.DeserializeObject<List<User>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstOperators;
        }
        public VMLocationLotParkedVehicles GetAllParkedVehicles(string accessToken, ParkedVehiclesFilter objparkedVehicles)
        {
            VMLocationLotParkedVehicles objVMLocationLotParkedVehicles = new VMLocationLotParkedVehicles();
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
                    string url = "api/InstaOperator/postOPAPPAllParkedVehicleDetails";
                    // make the request

                    var json = JsonConvert.SerializeObject(objparkedVehicles);
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
                                var resultobj = JsonConvert.DeserializeObject<VMLocationLotParkedVehicles>(Convert.ToString(apiResult.Object));
                                objVMLocationLotParkedVehicles.CustomerParkingSlotID = resultobj.CustomerParkingSlotID;
                                objVMLocationLotParkedVehicles.TotalTwoWheeler = resultobj.TotalTwoWheeler;
                                objVMLocationLotParkedVehicles.TotalFourWheeler = resultobj.TotalFourWheeler;
                                objVMLocationLotParkedVehicles.TotalOutTwoWheeler = resultobj.TotalOutTwoWheeler;
                                objVMLocationLotParkedVehicles.TotalOutFourWheeler = resultobj.TotalOutFourWheeler;
                                objVMLocationLotParkedVehicles.TotalHVWheeler = resultobj.TotalHVWheeler;
                                objVMLocationLotParkedVehicles.TotalOutHVWheeler = resultobj.TotalOutHVWheeler;
                                objVMLocationLotParkedVehicles.TotalThreeWheeler = resultobj.TotalThreeWheeler;
                                objVMLocationLotParkedVehicles.TotalOutThreeWheeler = resultobj.TotalOutThreeWheeler;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objVMLocationLotParkedVehicles;
        }


        public CustomerParkingSlot GetSelectedParkedVehicleDetails(string accessToken, int CustomerParkingSlotID)
        {
            CustomerParkingSlot objCustomerParkingSlot = new CustomerParkingSlot();
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
                    string url = "api/InstaOperator/getSelectedParkedVehicleDetails?CustomerParkingSlotID=" + Convert.ToString(CustomerParkingSlotID);
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
                                objCustomerParkingSlot = JsonConvert.DeserializeObject<CustomerParkingSlot>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return objCustomerParkingSlot;
        }

        public List<VMMultiLocations> GetAllLocations(string accessToken)
        {
            List<Location> lstLocations = new List<Location>();
            List<VMMultiLocations> lstVMLocations = new List<VMMultiLocations>();
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
                    string url = "api/InstaOperator/getOPAPPGetAllLocations";
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
                                lstLocations = JsonConvert.DeserializeObject<List<Location>>(Convert.ToString(apiResult.Object));


                                if (lstLocations.Count > 0)
                                {
                                    for (var i = 0; i < lstLocations.Count; i++)
                                    {
                                        VMMultiLocations objvm = new VMMultiLocations();
                                        objvm.LocationID = lstLocations[i].LocationID;
                                        objvm.LocationCode = Convert.ToString(lstLocations[i].LocationCode);
                                        objvm.LocationName = Convert.ToString(lstLocations[i].LocationName);
                                        lstVMLocations.Add(objvm);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstVMLocations;
        }
        public List<VMMultiLocations> GetAllLocationsByVehicleType(string accessToken, string VehicleTypeCode)
        {
            List<Location> lstLocations = new List<Location>();
            List<VMMultiLocations> lstVMLocations = new List<VMMultiLocations>();
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
                    string url = "api/InstaOperator/getOPAPPGetAllLocationsByVehicleType?VehicleTypeCode=" + VehicleTypeCode;
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
                                lstLocations = JsonConvert.DeserializeObject<List<Location>>(Convert.ToString(apiResult.Object));


                                if (lstLocations.Count > 0)
                                {
                                    for (var i = 0; i < lstLocations.Count; i++)
                                    {
                                        VMMultiLocations objvm = new VMMultiLocations();
                                        objvm.LocationID = lstLocations[i].LocationID;
                                        objvm.LocationCode = Convert.ToString(lstLocations[i].LocationCode);
                                        objvm.LocationName = Convert.ToString(lstLocations[i].LocationName);
                                        lstVMLocations.Add(objvm);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstVMLocations;
        }
        public List<VMMultiLocations> GetAllPassLocationsByVehicleType(string accessToken, string VehicleTypeCode, int CustomerVehiclePassId)
        {
            List<Location> lstLocations = new List<Location>();
            List<VMMultiLocations> lstVMLocations = new List<VMMultiLocations>();
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
                    string url = "api/InstaOperator/getOPAPPGetAllPassLocationsByVehicleType?VehicleTypeCode=" + VehicleTypeCode + "&CustomerVehiclePassId=" + CustomerVehiclePassId;
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
                                lstLocations = JsonConvert.DeserializeObject<List<Location>>(Convert.ToString(apiResult.Object));


                                if (lstLocations.Count > 0)
                                {
                                    for (var i = 0; i < lstLocations.Count; i++)
                                    {
                                        VMMultiLocations objvm = new VMMultiLocations();
                                        objvm.LocationID = lstLocations[i].LocationID;
                                        objvm.LocationCode = Convert.ToString(lstLocations[i].LocationCode);
                                        objvm.LocationName = Convert.ToString(lstLocations[i].LocationName);
                                        lstVMLocations.Add(objvm);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstVMLocations;
        }
        public List<ApplicationType> GetAllApplicationTypes(string accessToken)
        {
            List<ApplicationType> lstApplicationType = new List<ApplicationType>();
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
                    string url = "api/InstaOperator/getAllApplicationTypes";
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
                                lstApplicationType = JsonConvert.DeserializeObject<List<ApplicationType>>(Convert.ToString(apiResult.Object));


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstApplicationType;
        }
        public List<Status> GetAllStatus(string accessToken)
        {
            List<Status> lstStatus = new List<Status>();
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
                    string url = "api/InstaOperator/getAllStatus";
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
                                lstStatus = JsonConvert.DeserializeObject<List<Status>>(Convert.ToString(apiResult.Object));


                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return lstStatus;
        }


        #region Offline Functions
        public VMLocationLotParkedVehicles GetAllParkedVehiclesOffline()
        {
            VMLocationLotParkedVehicles objVMLocationLotParkedVehicles = new VMLocationLotParkedVehicles();
            List<LocationLotParkedVehicles> lstCustomerParkingSlot = new List<LocationLotParkedVehicles>();
            try
            {

                // Get Records From SQLLite
                var lstchekIns = Task.Run(async () => await App.SQLiteDb.GetAllVehicleAsync()
                                ).Result;
                if (lstchekIns.Count > 0)
                {
                    foreach (var items in lstchekIns)
                    {
                        LocationLotParkedVehicles objCustomerParkingSlot = new LocationLotParkedVehicles();
                        objCustomerParkingSlot.VehicleTypeCode = items.VehicleTypeCode;
                        if (items.VehicleTypeCode == "2W")
                        {
                            objCustomerParkingSlot.VehicleImage = "bike_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        else if (items.VehicleTypeCode == "4W")
                        {
                            objCustomerParkingSlot.VehicleImage = "car_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        else if (items.VehicleTypeCode == "3W")
                        {
                            objCustomerParkingSlot.VehicleImage = "ThreeW_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        else if (items.VehicleTypeCode == "HW")
                        {
                            objCustomerParkingSlot.VehicleImage = "hv_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        objCustomerParkingSlot.RegistrationNumber = items.RegistrationNumber;
                        objCustomerParkingSlot.ParkingBayName = items.BayRange;
                        objCustomerParkingSlot.ParkingBayRange = items.BayRange;
                        objCustomerParkingSlot.ApplicationTypeCode = "O";
                        lstCustomerParkingSlot.Add(objCustomerParkingSlot);
                    }
                    //Count Two Wheeler
                    if (lstCustomerParkingSlot.Count > 0)
                    {
                        var twoCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "2W");
                        var fourCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "4W");
                        var threeCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "3W");
                        var heavyCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "HW");
                        objVMLocationLotParkedVehicles.CustomerParkingSlotID = lstCustomerParkingSlot;
                        objVMLocationLotParkedVehicles.TotalTwoWheeler = twoCount;
                        objVMLocationLotParkedVehicles.TotalFourWheeler = fourCount;
                        objVMLocationLotParkedVehicles.TotalThreeWheeler = threeCount;
                        objVMLocationLotParkedVehicles.TotalHVWheeler = heavyCount;
                        objVMLocationLotParkedVehicles.TotalOutTwoWheeler = 0;
                        objVMLocationLotParkedVehicles.TotalOutFourWheeler = 0;
                        objVMLocationLotParkedVehicles.TotalOutThreeWheeler = 0;
                        objVMLocationLotParkedVehicles.TotalOutHVWheeler = 0;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return objVMLocationLotParkedVehicles;
        }

        public VMLocationLotParkedVehicles GetAllDeSyncVehiclesOffline()
        {
            VMLocationLotParkedVehicles objVMLocationLotParkedVehicles = new VMLocationLotParkedVehicles();
            List<LocationLotParkedVehicles> lstCustomerParkingSlot = new List<LocationLotParkedVehicles>();
            try
            {

                // Get Records From SQLLite
                var lstchekIns = Task.Run(async () => await App.SQLiteDb.GetDeSyncCheckInAsync()
                                ).Result;
                if (lstchekIns.Count > 0)
                {
                    foreach (var items in lstchekIns)
                    {
                        LocationLotParkedVehicles objCustomerParkingSlot = new LocationLotParkedVehicles();
                        objCustomerParkingSlot.VehicleTypeCode = items.VehicleTypeCode;
                        if (items.VehicleTypeCode == "2W")
                        {
                            objCustomerParkingSlot.VehicleImage = "bike_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        if (items.VehicleTypeCode == "4W")
                        {
                            objCustomerParkingSlot.VehicleImage = "car_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        if (items.VehicleTypeCode == "3W")
                        {
                            objCustomerParkingSlot.VehicleImage = "ThreeW_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        if (items.VehicleTypeCode == "HW")
                        {
                            objCustomerParkingSlot.VehicleImage = "hv_black.png";
                            objCustomerParkingSlot.BayNumberColor = "#444444";
                            objCustomerParkingSlot.VehicleStatusColor = "#3293fa";
                        }
                        objCustomerParkingSlot.RegistrationNumber = items.RegistrationNumber;
                        objCustomerParkingSlot.ParkingBayName = items.BayRange;
                        objCustomerParkingSlot.ParkingBayRange = items.BayRange;
                        objCustomerParkingSlot.ApplicationTypeCode = "O";
                        lstCustomerParkingSlot.Add(objCustomerParkingSlot);
                    }
                    //Count Two Wheeler
                    if (lstCustomerParkingSlot.Count > 0)
                    {
                        var twoCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "2W");
                        var fourCount = lstCustomerParkingSlot.Count(p => p.VehicleTypeCode == "4W");

                        objVMLocationLotParkedVehicles.CustomerParkingSlotID = lstCustomerParkingSlot;
                        objVMLocationLotParkedVehicles.TotalTwoWheeler = twoCount;
                        objVMLocationLotParkedVehicles.TotalFourWheeler = fourCount;
                        objVMLocationLotParkedVehicles.TotalOutTwoWheeler = 0;
                        objVMLocationLotParkedVehicles.TotalOutFourWheeler = 0;
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return objVMLocationLotParkedVehicles;
        }

        #endregion
    }
}

