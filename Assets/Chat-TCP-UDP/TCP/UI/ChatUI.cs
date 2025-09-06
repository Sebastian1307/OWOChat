using UnityEngine;
using TMPro;

public class ChatUI : MonoBehaviour
{
    public Transform contentParent; // Content del ScrollView
    public GameObject messagePrefab; // Prefab con Username y MessageText

    public void AddMessage(string username, string text)
    {
        GameObject newMsg = Instantiate(messagePrefab, contentParent);

        TMP_Text[] texts = newMsg.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in texts)
        {
            if (t.name == "Username") t.text = username;
            if (t.name == "MessageText") t.text = text;
        }
    }
}
    