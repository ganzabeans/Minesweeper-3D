using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell : MonoBehaviour
{

    //GAME OBJECTS
    GameObject player;
    GameObject boardControllerObject;
    public GameObject cube;
    public GameObject flag;
    public GameObject boxRender;

    //SCRIPTS
    static private BoardController boardController;
    private BoxRender colorBox;

    //PUBLIC STATICS
    public static bool firstClick;      //when first click happens is ture
    public static bool endgame, wingame;

   //BOOLS
    public bool inTrigger;
    bool steppedOn;
    public bool flagged;

    //INTEGERS
    int colorNum;
    int a, b; //coordinate location


    //******** METHODS *******************

    // MONOBEHAVIOR METHODS 

    //***** START **********
    //Find elen object to detect in trigger, set array 

    void Start ()                       
    { 
        player = GameObject.Find("Ellen");
		boardControllerObject = GameObject.Find("BoardController");

        a = ((int)cube.transform.position.x / 2);               //position in array
        b = ((int)cube.transform.position.z / 2);               //position in array

        colorBox = boxRender.GetComponent<BoxRender>();                                     //reference class for rendering
        boardController = boardControllerObject.GetComponent<BoardController>();            //reference boardController
    }

    void Update()
    {
        //checks if other cubes around are white
        //if true, then they need to be turned on if they dont' have a mine in them
        if (!endgame && !wingame && !flagged && boardController.IsBoardSet() && !steppedOn && firstClick)
        {
            colorNum = boardController.CheckAround(a, b);
            if (colorNum != 11)
            {
                steppedOn = true;
                colorBox.SetColor(colorNum);
            }
        }
        //sends coordinates to make a flag
        if (inTrigger && !endgame && !wingame && (boardController.IsBoardSet() && firstClick))
        {
            if (Input.GetKeyDown(KeyCode.F))
                boardController.MakeFlag(a, b);
        }
        //if player steps on a mine, show color
        if (endgame)
            colorBox.SetColor((boardController.FindColor(a, b)));
        //if player wins, set the board to white
		if (wingame)
			colorBox.SetColor(0);
    }


    //******** TRIGGER METHODS ********

    //When player enters 
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            inTrigger = true;

            if (firstClick)
               flagged = boardController.IsFlagged(a, b);

                if (!steppedOn && firstClick && !endgame && !flagged &&!wingame)
                {
                    steppedOn = true;
                    colorBox.SetColor((boardController.FindColor(a, b)));           //if triggered, set color
                }
                else if (!firstClick)                                               //this initializes the board
                {
                    firstClick = true;
                    steppedOn = true;
                    boardController.Click(a, b);
                    colorBox.SetColor(10);
                }
        }
    }

    //When player exists
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == player.transform)
        {
            inTrigger = false;
        }
    }


    //********FLAG METHODS************

    //checks if there are changes made to its flag
    public void CheckFlag()
    {
        flagged = boardController.IsFlagged(a, b); 
    }

    // if flag is removed, set flagged to false
    public void RemoveFlag(int x, int y)
    {
        if (a == x && b == y)
        {
            flagged = false;
        }
    }


}   
