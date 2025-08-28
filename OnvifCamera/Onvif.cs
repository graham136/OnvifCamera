using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Pal.Helper.Camera
{
    public class Onvif
    {
        private class UdpState
        {
            public UdpClient Client { get; set; }

            public IPEndPoint Endpoint { get; set; }

            public IList<string> Result { get; set; }

            public Action<string> Callback { get; set; }
        }

        public const int ONVIF_BROADCAST_TIMEOUT = 4000;

        public const int ONVIF_DISCOVERY_PORT = 3702;

        public static string OnvifDiscoveryAddress = "239.255.255.250";

        private static readonly SemaphoreSlim _discoverySlim = new SemaphoreSlim(1);

        public async Task<IList<string>> DiscoverAsync(Action<string> onDeviceDiscovered = null, int broadcastTimeout = 4000, int broadcastPort = 0, string deviceType = "NetworkVideoTransmitter")
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            List<Task<IList<string>>> discoveryTasks = new List<Task<IList<string>>>();
            NetworkInterface[] array = nics;
            foreach (NetworkInterface adapter in array)
            {
                if ((!adapter.Supports(NetworkInterfaceComponent.IPv4) && !adapter.Supports(NetworkInterfaceComponent.IPv6)))
                {
                    continue;
                }                

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    foreach (UnicastIPAddressInformation ua in adapterProperties.UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            Task<IList<string>> discoveryTask = DiscoverAsync(ua.Address.ToString(), onDeviceDiscovered, broadcastTimeout, broadcastPort, deviceType);
                            discoveryTasks.Add(discoveryTask);
                        }
                    }
                
            }

            await Task.WhenAll(discoveryTasks);
            return discoveryTasks.Where((Task<IList<string>> x) => x.IsCompleted && !x.IsFaulted && !x.IsCanceled).SelectMany((Task<IList<string>> x) => x.Result).Distinct()
                .ToList();
        }

        public async Task<IList<string>> DiscoverAsync(string ipAddress, Action<string> onDeviceDiscovered = null, int broadcastTimeout = 4000, int broadcastPort = 0, string deviceType = "NetworkVideoTransmitter")
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException("ipAddress");
            }

            if (ipAddress.StartsWith("192.168"))
            {

                await _discoverySlim.WaitAsync();
                string uuid = Guid.NewGuid().ToString().ToLowerInvariant();
                string onvifDiscoveryProbe = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\">\r\n   <s:Header>\r\n      <a:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</a:Action>\r\n      <a:MessageID>urn:uuid:" + uuid + "</a:MessageID>\r\n      <a:ReplyTo>\r\n        <a:Address>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>\r\n      </a:ReplyTo>\r\n      <a:To>urn:schemas-xmlsoap-org:ws:2005:04:discovery</a:To>\r\n   </s:Header>\r\n   <s:Body>\r\n      <Probe xmlns=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\">\r\n         <d:Types xmlns:d=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\" xmlns:dp0=\"http://www.onvif.org/ver10/network/wsdl\">dp0:" + deviceType + "</d:Types>\r\n      </Probe>\r\n   </s:Body>\r\n</s:Envelope>\r\n";
                IList<string> devices = new List<string>();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), broadcastPort);
                IPEndPoint multicastEndpoint = new IPEndPoint(IPAddress.Parse(OnvifDiscoveryAddress), 3702);
                try
                {
                    using UdpClient client = new UdpClient(endPoint);
                    UdpState s = new UdpState
                    {
                        Endpoint = endPoint,
                        Client = client,
                        Result = devices,
                        Callback = onDeviceDiscovered
                    };
                    client.BeginReceive(DiscoveryMessageReceived, s);
                    byte[] message = Encoding.UTF8.GetBytes(onvifDiscoveryProbe);
                    await client.SendAsync(message, message.Count(), multicastEndpoint);
                    await Task.Delay(broadcastTimeout);
                    return s.Result.OrderBy((string x) => x).ToArray();
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                    return new List<string>().ToArray();
                }
                finally
                {
                    _discoverySlim.Release();
                }
            }
            else
            {
                return new List<string>().ToArray();
            }
        }

        private void DiscoveryMessageReceived(IAsyncResult result)
        {
            try
            {
                UdpClient client = ((UdpState)result.AsyncState).Client;
                IPEndPoint remoteEP = ((UdpState)result.AsyncState).Endpoint;
                byte[] bytes = client.EndReceive(result, ref remoteEP);
                string @string = Encoding.UTF8.GetString(bytes);
                string text = remoteEP.Address.ToString();
                IList<string> result2 = ((UdpState)result.AsyncState).Result;
                string text2 = ReadOnvifEndpoint(@string);
                if (text2 != null)
                {
                    result2.Add(text2);
                    Action<string> callback = ((UdpState)result.AsyncState).Callback;
                    if (callback != null)
                    {
                        try
                        {
                            callback(text2);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }

                client.BeginReceive(DiscoveryMessageReceived, result.AsyncState);
            }
            catch (Exception ex2)
            {
                Debug.WriteLine(ex2.Message);
            }
        }

        private string ReadOnvifEndpoint(string message)
        {
            using StringReader textReader = new StringReader(message);
            XPathDocument xPathDocument = new XPathDocument(textReader);
            XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
            XPathNavigator xPathNavigator2 = xPathNavigator.SelectSingleNode("//*[local-name()='XAddrs']/text()");
            if (xPathNavigator2 != null)
            {
                string[] source = xPathNavigator2.Value.Split(' ');
                return source.First();
            }

            return null;
        }
    }
}

   
