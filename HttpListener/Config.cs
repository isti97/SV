using Newtonsoft.Json;
using System;

namespace HttpListener
{
    public class Config
    {
        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "port")]
        public int Port
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "targetUrl")]
        public string TargetUrl
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "targetResource")]
        public string TargetResource
        {
            get;
            set;
        }
    }
}