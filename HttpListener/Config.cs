using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HttpListener
{
    public class Config
    {
        public class Content
        {
            [JsonProperty(PropertyName = "config")]
            public List<Config> Config
            {
                get;
                set;
            }
        }

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