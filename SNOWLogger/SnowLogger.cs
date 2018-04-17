﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Bot.Sample.ProactiveBot
{
    public class SnowLogger
    {
        public static string CreateIncidentServiceNow(string shortDescription, string contact, string Description, string category_name)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowURL"];

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                request.Headers.Add("Authorization", auth);
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string Json = JsonConvert.SerializeObject(new
                    {
                        description = Description,
                        short_description = shortDescription,
                        contact_type = contact,
                        category = category_name,
                        subcategory = ConfigurationManager.AppSettings["ServiceNowSubCategory"],
                        assignment_group = ConfigurationManager.AppSettings["ServiceNowAssignmentGroup"],
                        impact = ConfigurationManager.AppSettings["ServiceNowIncidentImpact"],
                        priority = ConfigurationManager.AppSettings["ServiceNowIncidentPriority"],
                        caller_id = ConfigurationManager.AppSettings["ServiceNowCallerId"],
                        cmdb_id = ConfigurationManager.AppSettings["ServiceNowCatalogueName"],
                        comments = ConfigurationManager.AppSettings["ServiceNowComments"]
                    });

                    streamWriter.Write(Json);

                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    JObject joResponse = JObject.Parse(res.ToString());
                    JObject ojObject = (JObject)joResponse["result"];
                    string incidentNumber = ((JValue)ojObject.SelectToken("number")).Value.ToString();
                    return incidentNumber;
                }
            }
            catch(Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
            
        }
    }
}