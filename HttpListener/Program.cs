using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VirtualizationTool;
using static Virtualization.Config;

namespace Virtualization
{
    class Program
    {
        private HttpClient httpClient;
        private DBHandler dbHandler;
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
            program.Initialize();
            //ReadFile();
        }

        private void Initialize()
        {

            var fileContent = ReadFile(@"C:\Users\12353\Desktop\bachelor\SV\HttpListener\portsConfig.json");
            var configuration = JsonConvert.DeserializeObject<Content>(fileContent);
            foreach (var c in configuration.PortConfig)
            {
                switch (c.Type)
                {
                    case "UDP":
                        UDPManager uDPManager = new UDPManager(new DBHandler("udp_messages"));
                        uDPManager.Manage();
                       // return Types.UDP;
                        break;
                    case "AMQP":
                        AMQPManager aMQPManager = new AMQPManager(new DBHandler("amqp_messages"));
                        aMQPManager.Manage();
                        break;
                    default:
                        HTTPManager hTTPManager = new HTTPManager(new DBHandler("request_responses"));
                        hTTPManager.Manage();
                        break;
                }
            }
           /* if (configuration.Type == "UDP")
            {
                this.dbHandler = new DBHandler("udp_messages");
                return Types.UDP;
            }
            else
            {
                if (configuration.Type == "AMQP")
                {
                    this.dbHandler = new DBHandler("amqp_messages");
                    return Types.AMQP;
                }

                this.dbHandler = new DBHandler("request_responses");
                return Types.HTTP;
            }*/
        }

        /*private System.Net.HttpListener InitializeHttp()
        {
            var listener = new System.Net.HttpListener();
            var configuration = ReadFile("");
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

                //listener.Stop();
            }
            else
            {
                if (type == Types.UDP)
                {
                    UDPManager uDPManager = new UDPManager(this.dbHandler);
                    uDPManager.Manage();
                }
                else
                {
                    AMQPManager aMQPManager = new AMQPManager(this.dbHandler);
                    aMQPManager.Manage();
                }
            }

            Console.ReadKey();
        }*/

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

        public static string ReadFile(string path)
        {
            return File.ReadAllText(path);
            //return JsonConvert.DeserializeObject<Content>(File.ReadAllText(path));
        }

        private static string ConvertStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
