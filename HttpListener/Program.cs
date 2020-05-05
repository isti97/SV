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
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Initialize();
        }

        private void Initialize()
        {

            var fileContent = ReadFile(@"C:\Users\12353\Desktop\bachelor\SV\HttpListener\protocolConfig.json");
            var configuration = JsonConvert.DeserializeObject<Content>(fileContent);
            foreach (var c in configuration.PortConfig)
            {
                switch (c.Type)
                {
                    case "UDP":
                        UDPManager uDPManager = new UDPManager(new DBHandler("udp_messages"));
                        uDPManager.Manage();
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
        }

        public static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
