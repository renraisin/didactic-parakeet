using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //sets up reference to BoardManager to set up the level
    private BoardManager boardScript;

    //access from any script
    public static GameManager instance = null;

    //variable for the current elvel number, expressed in game as "Day X"
    private int level = 3; //3 is where enemies appear

    //call this before any Start functions
    private void Awake()
    {
      if (instance == null)
        {
            instance = this;
        } 
      else if (instance != this)
        {
            Destroy(gameObject);
        }
      //prevents this from being destroyed at every new scene reload
        DontDestroyOnLoad(gameObject);

        //reference the BoardManager script
        boardScript = GetComponent<BoardManager>();

        //initialize first level
        InitGame();
    }

    void InitGame() //initializes each level
    {
        //call setupscene manager of BoardManager script and give it current level number
        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
