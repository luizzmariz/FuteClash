using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageList : MonoBehaviour
{
    public GameManager gm;

    //messages
    public TMP_Text inputedMessage;
    public GameObject message;

    //chat buttons and scrollbar
    public Button globalChatButton;
    public Button privateChatButton;
    public ScrollRect SR;

    //prefab for every private chat
    public GameObject privateChatPage;

    //chats
    public GameObject globalChat;
    public GameObject privateChat;
    public List<string> privateChats;

    //internal variables
    public string currentChat;
    public string currentPrivateChat;
    public GameObject currentPrivateChatGO;

    void Start()
    {
        currentChat = "global";
        currentPrivateChat = " ";
        globalChat.SetActive(true);
        SR = GetComponent<ScrollRect>();
        SR.content = globalChat.GetComponent<RectTransform>();
        globalChatButton.interactable = false;
        privateChatButton.interactable = false;
    }

    public void SendMessage()
    {
        gm.MessageSender(inputedMessage.text, currentChat);
        if(currentChat != "global")
        {
            GameObject justcreattedMessage = Instantiate(this.message, privateChat.transform.GetChild(privateChats.IndexOf(currentPrivateChat)).transform);
            justcreattedMessage.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Eu";
            justcreattedMessage.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = inputedMessage.text;
        }
        inputedMessage.text = "";
    }

    public void ReceiveMessage(Data playersData)
    {   
        if(playersData.Message.chatId == "global" && !gm.playerStatus())
        {
            GameObject justcreattedMessage = Instantiate(this.message, globalChat.transform);
            justcreattedMessage.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = playersData.Message.senderId;
            justcreattedMessage.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playersData.Message.message;
        }
        else if(playersData.Message.chatId != "global")
        {
            int privChat;
            privateChatButton.interactable = true;
            if(!privateChats.Contains(playersData.Message.chatId))
            {
                CreatePrivateChat(playersData.Message.chatId);
            }
            if(currentPrivateChat == " ")
            {
                currentPrivateChat = playersData.Message.chatId;
                currentPrivateChatGO = privateChat.transform.GetChild(privateChats.IndexOf(playersData.Message.chatId)).gameObject;
            }
            privChat = privateChats.IndexOf(playersData.Message.chatId);
            GameObject justcreattedMessage = Instantiate(this.message, privateChat.transform.GetChild(privChat).transform);
            justcreattedMessage.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = playersData.Message.senderId;
            justcreattedMessage.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = playersData.Message.message;
        }
    }

    public void ChatChanger(string chatBLA)
    {
        
        if(chatBLA == "global")
        {
            currentChat = "global";
            globalChat.SetActive(true);
            SR.content = globalChat.GetComponent<RectTransform>();
            currentPrivateChatGO.SetActive(false);
            globalChatButton.interactable = false;
            privateChatButton.interactable = true;
        }
        else
        {
            if(chatBLA != "private")
            {
                if(currentPrivateChatGO != null)
                {
                    currentPrivateChatGO.SetActive(false);
                }
                currentPrivateChat = chatBLA;
                if(!privateChats.Contains(chatBLA))
                {
                    CreatePrivateChat(chatBLA);
                }
                currentPrivateChatGO = privateChat.transform.GetChild(privateChats.IndexOf(chatBLA)).gameObject;
            }
            currentChat = currentPrivateChat;
            globalChat.SetActive(false);
            currentPrivateChatGO.SetActive(true);
            SR.content = currentPrivateChatGO.GetComponent<RectTransform>();
            globalChatButton.interactable = true;
            privateChatButton.interactable = false;
        }
    }

    public void CreatePrivateChat(string chatName)
    {
        privateChats.Add(chatName);
        GameObject justcreattedChat = Instantiate(privateChatPage, transform.GetChild(1).gameObject.transform);
        justcreattedChat.SetActive(false);
    }
}
