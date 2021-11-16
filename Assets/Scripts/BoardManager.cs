using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count ( int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //make size of game board
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9); //sets number of destructible walls
    public Count foodCount = new Count(1, 5); //sets number of food items
    public GameObject exit; //the only exit on the levels

    //arrays to pass in multiple objects and choose one to spawn among variations. filled w prefabs
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; //keeps hierarchy clean for a parent object
    private List<Vector3> gridPositions = new List<Vector3>();
    //tracks all possible positions on game board and keep track of spawned objects

    void InitializeList() //
    {
        gridPositions.Clear(); //clears list of grid positions

        //fill list with each position on game board as a vector 3, beginning w X axis. -1 for border to prevent impossible level
        for (int x = 1; x < columns-1; x++)
        {
            for (int y = 1; y < rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup ()
    {
        //sets up outer wall and floor of game board
        boardHolder = new GameObject("Board").transform;

        //loop pattern to lay out floor and outer wall tiles. -1 to build an edge around active portion of game board
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //choose random floor tile from floor tile array to instantiate it
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //sets this equal to an index for a random chosen floor tile w/o pre specifying length

                //check to see if we are in an outer wall tile and choose an outer wall tile to instantiate
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //quaternion prevents rotating

                    instance.transform.SetParent(boardHolder);
                }
            }
        }
    }

    //returns Vector3
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count); //chooses btwn positions in grid positions list
        Vector3 randomPosition = gridPositions[randomIndex]; //chooses grid position 

        // don't let 2 objects store at the same location
        gridPositions.RemoveAt(randomIndex);

        //return value of random position to spawn object in a random location
        return randomPosition;
    }


    //spawn tiles at chosen positions
    void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1); //controls how many objects spawn eg walls in level
        for (int i = 0; i < objectCount; i++) //spawn num of objects specified by object count
        {
            //choose random positions
            Vector3 randomPosition = RandomPosition();
            //choose random tile from array of game objects to spawn
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);

        }
    }

    public void SetupScene(int level)
    {
        //single public function in this class, called by game manager
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        //generate enemies based on level number
        int enemyCount = (int)Mathf.Log(level,2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); //min and max numbers same bc not a random range
        //exit will always be in the same place in upper right even if board is resized
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);


    }
}
