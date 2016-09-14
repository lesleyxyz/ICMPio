using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//  netsh advfirewall firewall add rule name="pingListener IPv4" dir=in action=allow protocol=icmpv4:any,any
//  netsh advfirewall firewall add rule name="pingListener IPv6" dir=in action=allow protocol=icmpv6:any,any

namespace pingListener.Test
{
    class pingListener
    {
        public delegate void onPingReceiveHandler(IPAddress remoteEndPoint, int packetSize);
        public event onPingReceiveHandler onPingReceive;
        private Socket ICMPListener = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
        private IPAddress ip;
        public bool running = false;
       
        public pingListener(IPAddress localIp)
        {
            ip = localIp;
        }

        public void Start()
        {
            ICMPListener.Bind(new IPEndPoint(ip, 0));
            ICMPListener.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });
            running = true;
            retryp:
            byte[] buffer = new byte[65575]; //The max ping packet size including the header of an IPv6 address.
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int bytesRead = ICMPListener.ReceiveFrom(buffer, ref remoteEP);

            string remoteIP = StripPortFromEndPoint(remoteEP.ToString());

            onPingReceive(IPAddress.Parse(remoteIP), bytesRead);

            goto retryp;
        }
        public void Stop()
        {
            running = false;
        }
        private string StripPortFromEndPoint(string endPoint)
        {
            var splitList = endPoint.Split(':');
            if (splitList.Length > 2)
            {
                endPoint = IPAddress.Parse(endPoint).ToString();
            }
            else if (splitList.Length == 2)
            {
                endPoint = splitList[0];
            }
            else
            {
            }

            return endPoint;
        }

    }
}
