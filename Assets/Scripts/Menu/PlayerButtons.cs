using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtons : MonoBehaviour
{
    public string chatId;

    public GameObject messagePanel;
    public GameObject gameManager;

    void Start()
    {
        messagePanel = GameObject.Find("MessagePanel");
        gameManager = GameObject.Find("GameManager");
    }

    public void setChatId(string Id)
    {
        this.chatId = Id;
    }

    public void ChangeChat()
    {
        messagePanel.SendMessage("ChatChanger", chatId);
    }

    public void StartQueue()
    {
        gameManager.SendMessage("EnterQueue", chatId);
    }
}
