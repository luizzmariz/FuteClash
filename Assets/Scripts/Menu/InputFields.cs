using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFields : MonoBehaviour
{
    public GameManager gm;

    public TMP_Text usernameText;
    public TMP_Text emailText;

    public void SignIn()
    {
        if(usernameText.text != null && emailText.text != null)
        {
            gm.SetPlayerInfo(usernameText.text, emailText.text);
        }
        else
        {
            Debug.Log("Preencha todas as informações");
        }
    }
}
