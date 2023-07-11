using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchScripts : MonoBehaviour
{
    public GameObject gm;
    public GameObject optionsScreen;

    public Rigidbody ball;
    public Rigidbody goalKeeper;

    private bool canDash;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager");

        //gm.SendMessage("SetMatchOptions", optionsScreen);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //goalKeeper.AddForce()
        }
        else if(Input.GetKeyDown(KeyCode.D) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            //goalKeeper.AddForce()
        }
        else if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            //goalKeeper.AddForce()
        }
    }
}
