## About This Project

This project was created with Assets from Unity 3D Game Kit Lite. I created additional scripts and meshes to make the game functional.

![Image](https://raw.githubusercontent.com/ganzabeans/Minesweeper-3D/master/Screen%20Shot%202019-07-17%20at%201.06.21%20PM.png)
_For examples of gameplay_ [click here](https://braganza5.wixsite.com/anisha-braganza/copy-of-minesweeper-3d)

Rather than a clickable 2D surface such as in the original Minesweeper, this 3D version allows the user to run and jump across an active mine field via a human character. Just like in a real mine field, when the player lands on a cell, the cell is triggered. The player can also flag mine fields to deactivate them. The game is won when all cells without mines have been triggered and conversely is lost when a mine has been triggered.


Here are the Scripts that were added:

- Board
- Cell
- Flag
- Box Render


### Board

```markdown
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
        Debug.Log(clickCounter);

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

```

### Cell

The `Cell` notifies the `Board` when the player lands in its trigger. It also recieves instructions from the `Board` of which color to switch to. 

```markdown
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell : MonoBehaviour
{

    //GAME OBJECTS
    GameObject player;
    public GameObject boardObject;
    public GameObject cube;
    public GameObject boxRender;

    //SCRIPTS
    static private Board board;
    private BoxRender colorBox;

   //BOOLS
    public bool inTrigger;

    //INTEGERS
    int x, z; //coordinate location


    //******** METHODS *******************

    void Awake ()                       
    { 
        player = GameObject.Find("Ellen");
		
        x = ((int)cube.transform.position.x / 2);               //position in array
        z = ((int)cube.transform.position.z / 2);               //position in array

        colorBox = boxRender.GetComponent<BoxRender>();         //reference of Box Render and Board
        board = boardObject.GetComponent<Board>();
    }

    //******** TRIGGER METHODS ********

    //When player enters 
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == player.transform)
        {
            inTrigger = true;
            board.CellTriggered(x, z);
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

    // when board tells cell to change color, cell lets box render know which color to change to
    public void ChangeColor (int colorCode)
    { 
        colorBox.SetColor(colorCode);
    }

    // when game ends, turn on color
    public void EndGame ()
    {
        colorBox.SetColor(board.GetColor(x, z));
    }

    //checking if player wants to flag a square
    private void Update()
    {
        if (inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F))
                board.SetFlag(x, z);
        }
    }
}   

```

### Flag

The `Flag` manages the mesh flags created by the player. If the player removes a flag, it deletes the game object and notifies the `Board`.

```markdown
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    //GAME OBJECTS
    public GameObject cube;
    public GameObject boardObj;
    GameObject player;
    GameObject staff;

    //SCRIPTS
    private Board board;
    private Cell cell;

    //BOOLS
    bool m_inTrigger;
    bool m_foundStaff;


    //***********  METHODS ****************


    private void Awake()
    {
        player = GameObject.Find("Ellen");
        cell = cube.GetComponent<Cell>();
        board = boardObj.GetComponent<Board>();
	}


    //Destroys flag when game is over 
	public void EndGame()
	{
		Destroy(gameObject);           
	}


    //checks if character is on flag
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == player || other.gameObject == staff)
        {
            m_inTrigger = true;
        }
    }
    //checks if character leaves flag
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player || other.gameObject == staff)
        {
            m_inTrigger = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))        //to delete when clicked
        {
            if (m_inTrigger == true)
            {
                board.RemoveFlag((int)(this.transform.position.x - 1)/2,(int)(this.transform.position.z + 1)/2);
                Destroy(gameObject);
            }
        }
                       
        if (!m_foundStaff && Input.GetMouseButtonDown(0))       //staff is only enabled when clicked, old verison of unity couldn't find it on the prefab for somereason... 
        {                                                       //the assets so I just went with a tag when its available 
            staff = GameObject.FindGameObjectWithTag("Staff");
            if (staff)
                m_foundStaff = true;
        }
    }
}

```

### Box Render

The `BoxRender`controls the render settings of the mesh cell. It directly communicates with `Cell` to deterimne what color to render to.

```markdown

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRender : MonoBehaviour
{
    Renderer rend;

    //gameobject position
    void Awake()
    {
        rend = this.GetComponent<Renderer>();

        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", Color.grey);

        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.grey);
    }

    //  This function changes the cube color
    public void SetColor(int num)
    {
        Color[] colors = new Color[11];
        colors[0] = Color.white;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = Color.red;
        colors[4] = Color.HSVToRGB(267, 74f, 59f);      //purple
        colors[5] = Color.HSVToRGB(338f, 100f, 27f); //maroon
        colors[6] = Color.cyan;
        colors[7] = Color.black;
        colors[8] = Color.grey;
        colors[9] = Color.HSVToRGB(10f, 100f, 54f); //dark red
        colors[10] = Color.yellow;

        if ((rend != null) && (num <= 10))
        {
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", colors[num]);
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", colors[num]);
        }
    }
}
 
```

_These scripts are written by Anisha Braganza but are free to be used by anyone :)_
