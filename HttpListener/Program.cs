using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpListener
{
    class Program
    {
        
        private static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Bla();
            //ReadFile();
        }

        static async void Bla()
        {
            var web = new System.Net.HttpListener();

            var prefixes = ReadFile();
            foreach(var p in prefixes.Config)
            {
                web.Prefixes.Add("http://localhost:" + p.Endpoint.ToString() + "/");
            }
           //web.Prefixes.Add("http://localhost:8080/");

            Console.WriteLine("Listening..");

            web.Start();

            Console.WriteLine(web.GetContext());
            

            var context = web.GetContext();

            var response = context.Response;

            string responseString = await CreateRequest();

            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);

            Console.WriteLine(output);

            output.Close();

            web.Stop();

            Console.ReadKey();
        }

        private static async Task<string> CreateRequest()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            var url = "https://localhost:44356/api/values";
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
            return response;
        }

        private static Content ReadFile()
        {
            return JsonConvert.DeserializeObject<Content>(File.ReadAllText(@"C:\Users\MadCat\Documents\Visual Studio 2017\Projects\HttpListener\HttpListener\tsconfig1.json"));
        }
    }

    class Content
    {
        [JsonProperty(PropertyName = "config")]
        public List<Config> Config
        {
            get;
            set;
        }
    }
}
