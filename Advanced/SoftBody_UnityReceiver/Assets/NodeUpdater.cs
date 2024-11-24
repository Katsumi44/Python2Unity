using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NodeUpdater : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    public Transform[] nodes; // Assign control nodes here via the Unity Inspector

    void Start()
    {
        // Initialize UDP listener
        udpClient = new UdpClient(12345); // Port must match the one in your Python script
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    }

    void Update()
    {
        // Check for data from Python
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string positionsStr = Encoding.UTF8.GetString(data);

            // Parse positions and update nodes
            string[] positions = positionsStr.Split(';');
            for (int i = 0; i < nodes.Length; i++)
            {
                string[] coords = positions[i].Split(',');
                float x = float.Parse(coords[0]);
                float y = float.Parse(coords[1]);
                float z = float.Parse(coords[2]);

                nodes[i].position = new Vector3(x, y, z);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Close the UDP client on quit
        udpClient.Close();
    }
}
