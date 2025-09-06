using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Concurrent;

public class TCPServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient connectedClient;
    private NetworkStream networkStream;
    private byte[] receiveBuffer;

    public bool isServerRunning;
    public string username = "Servidor";

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

    public void StartServer(int port, string userNameInput)
    {
        username = userNameInput;
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        Debug.Log("Server started, waiting for connections...");
        tcpListener.BeginAcceptTcpClient(HandleIncomingConnection, null);
        isServerRunning = true;
    }

    private void HandleIncomingConnection(IAsyncResult result)
    {
        connectedClient = tcpListener.EndAcceptTcpClient(result);
        networkStream = connectedClient.GetStream();
        Debug.Log("Client connected: " + connectedClient.Client.RemoteEndPoint);

        receiveBuffer = new byte[connectedClient.ReceiveBufferSize];
        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);

        tcpListener.BeginAcceptTcpClient(HandleIncomingConnection, null);
    }

    private void ReceiveData(IAsyncResult result)
    {
        int bytesRead = networkStream.EndRead(result);
        if (bytesRead <= 0)
        {
            Debug.Log("Client disconnected");
            connectedClient.Close();
            return;
        }

        byte[] receivedBytes = new byte[bytesRead];
        Array.Copy(receiveBuffer, receivedBytes, bytesRead);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);

        Debug.Log("Received from client: " + receivedMessage);

        // Separar username|mensaje
        string[] parts = receivedMessage.Split('|');
        string sender = parts.Length > 1 ? parts[0] : "Cliente";
        string text = parts.Length > 1 ? parts[1] : receivedMessage;

        mainThreadActions.Enqueue(() =>
        {
            chatUI.AddMessage(sender, text);
        });

        networkStream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveData, null);
    }


    private void SendRawToClient(string message)
    {
        try
        {
            if (networkStream == null) return;

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(message);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();

            Debug.Log("Forwarded to client: " + message);
        }
        catch
        {
            Debug.Log("Could not forward message to client.");
        }
    }

    public void SendData(string message)
    {
        try
        {
            if (networkStream == null) return;

            string fullMessage = username + "|" + message;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(fullMessage);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();

            mainThreadActions.Enqueue(() =>
            {
                chatUI.AddMessage(username, message);
            });

            Debug.Log("Sent to client: " + fullMessage);
        }
        catch
        {
            Debug.Log("There is no client to send the message: " + message);
        }
    }

}
