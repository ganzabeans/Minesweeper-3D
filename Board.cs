using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    //Member vars
    int mines = 10;                         // number of mines
    int arraySize = 9;                      //gameboard size
    int bounds = 8;                         // boundary for array
    int[,] mineArray = new int[10, 2];      //array to store set mines
    int aMin, aPlus, bMin, bPlus;           //For boundary calculation
    static int clickCounter;                //to count how many squares have been clicked

    //BOARD CONTROLLER
    public GameObject boardControllerObject;
    private BoardController boardController;

    //FOR THE BOARD MAP 
    //Struct holds info about each cell
    struct CellInfo
    {
        public bool hasMine,
                    isClicked,
                    isFlagged;

        public int nextToMine;
    }
    //MAP IS AN ARRAY OF CELLINFO
    //9x9 board of cells
    CellInfo[,] board = new CellInfo[9, 9];



    //********* METHODS ***********

    private void Awkae()
    {
        boardController = boardControllerObject.GetComponent<BoardController>(); //reference board controller
    }

    // ***** CREATE MIND FIELD ********
    // builds the board, places mines randomly 
    // does not place mine on initial landing point

    public int CreateMineFields(int x, int z)
    {
        for (int i = 0; i < mines; i++)
        {
            int a, b;
            a = Random.Range(0, arraySize);
            b = Random.Range(0, arraySize);

            if (!board[a, b].hasMine && !(a == x && b == z))
            {
                board[a, b].hasMine = true;
                board[a, b].nextToMine = -10;    //to avoid issues with checking neighboring nulls
                
                mineArray[i, 0] = a;            //store mine locations in mine array
                mineArray[i, 1] = b;
            }
            else
                i--;        //in case there is a repeat it the randomizaiton 
        }

        board[x, z].isClicked = true;   //makes first square clicked
        clickCounter++;

        return 1;

    }

    //***** ASSIGN MINE COUNT NUM TO NEIGHBORS ********
    //counts number of mines each square is next to
    public int AssignNum()
    {
        int a, b;

        for (int i = 0; i < mines; i++)
        {
            a = mineArray[i, 0];
            b = mineArray[i, 1];

            //check all possibilities at the left bound
            if (a < bounds)
            {
                board[a + 1, b].nextToMine++;
                if (b < bounds)
                    board[a + 1, b + 1].nextToMine++;
                if (b > 0)
                    board[a + 1, b - 1].nextToMine++;
            }

            //check all possibiliteis at right boutd
            if (a > 0)
            {
                board[a - 1, b].nextToMine++;
                if (b < bounds)
                    board[a - 1, b + 1].nextToMine++;
                if (b > 0)
                    board[a - 1, b - 1].nextToMine++;
            }
            // check middle bound;
            if (b > 0)
                board[a, b - 1].nextToMine++;
            if (b < bounds)
                board[a, b + 1].nextToMine++;
        }

        return 1;
    }

    //**** CHECKS VAR STORED ***********
    //This method sees what is stored in the cell the player is on
    public int CheckNum(int a, int b)
    {
        if (board[a, b].isClicked)                  //check if square hasn't been stepped on already
            return 10;
        else                                        //if the square is not run over
        {
            board[a, b].isClicked = true;           //set clicked to true
            clickCounter++;

            if (clickCounter == 71 && !Cell.endgame)//win game
                return 11;

            if (board[a, b].hasMine == true)        //check if there is a mine
                return 9;                           //sad :'(

            else if (board[a, b].nextToMine != 0)   //check if it is next to a mine
                return board[a, b].nextToMine;
                                            
            else                                    //default 
                return 0;
        }
    }


    //***** CHECK IF NEIGHBOR SQUARES ARE NOT MINES ********
    // compares surrounding array to make sure other squares are still in bounds
    // this method is called by a null square to open up surrounding squres that are
    //touching it
    public int NullAround(int a, int b)
    {
        aMin = a - 1;
        aPlus = a + 1;
        bMin = b - 1;
        bPlus = b + 1;

        if (board[a, b].hasMine)        //if starting square has a mine, don't open it!
            return 10;

        if (a < bounds)
        {
            if (board[aPlus, b].isClicked && board[aPlus, b].nextToMine == 0 && !board[aPlus, b].hasMine)
            {
                board[a, b].isClicked = true;
                clickCounter++;
                return board[a,b].nextToMine;
            }
            if (b < bounds)
                if (board[aPlus, bPlus].isClicked && board[aPlus, bPlus].nextToMine == 0 && !board[aPlus, bPlus].hasMine)
                {
                    board[a, b].isClicked = true;
                    clickCounter++;
                    return board[a, b].nextToMine;
                }
            if (b > 0)
                if (board[aPlus, bMin].isClicked && board[aPlus, bMin].nextToMine == 0 && !board[aPlus, bMin].hasMine)
                {
                    board[a, b].isClicked = true;
                    clickCounter++;
                    return board[a, b].nextToMine;
                }
        }
        if (a > 0)
        {
            if (board[aMin, b].isClicked && board[aMin, b].nextToMine == 0 && !board[aMin, b].hasMine)
            {
                board[a, b].isClicked = true;
                clickCounter++;
                return board[a, b].nextToMine;
            }
            if (b < bounds)
                if (board[aMin, bPlus].isClicked && board[aMin, bPlus].nextToMine == 0 && !board[aMin, bPlus].hasMine)
                {
                    board[a, b].isClicked = true;
                    clickCounter++;
                    return board[a, b].nextToMine;
                }
            if (b > 0)
                if (board[aMin, bMin].isClicked && board[aMin, bMin].nextToMine == 0 && !board[aMin, bMin].hasMine)
                {
                    board[a, b].isClicked = true;
                    clickCounter++;
                    return board[a, b].nextToMine;
                }
        }
        if (b > 0)
        {
            if (board[a, bMin].isClicked && board[a, bMin].nextToMine == 0 && !board[a, bMin].hasMine)
            {
                board[a, b].isClicked = true;
                clickCounter++;
                return board[a, b].nextToMine;
            }
        }
        if (b < bounds)
        {
            if (board[a, bPlus].isClicked && board[a, bPlus].nextToMine == 0 && !board[a, bPlus].hasMine)
            {
                board[a, b].isClicked = true;
                clickCounter++;
                return board[a, b].nextToMine;
            }
        }

        return 10;
    }


    //****** FLAG FUNCTIONS *********

    //Set Flag Method
    public Vector3 SetFlag(int a, int b, Vector3 vector3)
    {
        int x, z;
        aMin = a - 1;
        aPlus = a + 1;
        bMin = b - 1;
        bPlus = b + 1;

        x = Mathf.RoundToInt(vector3.x);
        z = Mathf.RoundToInt(vector3.z);

        //assign flags based on vector

        //following if statements check the bounds to see if still in array 
        if (x == 1 && a < bounds)
        {
            if (z == 0)
            {
                board[aPlus, b].isFlagged = !board[aPlus, b].isFlagged;
                if (board[aPlus, b].isFlagged)
                    return (new Vector3(aPlus * 2 +1, 2, b * 2 - 1));
                else
                    return Vector3.zero;
            }
            if (z == 1 && b < bounds)
            {
                board[aPlus, bPlus].isFlagged = !board[aPlus, bPlus].isFlagged;
                if(board[aPlus,bPlus].isFlagged)
                    return (new Vector3(aPlus * 2 + 1, 2, bPlus * 2 - 1));
                else
                    return Vector3.zero;
            }
            if (z == -1 && b > 0)
            {
                board[aPlus, bMin].isFlagged = !board[aPlus, bMin].isFlagged;
                if (board[aPlus, bMin].isFlagged)
                    return (new Vector3(aPlus * 2 + 1, 2, bMin * 2 - 1));
                else
                    return Vector3.zero;
            }
        }
        else if (x == -1 && a > 0)
        {
            if (z == 0)
            {
                board[aMin, b].isFlagged = !board[aMin, b].isFlagged;
                if (board[aMin, b].isFlagged)
                    return (new Vector3(aMin * 2 + 1, 2, b * 2 - 1));
                else
                    return Vector3.zero;
            }
            if (z == 1 && b < bounds)
            {
                board[aMin, bPlus].isFlagged = !board[aMin, bPlus].isFlagged;
                if (board[aMin, bPlus].isFlagged)
                    return (new Vector3(aMin * 2 + 1, 2, bMin * 2 - 1));
                else
                    return Vector3.zero;
            }
            if (z == -1 && b > 0)
            {
                board[aMin, bMin].isFlagged = !board[aMin, bMin].isFlagged;
                if (board[aMin, bMin].isFlagged)
                    return (new Vector3(aMin * 2 + 1, 2, bMin * 2 - 1));
                else
                    return Vector3.zero;
            }
        }
        else if (x == 0 && z == -1 && b > 0)
        {
            board[a, bMin].isFlagged = !board[a, bMin].isFlagged;
            if (board[a, bMin].isFlagged)
                return (new Vector3(a * 2 + 1, 2, bMin * 2 - 1));
            else
                return Vector3.zero;
        }
        else if (x == 0 && z == 1 && b < bounds)
        {
            board[a, bPlus].isFlagged = !board[a, bPlus].isFlagged;
            if (board[a, bPlus].isFlagged)
                return (new Vector3(a * 2 + 1, 2, bPlus * 2 - 1));
            else
                return Vector3.zero;
        }
        return Vector3.zero;
    }


    //Check if flagged
    public bool SeeIfFlagged(int a, int b)
    {
        return board[a, b].isFlagged;
    }

    // removes flags when deleted
    public void RemoveFlag(int a, int b)
    {
        board[a, b+1].isFlagged = false;
        boardController.CheckFlagToCell();  //asks cell to see if its flag is deleted 
    }

   
}
