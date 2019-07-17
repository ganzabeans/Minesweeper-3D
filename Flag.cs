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