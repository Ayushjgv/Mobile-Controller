using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

[Serializable]
public class GyroData
{
    public float gyrX;
    public float gyrY;
    public float gyrZ;
}

public class GyroController : MonoBehaviour
{
    Socket socket;
    byte[] buffer = new byte[1024];
    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    public int port = 6000;

    Vector3 rotation = Vector3.zero;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(new IPEndPoint(IPAddress.Any, port));

        Debug.Log("Gyro Receiver Started");
    }

    void Update()
    {
        if (socket.Available > 0)
        {
            int length = socket.ReceiveFrom(buffer, ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(buffer, 0, length);

            int jsonStart = message.IndexOf('{');
            if (jsonStart >= 0)
                message = message.Substring(jsonStart);

            GyroData data = JsonUtility.FromJson<GyroData>(message);

            float sensitivity = 200f;

            rotation.x += data.gyrY * sensitivity * Time.deltaTime;
            rotation.y += -data.gyrX * sensitivity * Time.deltaTime;

            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    void OnApplicationQuit()
    {
        socket?.Close();
    }
}