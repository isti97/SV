using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpListener
{
    public class UDPManager
    {
        private int targetPort;
        private int listenPort;
        private int callbackPort;
        private int sourceListenerPort;

        public void Manage()
        {
            Initialize();

        }

        private void Initialize()
        {
            /*var configuration = Program.ReadFile();
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
                    var data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    ForwardMessage(data, targetPort);
                    ListenForCallback();
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

        private void ListenForCallback()
        {
            UdpClient listener = new UdpClient(callbackPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, callbackPort);

            try
            {
                while (true)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    var data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    ForwardMessage(data, sourceListenerPort);
                }
            }
            catch (SocketException e)
            {
            }
            finally
            {
                listener.Close();
            }*/
        }
    }
}
