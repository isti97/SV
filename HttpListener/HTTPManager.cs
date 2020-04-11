using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Virtualization;

namespace VirtualizationTool
{
    public class HTTPManager
    {
        private HttpClient httpClient;
        private DBHandler dbHandler;
        private string targetUrl;
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\udpConfig.json";
        Thread thread;

        public HTTPManager(DBHandler dBHandler)
        {
            this.dbHandler = dBHandler;
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            this.httpClient = new HttpClient(clientHandler);
        }

        private System.Net.HttpListener InitializeHttp()
        {
            var listener = new HttpListener();
            var content = Program.ReadFile(path);
            var configuration = JsonConvert.DeserializeObject<Config>(content);
            listener.Prefixes.Add(configuration.Endpoint + configuration.Port + "/");
            this.targetUrl = configuration.TargetUrl;
            return listener;
        }

        public void Manage()
        {
            thread = new Thread(Communicate);
            thread.Start();
        }

        public async void Communicate()
        {
            var listener = InitializeHttp();

            Console.WriteLine("Listening..");

            listener.Start();
            while (true)
            {
                var context = listener.GetContext();

                var response = context.Response;

                string responseString = await this.CreateRequest(context);

                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;

                var output = response.OutputStream;

                output.Write(buffer, 0, buffer.Length);

                output.Close();

                Thread.Sleep(100);
            }
        }

        private async Task<string> CreateRequest(HttpListenerContext context)
        {
            var httpPath = context.Request.Url.PathAndQuery;
            var headers = context.Request.Headers;
            var body = ConvertStreamToString(context.Request.InputStream); //body
            var url = this.targetUrl + httpPath;

            var document = dbHandler.CheckEntryInDB(url);

            if (document != null)
            {
                return document.GetElement("response").Value.ToString();
                //return document.GetElement("response").Value.ToString();
            }

            if (body.Length > 0)
            {
                // kuld tovabb a bodyt a requestel
            }

            string response = null;
            using (httpClient)
            {
                var responseTask = httpClient.GetAsync(url);
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    response = result.Content.ReadAsStringAsync().Result;
                }
            }
            dbHandler.CreateNewDocumentInDB(url, response);
            return response;
        }

        private static string ConvertStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
