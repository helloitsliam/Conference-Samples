using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;
using System.Net;

namespace COGNITIVE
{
    static class Helper
    {
        public const string sentimentUrl = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        public const string keyphraseUrl = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases";
        public const string languageUrl = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/languages";
        //public const string translateUrl = "https://api.cognitive.microsoft.com/sts/v1.0";
        public const string translateUrl = "https://api.microsofttranslator.com/v2/Http.svc/translate";

        public const string subscriptionKey = "{Your Azure Key}";
        public const string translationSubscriptionKey = "{Your Translation key}";

        public const string siteUrl = "{SharePoint Url}";
        public const string documentLibrary = "Documents";
        public const string filePath = "D:\\COGNITIVE\\FILES";

        public static string sentimentOutput = string.Empty;
        public static string keyphraseOutput = string.Empty;
        public static string languageOutput = string.Empty;
        public static string translateOutput = string.Empty;

        public static async Task<string> GetSentiment(string body)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Helper.subscriptionKey);
            var uri =  sentimentUrl + "?" + queryString;

            HttpResponseMessage response;
            String result = "";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "ERROR: " + result;
            }

            sentimentOutput = result;
            return sentimentOutput;
        }

        public static async Task<string> GetKeyPhrases(string body)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Helper.subscriptionKey);
            var uri = keyphraseUrl + "?" + queryString;

            HttpResponseMessage response;
            String result = "";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "ERROR: " + result;
            }

            keyphraseOutput = result;
            return keyphraseOutput;
        }

        public static async Task<string> GetLanguage(string body)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Helper.subscriptionKey);
            var uri = languageUrl + "?" + queryString;

            HttpResponseMessage response;
            String result = "";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "ERROR: " + result;
            }

            languageOutput = result;
            return languageOutput;
        }

        public static async Task<string> DoTranslate(string body)
        {
            string lang = "de";

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Helper.translationSubscriptionKey);
            var uri = translateUrl + "?to=" + lang + "&" + queryString;

            HttpResponseMessage response;
            String result = "";

            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "ERROR: " + result;
            }
            
            translateOutput = result;
            return translateOutput;
        }

        public static string DoTranslate(string body, string language, string translationlanguage)
        {
            string uri = string.Format(translateUrl + "?text=" + HttpUtility.UrlEncode(body.ToString()) + "&from={0}&to={1}", language, translationlanguage);
            var translationWebRequest = HttpWebRequest.Create(uri);
            translationWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", Helper.translationSubscriptionKey);
            WebResponse response = null;
            response =  translationWebRequest.GetResponse();

            // Parse the response XML
            Stream stream = response.GetResponseStream();
            StreamReader translatedStream = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
            xmlResponse.LoadXml(translatedStream.ReadToEnd());

            translateOutput = xmlResponse.InnerText; ;
            return translateOutput;
        }

        public static byte[] GetBinary(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
