using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UDPVideoSender : MonoBehaviour
{
    // Cambiamos la IP de "127.0.0.1" a "255.255.255.255" para broadcast
    public string ipAddress = "255.255.255.255";
    public int port = 5000;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private WebCamTexture webcamTexture;
    private Texture2D frameTex;
    public RawImage localPreview;

    public bool sendVideo = true;

    void Start()
    {
        udpClient = new UdpClient();
        // Habilitamos la opción de broadcast en el socket UDP
        udpClient.EnableBroadcast = true;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        // Resolución reducida y FPS bajos para UDP
        webcamTexture = new WebCamTexture(160, 120, 10);
        localPreview.texture = webcamTexture;
        webcamTexture.Play();
    }

    void Update()
    {
        if (!sendVideo || webcamTexture == null || !webcamTexture.isPlaying) return;
        if (webcamTexture.width < 16 || webcamTexture.height < 16) return;

        if (frameTex == null || frameTex.width != webcamTexture.width || frameTex.height != webcamTexture.height)
        {
            frameTex = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        }

        frameTex.SetPixels(webcamTexture.GetPixels());
        frameTex.Apply();

        // JPG comprimido con calidad baja
        byte[] jpg = frameTex.EncodeToJPG(15);
        udpClient.Send(jpg, jpg.Length, remoteEndPoint);
        Debug.Log("Frame enviado: " + jpg.Length + " bytes a " + remoteEndPoint);
    }

    public void ToggleCamera()
    {
        sendVideo = !sendVideo;
    }

    private void OnApplicationQuit()
    {
        if (webcamTexture != null) webcamTexture.Stop();
        udpClient.Close();
    }
}