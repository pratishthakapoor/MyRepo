﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.Bot.Sample.ProactiveBot
{

    /**
     * Snow Logger class which is used to connect to the SNOW service using Table API.
     **/

    public class SnowLogger
    {
       
        public static string GetTablename()
        {
            List<string> labelDetails = new List<string>();
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = "https://dev56432.service-now.com/api/now/table/sys_db_object?sysparm_query=sys_id=4c3fb9e6b96013006517ce7df7ee4671";
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";
                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    
                    JEnumerable<JToken> labelName = (JEnumerable<JToken>)obObject.Values("name");
                    //return labelName.ToString();
                    foreach (var item in labelName)
                    {
                        if (item != null)
                        {
                            return(((JValue)item).Value.ToString());
                            //return GetTableDetails(labelDetails);
                        }
                    }
                }

                return string.Empty;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public static string CreateIncidentServiceNow(string shortDescription, string contactType, string Description, string category_name)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                //string URL = ConfigurationManager.AppSettings["ServiceNowURL"];
                string URL = ConfigurationManager.AppSettings["ServiceNowUrl"] + GetTablename();

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
                        contact_type = contactType,
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

                /**
                 * HttpWebResponse captures the details send by the REST Table API
                 **/
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

        public static string RetrieveIncidentServiceNow(string Ticketresponse)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowURL"] + GetTablename() + "?" + "sysparm_query=number=" + Ticketresponse;

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";

                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    JEnumerable<JToken> incidentStatus = (JEnumerable<JToken>)obObject.Values("state");
                    foreach(var item in incidentStatus)
                    {
                        if (item != null)
                            return ((JValue)item).Value.ToString();
                    }
                }                
            }
            catch(Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
            return string.Empty;
        }

        internal static string RetrieveIncidentCloseDetails(string Newresponse)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowURL"] + GetTablename() + "?" + "sysparm_query=number=" + Newresponse;

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";

                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    JEnumerable<JToken> ClosedDeatils = (JEnumerable<JToken>)obObject.Values("close_code");
                    foreach (var ClosedItem in ClosedDeatils)
                    {
                        if (ClosedItem != null)
                            return ((JValue)ClosedItem).Value.ToString();
                    }
                }
            }
            catch (Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
            return string.Empty;
        }

        internal static string RetrieveIncidentResolveDetails(string Detailresponse)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowURL"] + GetTablename() + "?" + "sysparm_query=number=" + Detailresponse;

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";

                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    JEnumerable<JToken> ResolvedDeatils = (JEnumerable<JToken>)obObject.Values("close_notes");
                    foreach (var DetailItem in ResolvedDeatils)
                    {
                        if (DetailItem != null)
                            return ((JValue)DetailItem).Value.ToString();
                    }
                }
            }
            catch (Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
            return string.Empty;
        }

        internal static string RetrieveIncidentWorkNotes(string Notesresponse)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowURL"] + GetTablename() + "?" + "sysparm_query=number=" + Notesresponse;

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";

                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    JEnumerable<JToken> NotesDetails = (JEnumerable<JToken>)obObject.Values("sys_id");

                    foreach (var DetailItem in NotesDetails)
                    {
                        if (DetailItem != null)
                            return GetNotesDetailList(((JValue)DetailItem).Value.ToString());
                    }
                    
                }
            }
            catch (Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
            return string.Empty;
        }

        private static string GetNotesDetailList(object notesDetails)
        {
            try
            {
                string username = ConfigurationManager.AppSettings["ServiceNowUserName"];
                string password = ConfigurationManager.AppSettings["ServiceNowPassword"];
                string URL = ConfigurationManager.AppSettings["ServiceNowJournalURL"] + "?" + "sysparm_query=element_id=" + notesDetails;

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";
                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    JEnumerable<JToken> ResolvedDeatils = (JEnumerable<JToken>)obObject.Values("value");
                    foreach (var Item in ResolvedDeatils)
                    {
                        if (Item != null)
                            return ((JValue)Item).Value.ToString();
                    }
                }

            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }
    }
}