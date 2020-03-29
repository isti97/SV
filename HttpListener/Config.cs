using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Virtualization
{
    /*public class Content
    {
        [JsonProperty(PropertyName = "config")]
        public List<Config> Config
        {
            get;
            set;
        }
    }*/

    public class Config
    {
        /*public Config(string endpoint, int port, string targetUrl, string targetResource)
        {
            Endpoint = endpoint;
            Port = port;
            TargetUrl = targetUrl;
            TargetResource = targetResource;
        }*/

       // [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint
        {
            get;
            set;
        }

       // [JsonProperty(PropertyName = "port")]
        public int Port
        {
            get;
            set;
        }

       // [JsonProperty(PropertyName = "targetUrl")]
        public string TargetUrl
        {
            get;
            set;
        }

        // only for udp needed
        public string SourceUrl
        {
            get;
            set;
        }

      //  [JsonProperty(PropertyName = "targetResource")]
        public string TargetResource
        {
            get;
            set;
        }

        //[JsonProperty(PropertyName = "type")]
        public string Type
        {
            get;
            set;
        }

        // only for udp needed
        public int TargetPort
        {
            get;
            set;
        }

        // only for udp needed
        public int CallbackPort
        {
            get;
            set;
        }

        // only for udp needed
        public int SourceListenerPort
        {
            get;
            set;
        }
    }
}