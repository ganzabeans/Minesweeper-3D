using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    //GAME OBJECTS
    public GameObject boardObject;
    public GameObject cellObject;
    private GameObject player;
    public GameObject flagPrefab;
    
    //SCRIPTS
    private Cell cellScript;
    private Board board;
    private Flag flag;
    Vector3 flagPositionVector;

    //PUBLIC STATIC VARS
    public static int boardSet;

    //BOOLS
    bool m_flag;

  
    //********** MONOBEHAVIOR FUNCTIONS ************

    void Awake()
    {
        //boardObject = GameObject.FindGameObjectWithTag("ThisIsBoard");
        //Debug.Log("Board object is " + boardObject);
        board = boardObject.GetComponent<Board>();
        //Debug.Log("Board script is " + board);
        player = GameObject.Find("Ellen");
        //Debug.Log(player);
        flag = flagPrefab.GetComponent<Flag>();
    }

    // ***** THIS METHOD ACTIVATES THE BOARD *****
    // First step will not be a mine 
    public void Click(int x, int z)
    {
        cellScript = cellObject.GetComponent<Cell>();
        board = boardObject.GetComponent<Board>();
        flag = flagPrefab.GetComponent<Flag>();
       // Debug.Log("Board script is " + board);

        boardSet += board.CreateMineFields(x, z);
        boardSet += board.AssignNum();
    }

    // ***** CHECKS WHAT IS STORED IN EACH CUBE *****
    // returns the color to be updated on the cube
    public int FindColor(int a, int b)
    {
        int x = board.CheckNum(a, b);
        //calls an end to the game
        if (x == 9)
        {
           Cell.endgame = true;
           flag.EndGame();
        }
        if (x == 11)
        {
            Cell.wingame = true;
            flag.EndGame();
        }
        return x;
    }

    //****** CHECKS WHICH WHTIE SQUARES TO OPEN
    public int CheckAround(int a, int b)
    {
        return (board.NullAround(a, b));
    }


    //**** MAKES SURE BOARD IS SET BEFORE OPENIGN WHITE SQUARES
    public bool IsBoardSet()
    {
        if (boardSet == 2)
        {
            //Debug.Log(boardSet);
            return true;
        }
        else
            return false;
    }

    //******** FLAG METHODS ************
    public void MakeFlag(int a, int b)
    {
        player = GameObject.Find("Ellen");

        flagPositionVector = board.SetFlag(a, b, player.transform.rotation * Vector3.forward);
        //Debug.Log(flagPositionVector);
        if (flagPositionVector != Vector3.zero)
            Instantiate(flagPrefab, (flagPositionVector), Quaternion.identity);
    }

    public bool IsFlagged(int a, int b)
    {
        Debug.Log(board);

        return (board.SeeIfFlagged(a, b));

    }

    public void CheckFlagToCell()
    {
        cellScript.CheckFlag();
    }
}
