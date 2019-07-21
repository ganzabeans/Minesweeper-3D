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
