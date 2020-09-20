using Newtonsoft.Json;
using ParkHyderabadOperator.DAL.EncryptDecrypt;
using ParkHyderabadOperator.Model.APIInputModel;
using ParkHyderabadOperator.Model.APIOutPutModel;
using ParkHyderabadOperator.Model.APIResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
namespace ParkHyderabadOperator.DAL.DALLogin
{
    public class DALUserLogin
    {

        DataEncryptDecrypt objencrypt;
        PasswordEncryptDecrypt objpwdencrypt;
        public APIResponse LoginVerification(string accessToken, UserLogin objUser)
        {
            User resultObj = null;
            string enckey = string.Empty;
            APIResponse apiResult = null;
            try
            {

                objUser.UserName = objUser.UserName;
                objUser.Password = objUser.Password;

                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPLoginVerification";
                    // make the request

                    var json = JsonConvert.SerializeObject(objUser);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                resultObj = JsonConvert.DeserializeObject<User>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return apiResult;
        }
        public string SaveUserDailyLogin(string accessToken, User objUser)
        {
            string resultmsg = string.Empty;
            try
            {
                // enckey= objpwdencrypt.GenerateEncryptionKey();
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPSaveUserDailyLogin";
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
                            resultmsg = apiResult.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resultmsg;
        }
        public string UpdateUserPassword(string accessToken, User objUser)
        {
            string resultmsg = string.Empty;
            try
            {
                // enckey= objpwdencrypt.GenerateEncryptionKey();
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/updateUserPassword";
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
                            resultmsg = apiResult.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resultmsg;
        }




    }
}
