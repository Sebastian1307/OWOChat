using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPAudioReceiver : MonoBehaviour
{
    public int port = 5001;
    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;

    private AudioSource audioSource;

    private const int bufferSize = 44100 * 2; // 2s buffer
    private float[] audioBuffer = new float[bufferSize];
    private int writePos = 0;
    private int readPos = 0;

    void Start()
    {
        udpServer = new UdpClient(port);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        udpServer.BeginReceive(ReceiveData, null);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
    }

    private void ReceiveData(IAsyncResult result)
    {
        byte[] data = udpServer.EndReceive(result, ref remoteEndPoint);
        float[] samples = new float[data.Length / 4];
        Buffer.BlockCopy(data, 0, samples, 0, data.Length);

        lock (audioBuffer)
        {
            foreach (var s in samples)
            {
                audioBuffer[writePos] = s;
                writePos = (writePos + 1) % bufferSize;
            }
        }

        udpServer.BeginReceive(ReceiveData, null);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        lock (audioBuffer)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = audioBuffer[readPos];
                readPos = (readPos + 1) % bufferSize;
            }
        }
    }

    private void OnApplicationQuit()
    {
        udpServer.Close();
    }
}
