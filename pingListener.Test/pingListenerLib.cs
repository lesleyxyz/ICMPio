using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

//  netsh advfirewall firewall add rule name="pingListener IPv4" dir=in action=allow protocol=icmpv4:any,any
//  netsh advfirewall firewall add rule name="pingListener IPv6" dir=in action=allow protocol=icmpv6:any,any

namespace pingListener.Test
{
    class pingListener
    {
        public delegate void onPingReceiveHandler(IPAddress remoteEndPoint, int packetSize, byte[] packetBody);
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
            Thread threadLoop = new Thread(loop);
            threadLoop.Start();
        }
        private void loop()
        {
            ICMPListener.Bind(new IPEndPoint(ip, 0));
            ICMPListener.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });
            running = true;
            while(running) {
                byte[] buffer = new byte[65575]; //The max ping packet size including the header of an IPv6 address.
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                int bytesRead = ICMPListener.ReceiveFrom(buffer, ref remoteEP);

                byte[] fullPacket = buffer.Take(bytesRead).ToArray(); //buffer is 66575, it'll most likely be bigger than what we actually received 
                byte[] body = fullPacket.Skip(fullPacket.Length - 28).ToArray(); //Take all bytes but the 28 first (28 = 20 (ip address) + 8 (icmp header packet) 

                byte type = fullPacket[20];

                //byte code = fullPacket[21];

                if (type == 0x08) //request, reply = 0x00, we only want the requests to us.
                {
                    string remoteIP = StripPortFromEndPoint(remoteEP.ToString());

                    onPingReceive(IPAddress.Parse(remoteIP), bytesRead, body);
                }
            };
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

        public byte[] sendPacket(IPAddress end, byte[] body)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(end);
            if(reply.Status == IPStatus.Success)
            {
                return reply.Buffer;
            }else
            {
                Exception x = new PingException(reply.Status.ToString());
                return null;
            }
        }

    }
}
