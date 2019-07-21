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
    static int clickCounter = 1;                //to count how many squares have been clicked
    static bool boardSet;                   //check that board has been set up properly
    static bool firstClick;                 //initializes board
    static bool win;                        //if you win you get it!

    public GameObject boxObject;            //cell
    public GameObject flagPrefab;
    GameObject player;


    //FOR THE BOARD MAP 
    struct CellInfo
    {
        public bool hasMine,
                    isClicked,
                    isFlagged;

        public int nextToMine;
    }
  
    //9x9 Map of cells
    CellInfo[,] board = new CellInfo[9, 9];
    //9x9 Mesh Baord of cells
    public static GameObject[,] boardMesh = new GameObject[9, 9];



    //********* METHODS ***********

    private void Awake()
    {
        //Instantiate visible board
        for (int i = 0; i < 9; i++)
        {
            for (int k = 0; k < 9; k++)
                boardMesh[i, k] = Instantiate(boxObject, new Vector3(i * 2f, 0f, k * 2f), Quaternion.identity);
        }
    }


    //**** Cell Triggered ***********
    //This method is called when the cell is triggered
    public void CellTriggered(int x, int z)
    {
        if (board[x, z].isClicked)                      // if cell is already clicked, do nothing
            return;

        if (!board[x, z].isFlagged)                     // if cell is flagged, do nothing
        {   
            if (boardSet)
            {
                if (board[x, z].hasMine == true)        // if there is a mine, end game
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int k = 0; k < 9; k++)
                            boardMesh[i, k].SendMessage("EndGame");
                    }
                    return;
                }


                if (board[x, z].nextToMine != 0)       // if cell is next to a mine, activate 
                {
                    boardMesh[x, z].SendMessage("ChangeColor", board[x, z].nextToMine);
                    board[x, z].isClicked = true;          
                    clickCounter++;

                    if (clickCounter >= 71)                 // if all cells with no mines are explored, win game
                    {
                        Win();
                        return;
                    }

                    return;
                }

                if (board[x, z].nextToMine == 0)       // if cell is not next to a mine, open surrounding cells
                {
                    FloodFill(x, z);
                    return;
                }
            }
            if (!firstClick)                            // if the board is not set yet, set the board
            {
                firstClick = true;
                CreateMineFields(x, z);
                boardMesh[x, z].SendMessage("ChangeColor", board[x, z].nextToMine);
                board[x, z].isClicked = true;
                return;
            }
        }
    }


    //***** FLOOD FILL ********
    // compares surrounding array to make sure other squares are still in bounds
    // this method is called by a null square to open up surrounding squres that are
    //touching it (DFS)
    public void FloodFill(int x, int z)
    {
        if (board[x, z].isClicked)
        {
            return;
        }
        if (board[x, z].hasMine)    //shouldn't be true but had some random glitches
        {
            return;
        }
        if (!board[x,z].isFlagged)
        { 
            if (board[x, z].nextToMine > 0)
            {
                boardMesh[x, z].SendMessage("ChangeColor", board[x, z].nextToMine);
                board[x, z].isClicked = true;
                clickCounter++;

                if (clickCounter >= 71)                 // if all cells with no mines are explored, win game
                {
                    Win();
                    return;
                }

                return;
            }

            if (board[x, z].nextToMine == 0)
            {
                boardMesh[x, z].SendMessage("ChangeColor", board[x, z].nextToMine);
                board[x, z].isClicked = true;
                clickCounter++;

                if (clickCounter >= 71)                 // if all cells with no mines are explored, win game
                {
                    Win();
                    return;
                }

                if (x < bounds)
                    FloodFill(x + 1, z);
                if (x < bounds && z < bounds)
                    FloodFill(x + 1, z + 1);
                if (x < bounds && z > 0)
                    FloodFill(x + 1, z - 1);
                if (x > 0)
                    FloodFill(x - 1, z);
                if (x > 0 && z < bounds)
                    FloodFill(x - 1, z + 1);
                if (x > 0 && z > 0)
                    FloodFill(x - 1, z - 1);
                if (z < bounds)
                    FloodFill(x, z + 1);
                if (z > 0)
                    FloodFill(x, z - 1);
            }
        }
    }


    private void Win ()
    {
        if (clickCounter >= 71)                 // if all cells with no mines are explored, win game
        { 
            win = true;
            for (int i = 0; i < 9; i++)
            {
                for (int k = 0; k < 9; k++)
                    boardMesh[i, k].SendMessage("EndGame");
            }
        }
    }

    // ************ BOARD CREATION *****************

    // ***** CREATE MIND FIELD ********
    // builds the board, places mines randomly 
    // does not place mine on initial landing point

    public void CreateMineFields(int x, int z)
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

        AssignNum();
    }

    //***** ASSIGN MINE COUNT*******
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

        boardSet = true;

        return 1;
    }


    // ****** GET COLOR ***********
    // when the game ends, the board shows results if lost, all white if won
    public int GetColor(int x, int z)
    {
        board[x, z].isClicked = true;

        if (win)
            return 0;

        if (board[x, z].hasMine)
            return 9;
        else
            return board[x, z].nextToMine;

    }
    //****** FLAG FUNCTIONS *********

    //When user presses "F", a flag is created
    public void SetFlag(int x, int z)
    {
        player = GameObject.Find("Ellen");
        Vector3 vector3 = player.transform.rotation * Vector3.forward;      //determines direction player is facing and
                                                                                             //if there is an eligible place to flag

        int a, b;       //direction of look vector

        a = Mathf.RoundToInt(vector3.x) + x;        //coordinate of new flag
        b = Mathf.RoundToInt(vector3.z) + z;

        if ((a >= 0) && (a < arraySize) && (b >= 0) && (b < arraySize))
        {
            board[a, b].isFlagged = !board[a, b].isFlagged;
            if (board[a, b].isFlagged)
            {
                Instantiate(flagPrefab, new Vector3(a * 2 + 1, 2, b * 2 - 1), Quaternion.identity);
            }
        }
    }

    //When Flag is deleted by player, Board unflags cell
    public void RemoveFlag(int x, int z)
    {
        board[x, z].isFlagged = false;
    }

}
