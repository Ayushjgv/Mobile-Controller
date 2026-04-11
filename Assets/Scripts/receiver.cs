// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;

// public class receiver : MonoBehaviour
// {

//     UdpClient udpClient;
//     Thread receiveThread;

//     public int port = 6000;
//     bool running = true;

// void Start()
// {
//     receiveThread = new Thread(ReceiveData);
//     receiveThread.IsBackground = true;
//     receiveThread.Start();
// }

// void ReceiveData()
// {
//     udpClient = new UdpClient(port);
//     IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

//     Debug.Log("UDP Receiver started on port " + port);

//     while (running)
//     {
//         try
//         {
//             byte[] data = udpClient.Receive(ref remoteEndPoint);
//             string message = Encoding.UTF8.GetString(data);

//             Debug.Log("Received: " + message);
//         }
//         catch (Exception e)
//         {
//             Debug.Log("Error: " + e.Message);
//         }
//     }
// }

// void OnApplicationQuit()
// {
//     running = false;
//     udpClient?.Close();
// }

//     void Update()
//     {
        
//     }
// }




// using UnityEngine;
// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Collections.Concurrent;
// using System.Threading;

// public class Receiver : MonoBehaviour
// {
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
//     private bool isRunning = false;

//     public int port = 6000;

//     void Start()
//     {
//         try
//         {
//             // UdpClient handles broadcast reception far more reliably than raw Socket in Unity
//             udpClient = new UdpClient();
//             udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
//             udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
//             udpClient.EnableBroadcast = true;  // Critical for receiving broadcast packets

//             isRunning = true;

//             // Run on a background thread — never block Unity's main thread with recvfrom
//             receiveThread = new Thread(ReceiveLoop);
//             receiveThread.IsBackground = true;
//             receiveThread.Start();

//             Debug.Log("UDP Receiver started on port " + port);
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Failed to start UDP receiver: " + e.Message);
//         }
//     }

//     void ReceiveLoop()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

//         while (isRunning)
//         {
//             try
//             {
//                 // This blocks until a packet arrives — perfectly fine on a background thread
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.UTF8.GetString(data);

//                 // Don't call Debug.Log here — Unity's API is main-thread only
//                 messageQueue.Enqueue($"[{remoteEP.Address}:{remoteEP.Port}] {message}");
//             }
//             catch (SocketException e)
//             {
//                 if (isRunning) // Ignore errors triggered by intentional socket close
//                     messageQueue.Enqueue("SocketError: " + e.Message);
//             }
//             catch (Exception e)
//             {
//                 if (isRunning)
//                     messageQueue.Enqueue("Error: " + e.Message);
//             }
//         }
//     }

//     void Update()
//     {
//         // Drain the queue on the main thread where Debug.Log is safe
//         while (messageQueue.TryDequeue(out string message))
//         {
//             Debug.Log("Received: " + message);
//             // TODO: do something with message in your game here
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();  // This unblocks udpClient.Receive() in the thread
//         receiveThread?.Join(500);
//     }
// }


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
