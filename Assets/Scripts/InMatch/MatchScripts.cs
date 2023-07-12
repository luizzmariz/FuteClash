using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchScripts : MonoBehaviour
{
    [Header("Setting Match")]
    public GameManager gm;
    public GameObject optionsScreen;
    public Material[] materials;
    public TMP_Text text;
    public TMP_Text score;
    public bool gameIsGoing;
    Data info;

    [Header("Ball")]
    public GameObject ball;
    public float ballPosX;
    public float ballPosY;
    
    [Header("Players")]
    public GameObject goalKeeperA;
    public Graphic goalKeeper;
    public GameObject leftPlayerA;
    public Graphic leftPlayer;
    public GameObject rightPlayerA;
    public Graphic rightPlayer;

    public GameObject goalKeeperB;
    public GameObject leftPlayerB;
    public GameObject rightPlayerB;

    [Header("Players movement")]
    public float goalKeeperWalk;
    public float playerWalk;
    public int lastDirection = 1;
    private bool canDash;
    private bool isDashing;
    private float dashingTime = 0.5f;
    private float dashTimer;
    public GameObject playerToMove;

    private int text321 = 3;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameIsGoing = false;
        info = new Data();

        dashTimer = 0;
        canDash = true;
        isDashing = false;

        playerToMove = leftPlayerA;
        info.goalKeeperX = goalKeeperA.GetComponent<Transform>().position.x;
        info.leftPlayerZ = leftPlayerA.GetComponent<Transform>().position.z;
        info.rightPlayerZ = rightPlayerA.GetComponent<Transform>().position.z;

        string teams = gm.SetMatchOptions(optionsScreen);
        SetTeams(teams);
        Invoke("StartText", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        SendInfo();
        //MoveBall();

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
        else if(dashTimer <= 0)
        {
            dashTimer = 0;
            canDash = true;
        }
        if (isDashing)
        {
            if(dashingTime > 0)
            {
                dashingTime -= Time.deltaTime;
            }
            else
            {
                dashingTime = 0.5f;
                isDashing = false;
            }
        }
        
        goalKeeperWalk = Input.GetAxis("Horizontal");

        if(goalKeeperWalk != 0 && !isDashing)
        {
            if(goalKeeperWalk > 0)
            {
                goalKeeperA.GetComponent<Rigidbody>().velocity = new Vector3(1f, 0f, 0f);
                lastDirection = 1;
            }
            else
            {
                goalKeeperA.GetComponent<Rigidbody>().velocity = new Vector3(-1f, 0f, 0f);
                lastDirection = -1;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            goalKeeperA.GetComponent<Rigidbody>().AddForce(lastDirection * 2.5f, 0, 0, ForceMode.Impulse);
            isDashing = true;
            canDash = false;
            dashTimer = 2.5f;
        }
        else if(goalKeeperWalk == 0)
        {
            if(goalKeeperA.GetComponent<Rigidbody>().velocity.x > 0)
            {
                goalKeeperA.GetComponent<Rigidbody>().velocity -= new Vector3(Time.deltaTime * 1.5f, 0f, 0f);
            }
            else if(goalKeeperA.GetComponent<Rigidbody>().velocity.x < 0)
            {
                goalKeeperA.GetComponent<Rigidbody>().velocity += new Vector3(Time.deltaTime * 1.5f, 0f, 0f);
            }
        }

        playerWalk = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Q))
        {
            playerToMove = leftPlayerA;
            rightPlayerA.GetComponent<Rigidbody>().velocity = new Vector3(0, 0f, 0f);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            playerToMove = rightPlayerA;
            leftPlayerA.GetComponent<Rigidbody>().velocity = new Vector3(0, 0f, 0f);
        }

        if(playerWalk != 0)
        {
            if(playerToMove.transform.position.z <= -1f && playerToMove.transform.position.z >= -3f)
            {
                playerToMove.GetComponent<Rigidbody>().velocity = new Vector3(0, 0f, playerWalk);
            }
            else
            {
                if(playerToMove.transform.position.z > -1f)
                {
                    playerToMove.GetComponent<Transform>().position = new Vector3(playerToMove.GetComponent<Transform>().position.x, 0.3F, -1F);
                }
                else
                {
                    playerToMove.GetComponent<Transform>().position = new Vector3(playerToMove.GetComponent<Transform>().position.x, 0.3F, -3F);
                }
            }
        }
        else 
        {
             playerToMove.GetComponent<Rigidbody>().velocity = new Vector3(0, 0f, 0f);
        }

        info.goalKeeperX = goalKeeperA.GetComponent<Transform>().position.x;
        info.leftPlayerZ = leftPlayerA.GetComponent<Transform>().position.z;
        info.rightPlayerZ = rightPlayerA.GetComponent<Transform>().position.z;
    }

    public void SetTeams(string teams)
    {
        switch(teams)
        {
            case "11":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[0];
            leftPlayer.color = new Color(0.9f, 0.35f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[1];
            goalKeeper.color = new Color(0.9f, 0.5f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[2];
            rightPlayer.color = new Color(0.4f, 0.2f, 0.1f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[0];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[1];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[2];
            break;

            case "12":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[0];
            leftPlayer.color = new Color(0.9f, 0.35f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[1];
            goalKeeper.color = new Color(0.9f, 0.5f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[2];
            rightPlayer.color = new Color(0.4f, 0.2f, 0.1f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[3];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[4];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[5];
            break;

            case "13":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[0];
            leftPlayer.color = new Color(0.9f, 0.35f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[1];
            goalKeeper.color = new Color(0.9f, 0.5f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[2];
            rightPlayer.color = new Color(0.4f, 0.2f, 0.1f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[6];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[7];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[8];
            break;

            case "21":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[3];
            leftPlayer.color = new Color(0.4f, 0.35f, 0.9f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[4];
            goalKeeper.color = new Color(0.9f, 0.75f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[5];
            rightPlayer.color = new Color(0.35f, 0.35f, 0.35f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[0];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[1];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[2];
            break;

            case "22":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[3];
            leftPlayer.color = new Color(0.4f, 0.35f, 0.9f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[4];
            goalKeeper.color = new Color(0.9f, 0.75f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[5];
            rightPlayer.color = new Color(0.35f, 0.35f, 0.35f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[3];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[4];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[5];
            break;

            case "23":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[3];
            leftPlayer.color = new Color(0.4f, 0.35f, 0.9f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[4];
            goalKeeper.color = new Color(0.9f, 0.75f, 0.1f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[5];
            rightPlayer.color = new Color(0.35f, 0.35f, 0.35f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[6];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[7];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[8];
            break;

            case "31":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[6];
            leftPlayer.color = new Color(0.6f, 0.9f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[7];
            goalKeeper.color = new Color(0.95f, 0.25f, 0.9f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[8];
            rightPlayer.color = new Color(0.15f, 0.85f, 0.9f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[0];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[1];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[2];
            break;

            case "32":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[6];
            leftPlayer.color = new Color(0.6f, 0.9f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[7];
            goalKeeper.color = new Color(0.95f, 0.25f, 0.9f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[8];
            rightPlayer.color = new Color(0.15f, 0.85f, 0.9f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[3];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[4];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[5];
            break;

            case "33":
            leftPlayerA.GetComponent<MeshRenderer>().material = materials[6];
            leftPlayer.color = new Color(0.6f, 0.9f, 0.35f, 1f);
            goalKeeperA.GetComponent<MeshRenderer>().material = materials[7];
            goalKeeper.color = new Color(0.95f, 0.25f, 0.9f, 1f);
            rightPlayerA.GetComponent<MeshRenderer>().material = materials[8];
            rightPlayer.color = new Color(0.15f, 0.85f, 0.9f, 1f);

            leftPlayerB.GetComponent<MeshRenderer>().material = materials[6];
            goalKeeperB.GetComponent<MeshRenderer>().material = materials[7];
            rightPlayerB.GetComponent<MeshRenderer>().material = materials[8];
            break;

            default:
            break;
        }
    }

    public void StartText()
    {
        if(text321 > 0)
        {
            text.text = "Jogo vai começar em " + text321 + "...";
            Invoke("StartText", 1.0f);
            text321--;
        }
        else
        {
            gameIsGoing = true;
            text.text = "";
            text321 = 3;
        }
    }

    public void SendInfo()
    {
        gm.SendMessage("MatchInformation", info);
    }

    public void MoveBall()
    {
        //ball.GetComponent<Transform>().position = new Vector3(ballPosX, 0f, ballPosX);
    }

    public void ReceiveInfo(Data gg)
    {
        //ballPosX = gg.ballX;
        //ballPosY = gg.ballY;

        goalKeeperB.GetComponent<Transform>().position = new Vector3(gg.goalKeeperX, 0f, 4.7f);
        leftPlayerB.GetComponent<Transform>().position = new Vector3(1.5f, 0f, gg.leftPlayerZ);
        rightPlayerB.GetComponent<Transform>().position = new Vector3(-1.5f, 0f, gg.rightPlayerZ);

        //score.text = gg.pontuatiuon;
    }
}
