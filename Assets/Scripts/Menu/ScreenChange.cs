using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenChange : MonoBehaviour
{
    public GameManager gm;
    public string buttonName;

    public Button thisButton;

    void Start()
    {
        thisButton = GetComponent<Button>();
    }

    public void ChangeScreen()
    {
        gm.ScreenChanger(buttonName);
    }

    public void SetInteractability()
    {
        if(thisButton.interactable)
        {
            thisButton.interactable = false;
        }
        else
        {
            thisButton.interactable = true;
        }
    }
}
