using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

public class Receiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    private bool isRunning = false;

    public int port = 6000;

    void Start()
    {
        try
        {
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            udpClient.EnableBroadcast = true;

            isRunning = true;

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("UDP Receiver started on port " + port);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to start UDP receiver: " + e.Message);
        }
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                messageQueue.Enqueue(message);
            }
            catch (SocketException e)
            {
                if (isRunning)
                    messageQueue.Enqueue("SocketError: " + e.Message);
            }
            catch (Exception e)
            {
                if (isRunning)
                    messageQueue.Enqueue("Error: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        udpClient?.Close();
        receiveThread?.Join(500);
    }
}
