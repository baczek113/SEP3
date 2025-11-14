package Client;

import java.io.IOException;
import java.net.*;

public class GrpMulticastClientTeam2 {
    final static String INET_ADDR = "228.1.2.3";
    final static int PORT = 6789;
    public static void main(String[] args) throws UnknownHostException {
        InetAddress address = InetAddress.getByName(INET_ADDR);
// create buffer of bytres
        byte[] buf = new byte[1024];
// create a new MulticastSocket
        try(MulticastSocket socket = new MulticastSocket(PORT)) {
            InetSocketAddress group = new InetSocketAddress(address, PORT);
            NetworkInterface netIf = NetworkInterface.getByName("team40");
// join the group
            socket.joinGroup(group, netIf);
// Receive the packet
            while(true) {
                DatagramPacket msgPkt = new DatagramPacket(buf, buf.length);
                socket.receive(msgPkt);
                String msg = new String(msgPkt.getData(), 0, msgPkt.getLength());
                System.out.println("Team 4 received: " + msg);
            }
        } catch (IOException ioe) {
            ioe.printStackTrace();
        }
    }
}