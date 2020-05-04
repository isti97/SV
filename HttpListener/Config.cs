using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Virtualization
{
    public class Content
    {
        [JsonProperty(PropertyName = "config")]
        public List<PortConfig> PortConfig
        {
            get;
            set;
        }
    }

    public class PortConfig
    {
        public string Type
        {
            get;
            set;
        }
    }

    public class Config
    {

        public string Endpoint
        {
            get;
            set;
        }

        public int? Port
        {
            get;
            set;
        }

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

        public string TargetResource
        {
            get;
            set;
        }

        // only for udp needed
        public int? TargetPort
        {
            get;
            set;
        }

        // only for udp needed
        public int? CallbackPort
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

        //only for AMQP

        public string SourceQueue
        {
            get;
            set;
        }

        public string DestinationQueue
        {
            get;
            set;
        }
    }
}