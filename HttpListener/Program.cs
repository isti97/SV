using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static HttpListener.Config;

namespace HttpListener
{
    class Program
    {
        private HttpClient httpClient;
        private DBHandler dbHandler;
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\tsconfig1.json";
        private string targetUrl;

        public Program()
        {
            this.dbHandler = new DBHandler();
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

        private System.Net.HttpListener Initialize()
        {

            var listener = new System.Net.HttpListener();

            var configuration = ReadFile();
            listener.Prefixes.Add(configuration.Endpoint + configuration.Port + "/");
            this.targetUrl = configuration.TargetUrl;
            return listener;
        }

        private async void Manage()
        {
            var listener = Initialize();

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

        private static Config ReadFile()
        {
            var c = @"{
  'config':
    {
                'endpoint': 'http://localhost:',
      'port': 8080,
      'targetUrl': 'http://localhost:62863',
      'targetResource': '/api/values/'
    }
        }";
            Config t = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
            return t;
        }

        private static string ConvertStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
