using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPAudioSender : MonoBehaviour
{
    public string ipAddress = "127.0.0.1";
    public int port = 5001;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private AudioClip micClip;
    private int lastSamplePos;
    public bool sendAudio = true;

    private const int chunkSize = 1024; // número de samples por paquete

    void Start()
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        micClip = Microphone.Start(null, true, 1, 44100);
        lastSamplePos = 0;
    }

    void Update()
    {
        if (!sendAudio || micClip == null) return;

        int pos = Microphone.GetPosition(null);
        if (pos < 0 || pos == lastSamplePos) return;

        int samplesToRead = pos - lastSamplePos;
        if (samplesToRead < 0) // buffer wrap-around
            samplesToRead += micClip.samples;

        float[] samples = new float[samplesToRead];
        micClip.GetData(samples, lastSamplePos);

        // dividir en chunks pequeños
        for (int i = 0; i < samples.Length; i += chunkSize)
        {
            int len = Mathf.Min(chunkSize, samples.Length - i);
            float[] chunk = new float[len];
            Array.Copy(samples, i, chunk, 0, len);

            byte[] bytes = new byte[len * 4];
            Buffer.BlockCopy(chunk, 0, bytes, 0, bytes.Length);

            udpClient.Send(bytes, bytes.Length, remoteEndPoint);
        }

        lastSamplePos = pos;
    }

    public void ToggleMic()
    {
        sendAudio = !sendAudio;
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
        Microphone.End(null);
    }
}
