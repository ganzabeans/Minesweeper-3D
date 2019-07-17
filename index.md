## About this Project

This project was created with Assets from Unity 3D Game Kit Lite. I created additional scripts and the game board to run the game.

Here are the Scripts that were added:

- Board Controller
- Board
- Cell
- Box Render
- Flag


BoardController

Markdown is a lightweight and easy-to-use syntax for styling your writing. It includes conventions for

```markdown

using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;***

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


- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/ganzabeans/Minesweeper-3D/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://help.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.
