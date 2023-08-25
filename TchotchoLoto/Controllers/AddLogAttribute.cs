
using System.Web.Mvc;
using System.Reflection;
using System.Diagnostics;
using TchotchoLoto.Models;
using System;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace TchotchoLoto.Controllers
{
    public class AddLogAttribute : ActionMethodSelectorAttribute
    {

        private Entities db = new Entities();
        private const string URL = "http://api.ipstack.com/";

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo actionDescriptor)
        {

            var request = controllerContext.HttpContext.Request;



            //Debug.WriteLine("R: "+ request);

            string clientInfo = "";

            try
            {
                if (request != null)
                {

                    HttpClient client = new HttpClient
                    {
                        BaseAddress = new Uri(URL)
                    };

                    HttpResponseMessage responseMessage = null;


                    try
                    {

                        responseMessage = client.GetAsync("http://api.ipstack.com/" + request.UserHostAddress + "?access_key=8b5e2bcd484337d044589fcdf99f4dc3").Result;
                        //responseMessageGeolo = client.GetAsync("http://api.ipstack.com/" + request.UserHostAddress + "?access_key=47e329f6bc86866c85a3ac65c6ff5b9a").Result;

                        //responseMessage = client.GetAsync("http://api.ipstack.com/" + request.UserHostAddress + "?access_key=70c86c6b3c1b857ce86f720800c7c83a").Result;

                        // Forward Geocoding API Endpoint http : //api.positionstack.com/v1/ forward ? access_key = YOUR_ACCESS_KEY & query = 1600 Pennsylvania Ave NW , Washington DC


                        //Debug.WriteLine("Res: " + responseMessage);


                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine("Error Api");

                    }


                    if (request.Url != null)
                    {
                        clientInfo += "url:" + request.Url.AbsolutePath;
                    }

                    if (request.Browser != null)
                    {
                        clientInfo += ";platform:" + request.Browser.Platform;
                    }

                    if (responseMessage != null && responseMessage.IsSuccessStatusCode)
                    {

                        string rs = responseMessage.Content.ReadAsStringAsync().Result;

                        var obj = JObject.Parse(rs);

                        if (obj != null)
                        {
                            clientInfo += ";client_IP:" + obj["ip"];
                            clientInfo += ";type_IP:" + obj["type"];
                            clientInfo += ";client_hostname:" + obj["hostname"];
                            clientInfo += ";continent:" + obj["continent_name"];
                            clientInfo += ";country:" + obj["country_name"];
                            clientInfo += ";region:" + obj["region_name"];
                            clientInfo += ";city:" + obj["city"];
                            clientInfo += ";zip_code:" + obj["zip"];
                            clientInfo += ";latitude:" + obj["latitude"];
                            clientInfo += ";longitude:" + obj["longitude"];

                            if (obj["time_zone"] != null)
                            {
                                clientInfo += ";time_zone:" + obj["time_zone"].Children()["current_time"];
                            }

                            if (obj["connection"] != null)
                            {
                                clientInfo += ";connection:" + obj["connection"].Children()["isp"];
                            }

                            if (obj["security"] != null)
                            {
                                clientInfo += ";is_proxy:" + obj["security"].Children()["is_proxy"];
                                clientInfo += ";threat_level:" + obj["security"].Children()["threat_level"];
                                clientInfo += ";threat_types:" + obj["security"].Children()["threat_types"];
                            }

                        }

                    }


                }


                //Debug.WriteLine("cl: " + clientInfo);


                AddAppAccessLog addAppAccessLog = new AddAppAccessLog();
                addAppAccessLog.clientInfo = clientInfo;
                addAppAccessLog.Date = System.DateTime.Now;

                db.AddAppAccessLogs.Add(addAppAccessLog);

                db.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }


            return true;

        }
    }



}