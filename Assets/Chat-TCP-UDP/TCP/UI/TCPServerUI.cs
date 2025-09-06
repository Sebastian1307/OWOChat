    using UnityEngine;
    using TMPro;

    public class TCPServerUI : MonoBehaviour
    {
        public int serverPort = 4000;
        [SerializeField] private TCPServer _server;
        [SerializeField] private TMP_InputField messageInput;
        public TMP_InputField userInput;

        public void SendServerMessage()
        {
            if (!_server.isServerRunning)
            {
                Debug.Log("The server is not running");
                return;
            }

            if (string.IsNullOrEmpty(messageInput.text))
            {
                Debug.Log("The chat entry is empty");
                return;
            }

            string message = messageInput.text;
            _server.SendData(message); // esto ya lo muestra en el server UI
            messageInput.text = "";
        }

        public void StartServer()
        {
            string username = string.IsNullOrEmpty(userInput.text) ? "Servidor" : userInput.text;
            _server.StartServer(serverPort, username);
        }
    }
