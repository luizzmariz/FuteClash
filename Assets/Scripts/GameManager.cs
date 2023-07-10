#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Screens")]
    public GameObject startScreen;
    public GameObject signInScreen;
    public GameObject lobbyScreen;
    public GameObject optionsScreen;
    public GameObject deckEditScreen;
    public GameObject playMatchScreen;
    public GameObject chatScreen;
    public GameObject messagePanel;
    public GameObject matchOptions;
    Data.Messages msg;

    public GameObject gameLogo;

    [Header("ServerConnection")]
    public bool isConnected = false; 
    public bool isTryingToConnect = false; 
    public bool connectedAux = false; 
    public bool serverClosed = false; 
    public GameObject startButton;
    private SocketIOUnity socket;

    [Header("Player")]
    public bool playerAcceptable = false;
    public GameObject playersListPanel;
    public GameObject invalidUsername;
    public GameObject invalidEmail;
    Data.Player player;
    Data.Player oponent;
    private int team = 1;
    private int oponentTeam;
    
    [Header("Match")]
    public bool isInMatch;
    public bool isQueueing;
    public string challenged;
    public bool matchVariablesAssigned = false;
    public GameObject onQueueListPanel;
    public Button matchButton;
    

    public void Start()
    {
        if(instance == null) 
        {
			instance = this;
		}
        else if(instance != this) 
        {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

        startScreen.SetActive(true);
        gameLogo.SetActive(true);
        optionsScreen.SetActive(false);
        signInScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        deckEditScreen.SetActive(false);
        playMatchScreen.SetActive(false);
        chatScreen.SetActive(true);

        invalidUsername.SetActive(false);
        invalidEmail.SetActive(false);
        isInMatch = false;
        isQueueing = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ScreenChanger("options");
        }
        if(connectedAux)
        {
            ScreenChanger("startGame");
            connectedAux = false;
        }
        if(serverClosed)
        {
            QuitGame();
        }
    }

    public void ScreenChanger(string screen)
    {
        switch(screen)
        {
            case "startGame":
            if(isConnected)
            {
                startButton.SendMessage("SetInteractability");
                startScreen.SetActive(false);
                signInScreen.SetActive(true);
            }
            else if(!isConnected)
            {
                ServerConection();
            }
            break;

            case "options":
            if((!isInMatch))
            {
                if(!optionsScreen.activeInHierarchy)
                {
                    optionsScreen.SetActive(true);
                }
                else if(optionsScreen.activeInHierarchy)
                {
                    optionsScreen.SetActive(false);
                }
            }
            else if(matchOptions != null)
            {
                if(!matchOptions.activeInHierarchy)
                {
                    matchOptions.SetActive(true);
                }
                else if(matchOptions.activeInHierarchy)
                {
                    matchOptions.SetActive(false);
                }
            }
            break;

            case "exitGame":
            QuitGame();
            break;

            case "signIn":
            if(!playerAcceptable)
            {
                SocketEmit("signIn");
            }
            else if(playerAcceptable)
            {
                signInScreen.SetActive(false);
                gameLogo.SetActive(false);
                lobbyScreen.SetActive(true);
            }
            break;

            case "deckEdit":
            chatScreen.SetActive(false);
            playMatchScreen.SetActive(false);
            deckEditScreen.SetActive(true);
            break;

            case "playMatch":
            chatScreen.SetActive(false);
            deckEditScreen.SetActive(false);
            playMatchScreen.SetActive(true);
            SocketEmit("getQueueInfo");
            break;

            case "chat":
            playMatchScreen.SetActive(false);
            deckEditScreen.SetActive(false);
            chatScreen.SetActive(true);
            break;

            case "createMatch":
            optionsScreen.SetActive(false);
            SceneManager.LoadScene("Match");
            //optionsScreenInGame = 
            break;

            case "endMatch":
            optionsScreen.SetActive(false);
            SceneManager.LoadScene("LobbyMenu");
            matchButton.interactable = true;
            break;

            case "disconnect":
            if(isConnected)
            {
                socket.Disconnect();
                signInScreen.SetActive(false);
                optionsScreen.SetActive(false);
                lobbyScreen.SetActive(false);
                startScreen.SetActive(true);
                gameLogo.SetActive(true);
            }
            chatScreen.SetActive(false);
            break;

            default:
            Debug.Log("some button pressed");
            break;
        }
    }

    public void ServerConection()
    {
        var uri = new Uri("http://localhost:6900/");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 4
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        if(!isConnected)
        {
            socket.Connect();
            isTryingToConnect = true;
        }

        if(isTryingToConnect)
        {
            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("Connection started");
                isTryingToConnect = false;
                isConnected = true;
                connectedAux = true;
            };
        }
        
        socket.OnDisconnected += (sender, e) =>
        {
            isConnected = false;
            Debug.Log("disconnect: " + e);
            if(e.ToString() == "transport close")
            {
                serverClosed = true;
            }
        };

        socket.OnUnityThread("signIn", (data) =>
        {
            List<Data> deserializedProduct = JsonConvert.DeserializeObject<List<Data>>(data.ToString());
            if(deserializedProduct[0].signedCheck)
            {
                playerAcceptable = true;
                ScreenChanger("signIn");
                invalidUsername.SetActive(false);
                invalidEmail.SetActive(false);
            }
            else if(!deserializedProduct[0].signedCheck)
            {
                if(!deserializedProduct[0].usernameCheck)
                {
                    invalidUsername.SetActive(true);
                }
                else if(deserializedProduct[0].usernameCheck)
                {
                    invalidUsername.SetActive(false);
                }
                if(!deserializedProduct[0].emailCheck)
                {
                    invalidEmail.SetActive(true);
                }
                else if(deserializedProduct[0].emailCheck)
                {
                    invalidEmail.SetActive(false);
                }
            }
        });

        socket.OnUnityThread("playersList", (data) =>
        {
            //Debug.Log(data);
            List<Data> deserializedProduct = JsonConvert.DeserializeObject<List<Data>>(data.ToString());
            for(int i = 0; i < deserializedProduct[0].playersList.Count; i++)
            {
                if(deserializedProduct[0].playersList[i].playerUsername == player.playerUsername)
                {
                    deserializedProduct[0].playersList.RemoveAt(i);
                }
            }
            if(deserializedProduct[0].queueList)
            {
                Debug.Log("Teste 4");
                onQueueListPanel.SendMessage("ListUpdate", deserializedProduct[0]);
            }
            else
            {
                playersListPanel.SendMessage("ListUpdate", deserializedProduct[0]);
            }
        });

        socket.OnUnityThread("message", (data) =>
        {
            //Debug.Log(data);
            List<Data> deserializedProduct = JsonConvert.DeserializeObject<List<Data>>(data.ToString());
            messagePanel.SendMessage("ReceiveMessage", deserializedProduct[0]);
        });

        socket.OnUnityThread("startMatch", (data) =>
        {
            //Debug.Log(data);
            List<Data> deserializedProduct = JsonConvert.DeserializeObject<List<Data>>(data.ToString());
            isQueueing = false;
            isInMatch = true;
            ScreenChanger("createMatch");
            this.oponent = deserializedProduct[0].oponent;
            oponentTeam = deserializedProduct[0].team;
        });
    }

    public void SocketEmit(string channel)
    {
        switch(channel)
        {
            case "signIn":
            socket.Emit("signIn", player);
            break;

            case "message":
            socket.Emit("message", msg);
            break;

            case "onQueue":
            Debug.Log("Teste 3.1");
            socket.Emit("onQueue", isQueueing);
            break;

            case "challengeSomeone":
            Debug.Log("Teste 3.2");
            socket.Emit("challengeSomeone", challenged);
            break;

            case "getQueueInfo":
            socket.Emit("getQueueInfo");
            break;

            default:
            break;
        }
    }

    public void SetPlayerInfo(string username, string email)
    {
        this.player.playerUsername = username;
        this.player.playerEmail = email;
        ScreenChanger("signIn");
    }

    public void MessageSender(string message, string chat)
    {
        this.msg.message = message;
        this.msg.chatId = chat;
        SocketEmit("message");
    }

    public void EnterQueue(string enemy)
    {
        if(enemy == "none")
        {
            matchButton.interactable = false;
            isQueueing = true;
            Debug.Log("Teste 2.1");
            SocketEmit("onQueue");
        }
        else
        {
            matchButton.interactable = false;
            challenged = enemy;
            Debug.Log("Teste 2.2");
            SocketEmit("challengeSomeone");
        }
    }

    public void SetMatchOptions(GameObject opt)
    {
        matchOptions = opt;
    }

    public void EndMatch()
    {
        //SocketEmit("message");
        ScreenChanger("endMatch");
    }

    public bool playerStatus()
    {
        return this.isInMatch;
    }

    public void ChangeTeam(int team)
    {
        this.team = team;
    }
    
    public void QuitGame()
    {
        ScreenChanger("disconnect");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}

