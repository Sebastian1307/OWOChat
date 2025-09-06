using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UDPVideoReceiver : MonoBehaviour
{
    public int port = 5000;
    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    public RawImage remoteView;
    private Texture2D tex;

    void Start()
    {
        // El servidor escucha en todas las IPs disponibles en el puerto especificado
        udpServer = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        udpServer.BeginReceive(ReceiveData, null);
        tex = new Texture2D(2, 2);
        remoteView.texture = tex;
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] receivedBytes = udpServer.EndReceive(result, ref remoteEndPoint);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                Debug.Log("Frame recibido: " + receivedBytes.Length + " bytes de " + remoteEndPoint);
                tex.LoadImage(receivedBytes);
                tex.Apply();
                remoteView.texture = tex;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error decodificando el frame: " + e.Message);
            }
        });
        udpServer.BeginReceive(ReceiveData, null);
    }

    private void OnApplicationQuit()
    {
        udpServer.Close();
    }
}