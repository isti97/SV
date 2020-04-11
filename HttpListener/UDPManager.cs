using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Virtualization
{
    public class UDPManager
    {
        private int targetPort;
        private int listenPort;
        private int callbackPort;
        private int sourceListenerPort;
        private DBHandler dBHandler;
        private const string path = @"C:\Users\12353\Desktop\bachelor\SV\HttpListener\udpConfig.json";
        Thread thread;

        public UDPManager(DBHandler dBHandler)
        {
            this.dBHandler = dBHandler;
        }

        public void Manage()
        {
            Initialize();
            thread = new Thread(ListenCommunication);
            thread.Start();
        }

        private void Initialize()
        {
            var content = Program.ReadFile(path);
            var configuration = JsonConvert.DeserializeObject<Config>(content);
            this.targetPort = configuration.TargetPort;
            this.listenPort = configuration.Port;
            this.callbackPort = configuration.CallbackPort;
            this.sourceListenerPort = configuration.SourceListenerPort;
        }

        private void ListenCommunication()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    var req = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    var document = dBHandler.CheckEntryInDB(req);
                    if (document != null)
                    {
                        var resp = document.GetElement("response").Value.ToString();
                        ForwardMessage(resp, sourceListenerPort);
                        continue;
                    }

                    ForwardMessage(req, targetPort);
                    var res = ListenForCallback();
                    dBHandler.CreateNewDocumentInDB(req, res);
                    Thread.Sleep(100);
                }
            }
            catch (SocketException e)
            {
            }
            finally
            {
                listener.Close();
            }
        }

        private void ForwardMessage(string data, int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress destination = IPAddress.Parse("127.0.0.1");
            byte[] sendbuf = Encoding.ASCII.GetBytes(data);
            IPEndPoint endPoint = new IPEndPoint(destination, port);
            s.SendTo(sendbuf, endPoint);
        }

        private string ListenForCallback()
        {
            UdpClient listener = new UdpClient(callbackPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, callbackPort);

            try
            {
                byte[] bytes = listener.Receive(ref groupEP);
                var data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                ForwardMessage(data, sourceListenerPort);

                return data;
            }
            catch (SocketException e)
            {
                return null;
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
