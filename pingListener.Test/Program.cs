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
        static string ip = "192.168.0.2";
        pingListener pingList = new pingListener(IPAddress.Parse(ip));
        static void Main(string[] args)
        {
            Program me = new Program();
            me.pingList.onPingReceive += me.OnPing;
            me.pingList.Start();
            while (true)
            { //send a packet by typing in console: ip|message (for ex: 8.8.8.8|Hey)
                string input = Console.ReadLine();
                string[] parts = input.Split('|');
                string ip = parts[0];
                string body = parts[1];
                me.pingList.sendPacket(IPAddress.Parse(ip), Encoding.UTF8.GetBytes(body));
            }
        }
        void OnPing(IPAddress remoteEndPoint, int packetSize, byte[] packetBody)
        {
            if (remoteEndPoint.ToString() == ip) { return;  }
            Console.WriteLine("Request from {0}: bytes={1}: {2}", remoteEndPoint.ToString(), packetSize, Environment.NewLine + Encoding.UTF8.GetString(packetBody));
            Console.Out.FlushAsync();
        }
    }
}
