using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Virtualization.Config;

namespace Virtualization
{
    class Program
    {
        private HttpClient httpClient;
        private DBHandler dbHandler;
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\tsconfig1.json";
        private string targetUrl;

        public Program()
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            this.httpClient = new HttpClient(clientHandler);
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Manage();
            //ReadFile();
        }

        private Types Initialize()
        {

            var configuration = ReadFile();
            if (configuration.Type == "UDP")
            {
                this.dbHandler = new DBHandler("udp_messages");
                return Types.UDP;
            }
            else
            {
                this.dbHandler = new DBHandler("request_responses");
                return Types.HTTP;
            }
        }

        private System.Net.HttpListener InitializeHttp()
        {
            var listener = new System.Net.HttpListener();
            var configuration = ReadFile();
            listener.Prefixes.Add(configuration.Endpoint + configuration.Port + "/");
            this.targetUrl = configuration.TargetUrl;
            return listener;
        }

        private async void Manage()
        {
            var type = Initialize();
            if (type == Types.HTTP)
            {
                var listener = InitializeHttp();

                Console.WriteLine("Listening..");

                listener.Start();

                var context = listener.GetContext();

                var response = context.Response;

                string responseString = await this.CreateRequest(context);

                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;

                var output = response.OutputStream;

                output.Write(buffer, 0, buffer.Length);


                // Console.WriteLine(output);

                output.Close();

                listener.Stop();
            }
            else
            {
                UDPManager uDPManager = new UDPManager(this.dbHandler);
                uDPManager.Manage();
            }

            Console.ReadKey();
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
                return document.GetElement("response").ToString();
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

        public static Config ReadFile()
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
        }

        private static string ConvertStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
