package Server;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.nio.charset.StandardCharsets;
import java.util.Random;
import java.util.concurrent.TimeUnit;

public class GroupMulticastServer {
    final static String INET_ADDR = "228.1.2.3";
    final static int PORT = 6789;
    public static void main(String[] args) throws UnknownHostException
    {
        String [] techs = {"Java","C#-Blazor","WebSocket","SignalR","Spring Boot", "gRPC"};
// get the address to connect to
        InetAddress address = InetAddress.getByName(INET_ADDR);
        try (DatagramSocket serverSocket = new DatagramSocket()) {
            for (String t : techs) {
                int grp = getRandomGroup();
                String msg = "Group" + grp + " is using [" + t + "] in their project";
// create a packet
                byte[] msgBytes = msg.getBytes(StandardCharsets.UTF_8);
                DatagramPacket msgPkt =
                        new DatagramPacket(msgBytes, msgBytes.length, address, PORT);
// send the packet
                serverSocket.send(msgPkt);
                System.out.println("==>> Server has sent a packet with message: " + msg);
                TimeUnit.SECONDS.sleep(5);
            }
        } catch (IOException | InterruptedException ioe) {
            ioe.printStackTrace();
        }
    }
    private static int getRandomGroup() {
        Random rand = new Random();
        return rand.nextInt(4) + 1;
    }
}
