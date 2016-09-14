Note:
In order to use this, make sure you are able to accept all ICMP traffic:

netsh advfirewall firewall add rule name="pingListener IPv4" dir=in action=allow protocol=icmpv4:any,any