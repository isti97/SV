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

        private static HttpClient client = new HttpClient();
        private static MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\tsconfig1.json";

        static void Main(string[] args)
        {
            //CreateNewDocumentInDB();
            Bla();
            //ReadFile();
        }

        static async void Bla()
        {
            var web = new System.Net.HttpListener();

            var prefixes = ReadFile();
            foreach (var p in prefixes.Config)
            {
                web.Prefixes.Add(p.Endpoint + p.Port.ToString() + "/");
            }
            // web.Prefixes.Add("http://localhost:8080/");

            Console.WriteLine("Listening..");

            web.Start();

            //Console.WriteLine(web.GetContext());

            var context = web.GetContext();

            //var path = context.Request.Url.PathAndQuery;

            
            var response = context.Response;

            string responseString = await CreateRequest(context);

            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);

            
            // Console.WriteLine(output);

            output.Close();

            web.Stop();

            Console.ReadKey();
        }

        private static async Task<string> CreateRequest(HttpListenerContext context)
        {
            var path = context.Request.Url.PathAndQuery;
            var headers = context.Request.Headers;
            var body = ConvertStreamToString(context.Request.InputStream); //body
            var url = "http://localhost:49363" + path;

            if (body.Length > 0)
            {
                // kuld tovabb a bodyt a requestel
            }
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };


            string response = null;
            using (var client = new HttpClient(clientHandler))
            {
                var responseTask = client.GetAsync(url);
                responseTask.Wait();

                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    response = result.Content.ReadAsStringAsync().Result;
                }
            }
            CreateNewDocumentInDB(url, response);
            return response;
        }

        private static Content ReadFile()
        {
            return JsonConvert.DeserializeObject<Content>(File.ReadAllText(path));
        }

        private static void CreateNewDocumentInDB(string request, string response)
        {

            IMongoDatabase db = dbClient.GetDatabase("sv");

            var req_res = db.GetCollection<BsonDocument>("request_responses");

            var doc = new BsonDocument
            {
                {"request", request},
                {"response", response}
            };

            req_res.InsertOne(doc);
        }

        private static string ConvertStreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
