using Newtonsoft.Json;
using ParkHyderabadOperator.Model;
using ParkHyderabadOperator.Model.APIResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace ParkHyderabadOperator.DAL.DALExceptionLog
{
    public class DALExceptionManagment
    {

        public DALExceptionManagment()
        { }
        public void InsertException(string accessToken, string ApplicationType, string ExceptionMessage, string Module, string Procedure, string Method)
        {
            try
            {


                ExceptionLog objexlog = new ExceptionLog();
                objexlog.ApplicationType = ApplicationType;
                objexlog.ExceptionMessage = ExceptionMessage;
                objexlog.Module = Module;
                objexlog.Procedure = Procedure;
                objexlog.Method = Method;

                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaOperator/postOPAPPExceptionLog";
                    // make the request

                    var json = JsonConvert.SerializeObject(objexlog);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }
    }
}


