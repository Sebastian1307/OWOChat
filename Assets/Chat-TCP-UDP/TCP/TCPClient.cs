using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Concurrent;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private byte[] receiveBuffer;

    public bool isServerConnected;
    public string username = "Cliente";

    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
    private ChatUI chatUI;

    private void Awake()
    {
        chatUI = FindObjectOfType<ChatUI>();
    }

    private void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    public void ConnectToServer(string ipAddress, int port, string userNameInput)
    {
        username = userNameInput;
        tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Parse(ipAddress), port);
        networkStream = tcpClient.GetStream();
        receiveBuffer = new byte[tcpClient.ReceiveBufferSize];
        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
        isServerConnected = true;
    }

    private void ReceiveData(IAsyncResult result)
    {
        int bytesRead = networkStream.EndRead(result);
        if (bytesRead <= 0) return;

        byte[] receivedBytes = new byte[bytesRead];
        Array.Copy(receiveBuffer, receivedBytes, bytesRead);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        Debug.Log("Received from server: " + receivedMessage);

        string[] parts = receivedMessage.Split('|');
        string sender = parts.Length > 1 ? parts[0] : "Servidor";
        string text = parts.Length > 1 ? parts[1] : receivedMessage;

        mainThreadActions.Enqueue(() =>
        {
            chatUI.AddMessage(sender, text);
        });

        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
    }


    public void SendData(string message)
    {
        try
        {
            if (networkStream == null) return;

            // Combinar username + mensaje
            string fullMessage = username + "|" + message;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(fullMessage);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();

            // Mostrar en el client UI al enviar
            mainThreadActions.Enqueue(() =>
            {
                chatUI.AddMessage(username, message);
            });

            Debug.Log("Sent to server: " + fullMessage);
        }
        catch
        {
            Debug.Log("There is no server to send the message: " + message);
        }
    }

}
