﻿using System.Collections;
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
    public static int boardSet; //Confirm board is set before Cell Update calls

  
    //**** MONOBEHAVIOR FUNCTION *****

    void Awake()
    {
        //Reference Scripts
        board = boardObject.GetComponent<Board>();
        player = GameObject.Find("Ellen");
        flag = flagPrefab.GetComponent<Flag>();
    }


    // ***** Click *****
    // THIS METHOD ACTIVATES THE BOARD 
    // First square landed on will not be a mine

    public void Click(int x, int z)
    {
        cellScript = cellObject.GetComponent<Cell>();       //File crashed then got a bit buggy so had to 
        board = boardObject.GetComponent<Board>();          //reference again... :( 
        flag = flagPrefab.GetComponent<Flag>();

        boardSet += board.CreateMineFields(x, z);           //Creates the random field based on first square activated
        boardSet += board.AssignNum();                      //Counts the number of mines squares are next to 
    }


    // ***** Find Color *****
    // CHECK WHAT IS STORED IN EACH CUBE 
    // returns the color of the cube

    public int FindColor(int a, int b)
    {
        int x = board.CheckNum(a, b);
                     
        if (x == 9)                         // x == 9 calls a loss to the game
        {
           Cell.endgame = true;
           flag.EndGame();
        }
        if (x == 11)                        // x == 11 calls a win to the game
        {
            Cell.wingame = true;
            flag.EndGame();
        }
        return x;
    }


    //******* Check Around ********
    // CHECKS WHICH SQUARES TO OPEN 
    // Asks board to check around the square to see there are active blank squares
    // if so and square does not have a mine, it will turn on

    public int CheckAround(int a, int b)
    {
        return (board.NullAround(a, b));
    }


    //****** Is Board Set? *******
    //MAKES SURE BOARD IS SET BEFORE OPENIGN WHITE SQUARES
    //stops cell update functions if board hasn't been set

    public bool IsBoardSet()
    {
        if (boardSet == 2)      //2 represents the two method calls to set the board
        {
            return true;
        }
        else
            return false;
    }


    //********** FLAG METHODS ************
    // These methods deal with flagging the cells

    //creates a flag
    public void MakeFlag(int a, int b)
    {
        player = GameObject.Find("Ellen");                                                          //after file crashed, had to reference again :( 
        flagPositionVector = board.SetFlag(a, b, player.transform.rotation * Vector3.forward);      //determines direction player is facing and
                                                                                                    //if there is an eligible place to flag
        if (flagPositionVector != Vector3.zero)
            Instantiate(flagPrefab, (flagPositionVector), Quaternion.identity);
    }

    //checks to see if the cell is flagged from the board
    public bool IsFlagged(int a, int b)
    {
        return (board.SeeIfFlagged(a, b));
    }

    //asks the Cell to see if there is an update to being flagged or not
    public void CheckFlagToCell()
    {
        cellScript.CheckFlag();
    }
}