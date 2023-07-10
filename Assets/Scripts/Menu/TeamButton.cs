using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamButton : MonoBehaviour
{
    public GameManager gm;

    public int team;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SelectTeam()
    {
        gm.SendMessage("ChangeTeam", team);
    }
}
