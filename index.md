## About This Project

This project was created with Assets from Unity 3D Game Kit Lite. I created additional scripts and meshes to make the game functional.

Rather than a clickable 2D surface such as in the original Minesweeper, this 3D version allows the user to run and jump across an active mine field via a human character. Just like in a real mine field, when the player lands on a cell, the cell is triggered. The player can also flag mine fields to deactivate them. The game is won when all cells without mines have been triggered and conversely is lost when a mine has been triggered.


Here are the Scripts that were added:

- Board Controller
- Board
- Cell
- Flag
- Box Render


### Board Controller

The `BoardController` is the Control between the graphical interface and the actual calculations of the game. It manages the `Board` (game calculations), the `Cell`s (graphic cells that player interacts with), the `Flag`s (graphic dots), as well as the Player itself.

```markdown

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
    public static int boardSet; //Confirm board is set before Cell Update calls

  
    //MONOBEHAVIOR FUNCTIONS

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

```

### Board

The `Board` runs the calculations and holds a non-graphical map of the game. It uses an array of structs to hold information about each cell. The `BoardController` notifies the board on where the player is in the graphical world. 

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

```

### Cell

The `Cell` works with the mesh boxes that the player interacts with. It is triggered when the player lands on top of it; the cell then notifies the `BoardController` which returns information to the cell about what color to render to (if any).

```markdown
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


    //******** METHODS ***************

    // MONOBEHAVIOR METHODS 

    //***** START *******
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
            if (colorNum != 10)
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
                    colorBox.SetColor(11);
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

```

### Flag

The `Flag`controls the mesh flag spheres that the player and mesh board interacts with. When triggered by the player, it checks for a mouse click to see if the player wants to delete it. 

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
				Destroy(gameObject);
				cell.RemoveFlag((int)(this.transform.position.x / 2), (int)(this.transform.position.z / 2)); 
                board.RemoveFlag((int)(this.transform.position.x/2), (int)(this.transform.position.z/2));       
            }
        }
                       
        if (!m_foundStaff && Input.GetMouseButtonDown(0))       //staff is only enabled when clicked, old verison of unity couldn't find 
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
        switch (num)
        {
            case 0:
                rend.material.shader = Shader.Find("_Color");               // 0 is white
                rend.material.SetColor("_Color", Color.white);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.white);
                break;
            case 1:
                rend.material.shader = Shader.Find("_Color");               // 1 is blue
                rend.material.SetColor("_Color", Color.blue);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.blue);
                break;
            case 2:                                                         // 2 is green
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.green);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.green);
                break;
            case 3:
                rend.material.shader = Shader.Find("_Color");               // 3 is red
                rend.material.SetColor("_Color", Color.red);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.red);
                break;
            case 4:
                rend.material.shader = Shader.Find("_Color");               // 4 is purple
                rend.material.SetColor("_Color", Color.HSVToRGB(267, 74f, 59f)); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(267, 74f, 59f));
                break;
            case 5:
                rend.material.shader = Shader.Find("_Color");               // 5 is maroon
                rend.material.SetColor("_Color", Color.HSVToRGB(338f, 100f, 27f)); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(338f, 100f, 27f));
                break;
            case 6:
                rend.material.shader = Shader.Find("_Color");               // 6 is turquoise 
                rend.material.SetColor("_Color", Color.cyan);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.cyan);
                break;
            case 7:
                rend.material.shader = Shader.Find("_Color");               // 7 is black 
                rend.material.SetColor("_Color", Color.black);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.black);
                break;
            case 8:                                                         // 8 is grey
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.grey);
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.grey);
                break;
            case 9: //end game                                              //mines are dark read but look black 
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.HSVToRGB(10f, 100f, 54f));   
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.HSVToRGB(10f, 100f, 54f));
                break;
            case 10:
                //do nothing
                break;
            case 11: //start game                                           // start block is yellow
                rend.material.shader = Shader.Find("_Color");
                rend.material.SetColor("_Color", Color.yellow); 
                rend.material.shader = Shader.Find("Specular");
                rend.material.SetColor("_SpecColor", Color.yellow);
                break;

        }
    }
}

```

_All this code was written by Anisha Braganza but is free to be used by for anyone :) _
- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)

