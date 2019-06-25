You can use ICMPio for layer 3 IPv4 communication (I'm working on IPv6) without having the requirement to open a port.

Note:
In order to use this, make sure you are able to accept all ICMP traffic:

    netsh advfirewall firewall add rule name="pingListener IPv4" dir=in action=allow protocol=icmpv4:any,any

(run in cmd as admin on all receiving endpoints)
(also it's possible that you have to configure your router to send all ping packets/all unknown packets to your IP. I had to set the DMZ to my IP but my friend didn't have to do anything)

Note: 
There's already a lot of traffic on the icmp protocol (for ex. from your pc to your router), you should make it so you add an identifier in your body or something.


Initiate using ICMPio:

    ICMPio icmpTest = new ICMPio(IPAddress.Parse(ip)); //Where IP is your local ip address
    icmpTest.Start();
    
Receive ICMP packets:

    icmpTest.onPingReceive += me.OnPing;
    
    void OnPing(IPAddress remoteEndPoint, int packetSize, byte[] packetBody){
        //do stuff with remoteEndPoint (remote ipaddress), packetSize and packetBody
    }
    
Send ICMP packet:

    icmpTest.sendPacket(IPAddress.Parse(ip), body); //where ip is the remote IP (to send the packet to) and body is the packet body in bytes
