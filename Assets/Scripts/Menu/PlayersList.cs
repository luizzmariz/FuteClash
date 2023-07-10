using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayersList : MonoBehaviour
{
    public GameManager gm;

    public GameObject playerButton;

    void Start()
    {
        
    }

    public void ListUpdate(Data playersData) 
    {
        Debug.Log("Atualizando lista de players");
        foreach(Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach(Data.Player p in playersData.playersList)
        {
            GameObject justcreattedButton = Instantiate(playerButton, transform);
            justcreattedButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = p.playerUsername;
            justcreattedButton.SendMessage("setChatId", p.id);
        }
    }
}
