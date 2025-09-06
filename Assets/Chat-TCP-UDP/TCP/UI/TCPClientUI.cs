using UnityEngine;
using TMPro;

public class TCPClientUI : MonoBehaviour
{
    public int serverPort = 4000;
    public string serverAddress = "127.0.0.1";
    [SerializeField] private TCPClient _client;
    [SerializeField] private TMP_InputField messageInput;
    public TMP_InputField userInput;

    public void SendClientMessage()
    {
        if (!_client.isServerConnected)
        {
            Debug.Log("The client is not connected");
            return;
        }

        if (string.IsNullOrEmpty(messageInput.text))
        {
            Debug.Log("The chat entry is empty");
            return;
        }

        string message = messageInput.text;
        _client.SendData(message); // esto ya lo muestra en el client UI
        messageInput.text = "";
    }

    public void ConnectClient()
    {
        string username = string.IsNullOrEmpty(userInput.text) ? "Cliente" : userInput.text;
        _client.ConnectToServer(serverAddress, serverPort, username);
    }
}
