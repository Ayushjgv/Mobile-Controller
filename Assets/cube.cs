using System;
using UnityEngine;
using System.Collections.Concurrent;

public class Cube : MonoBehaviour
{
    public Receiver receiver;
    public float sensitivity = 100f;

    void Update()
    {
        if (receiver == null) return;

        while (receiver.messageQueue.TryDequeue(out string json))
        {
            try
            {
                GyroData data = JsonUtility.FromJson<GyroData>(json);
                if (data == null) return;

                float x = data.gyrX * sensitivity * Time.deltaTime;
                float y = data.gyrY * sensitivity * Time.deltaTime;
                float z = data.gyrZ * sensitivity * Time.deltaTime;

                transform.Rotate(x, y, z, Space.World);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to parse packet: " + e.Message);
            }
        }
    }

    [System.Serializable]
    private class GyroData
    {
        public float gyrX;
        public float gyrY;
        public float gyrZ;
        public long  gyr_time;
    }
}