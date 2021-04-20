using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace secadv
{
    class Program
    {
        static void Main(string[] args)
        {
            //credentials nooit plaatsen in een variabele
            string grant_type = "client_credentials";
            string scope = "api1";
            string client_id = "pxl-secadv";
            string client_secret = "maarten_lust_geen_spruitjes";
            string tokenEndPoint = "https://ventielshop.dubbadub.be:8081/connect/token";
            string ventielShopWebsite = "https://ventielshop.dubbadub.be/fiets";

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpWebRequest request;
            HttpWebResponse response;
            string postData = $"grant_type={grant_type}&scope={scope}&client_id={client_id}&client_secret={client_secret}";


            request = (HttpWebRequest)WebRequest.Create(tokenEndPoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] data = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("-----------------------");
            Console.WriteLine("Respone status code: " + response.StatusCode);
            string token;
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();
                token = responseBody.TrimStart('{').TrimEnd('}').Split(",")[0].Split(":")[1].Trim('"');
                reader.Close();
            }
            Console.WriteLine("-----------------------");
            Console.WriteLine();

            request = (HttpWebRequest)WebRequest.Create(ventielShopWebsite);
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Method = "GET";

            response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("-----------------------");
            Console.WriteLine("Response status code: " + response.StatusCode);
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();
                Console.WriteLine("Response: " + responseBody);
                reader.Close();
            }
            Console.WriteLine("-----------------------");
        }
    }

    public class FolderSettings
    {
        public Dictionary<string, string> TargetFolderLocations { get; set; }
    }
}
