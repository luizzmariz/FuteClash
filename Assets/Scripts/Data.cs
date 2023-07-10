using System;
using System.Collections;
using System.Collections.Generic;
public class Data
{
    public string name;

    //message
    public struct Messages
    {
        public string senderId;
        public string chatId;
        public string message;
    }
    public Messages Message;
    
    //signIn
    public bool signedCheck;
    public bool usernameCheck;
    public bool emailCheck;

    public struct Player
    {
        public string id;
        public string playerUsername;
        public string playerEmail;
    }

    //onlinePlayers
    public int onlinePlayers;
    public List<Player> playersList;
    public bool queueList;

    public Player oponent;
    public int team;
}
