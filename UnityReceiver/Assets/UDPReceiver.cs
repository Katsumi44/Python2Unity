using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class PointData
    {
        public Point[] points;
    }

    public GameObject[] points; // Assign 5 GameObjects in the Inspector
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    public int port = 5005;

    void Start()
    {
        udpClient = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        Debug.Log($"Listening for UDP packets on port {port}");

        // Validate the points array
        if (points == null || points.Length != 5)
        {
            Debug.LogError("The 'points' array must contain exactly 5 GameObjects.");
            return;
        }

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null)
            {
                Debug.LogError($"Point {i} is not assigned. Assign a GameObject in the Inspector.");
            }
        }
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string jsonData = Encoding.UTF8.GetString(data);
            Debug.Log($"Raw JSON Data Received: {jsonData}");

            try
            {
                PointData pointsData = JsonUtility.FromJson<PointData>(jsonData);

                if (pointsData == null || pointsData.points == null)
                {
                    Debug.LogWarning("Parsed data is null or invalid.");
                    return;
                }

                UpdatePoints(pointsData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing data: {ex.Message}");
            }
        }
    }

    void UpdatePoints(PointData pointsData)
    {
        if (pointsData.points.Length != points.Length)
        {
            Debug.LogWarning("Received data does not match the number of points.");
            return;
        }

        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                Vector3 position = new Vector3(
                    pointsData.points[i].x,
                    pointsData.points[i].y,
                    pointsData.points[i].z
                );

                Debug.Log($"Updating Point {i}: {position}");
                points[i].transform.position = position;
            }
            else
            {
                Debug.LogError($"Point {i} in the 'points' array is null.");
            }
        }
    }
}
