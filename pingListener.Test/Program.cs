using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pingListener.Test
{
    class Program
    {
        pingListener pingList = new pingListener(IPAddress.Parse("192.168.0.135"));
        static void Main(string[] args)
        {
            Program me = new Program();
            me.pingList.onPingReceive += me.OnPing;
            me.pingList.Start();
        }
        void OnPing(IPAddress remoteEndPoint, int packetSize)
        {
            if (remoteEndPoint.ToString() == "192.168.0.135") { return;  }
            Console.WriteLine("Request from {0}: bytes={1}", remoteEndPoint.ToString(), packetSize);
        }
    }
}
