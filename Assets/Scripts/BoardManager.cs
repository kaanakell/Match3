using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    //define size of the board
    public int boardWidth = 7;
    public int boardHeight = 7;
    //define some spacing for board
    public float spacingX;
    public float spacingY;
    //get a reference to our prefabs
    public GameObject[] foodPrefabs;
    //get a reference to the collection nodes
    public Node[,] boardManager;
    public GameObject boardManagerGameObject;

    public List<GameObject> foodsToDestroy = new();
    public GameObject foodParent;

    private MatchManager matchManager;
    private SpinManager spinManager;


    //layoutAray
    //public ArrayLayout arrayLayout;
    //public static of board
    public static BoardManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchManager = GameObject.Find("MatchManager").GetComponent<MatchManager>();
        spinManager = GameObject.Find("SpinManager").GetComponent<SpinManager>();

        InitializeBoard();
        spinManager.InitializeColumns();
        spinManager.Spin();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Foods>())
            {
                if (matchManager.isProcessingMove)
                {
                    return;
                }

                Foods food = hit.collider.gameObject.GetComponent<Foods>();
                Debug.Log("I have a clicked a food it is: " + food.gameObject);

                matchManager.SelectFood(food);
            }
        }
    }


    void InitializeBoard()
    {
        DestroyFoods();
        boardManager = new Node[boardWidth, boardHeight];

        spacingX = (boardWidth - 1) / 2;
        spacingY = (boardHeight - 1) / 2;

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                int randomIndex = Random.Range(0, foodPrefabs.Length);

                GameObject food = Instantiate(foodPrefabs[randomIndex], position, Quaternion.identity);
                food.transform.SetParent(foodParent.transform);
                food.GetComponent<Foods>().SetIndicies(x, y);
                boardManager[x, y] = new Node(true, food);
                foodsToDestroy.Add(food);
            }
        }
        if (matchManager.CheckBoard())
        {
            Debug.Log("We have matches let's re-create the board");
            InitializeBoard();
        }
        else
        {
            Debug.Log("There are no matches, we can start the game");
        }
    }

    public void RemoveAndRefill(List<Foods> _foodsToRemove)
    {
        //Removing the food and clearing the board at that location
        foreach (Foods food in _foodsToRemove)
        {
            //getting it's x and y indicies and storing them
            int xIndex = food.xIndex;
            int yIndex = food.yIndex;

            //Destroy the food
            Destroy(food.gameObject);

            //Create a blank node on the food board.
            boardManager[xIndex, yIndex] = new Node(true, null);
        }

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (boardManager[x, y].food == null)
                {
                    Debug.Log("The location X: " + x + "Y:" + y + "is empty, attemting to refill it.");
                    RefillFoods(x, y);
                }
            }
        }
    }

    #region Cascading Foods

    private void RefillFoods(int x, int y)
    {
        //y offset
        int yOffset = 1;

        //while the cell above our current cell is null and we're below the height of the board
        while (y + yOffset < boardHeight && boardManager[x, y + yOffset].food == null)
        {
            //increment y offset
            Debug.Log("The food above me is null, but i'm not at the top of the board yet, so add to my yOffset and try again. Current Offset is: " + yOffset + "I'm about to add 1.");
            yOffset++;
        }

        //we've either hit the top of the board or we found a food
        if (y + yOffset < boardHeight && boardManager[x, y + yOffset].food != null)
        {
            //we've found a food
            Foods foodAbove = boardManager[x, y + yOffset].food.GetComponent<Foods>();

            //Move it to the correct location
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, foodAbove.transform.position.z);
            Debug.Log("I've found a food when refilling the board and it was in the location: [" + x + "," + (y + yOffset) + "] we have moved it to the location: [" + x + "," + y + "]");
            //Move to location
            foodAbove.MoveToTarget(targetPos);
            //update incidices
            foodAbove.SetIndicies(x, y);
            //update our board
            boardManager[x, y] = boardManager[x, y + yOffset];
            //set the location the position came from to null
            boardManager[x, y + yOffset] = new Node(true, null);
        }

        //if we've hit the top of the board without finding a food
        if (y + yOffset == boardHeight)
        {
            Debug.Log("I've reached the top of the board without finding a food");
            SpawnFoodAtTop(x);
        }

    }

    private void SpawnFoodAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMoveTo = 7 - index;
        Debug.Log("About to spawn a food, ideally i'd like to put it in the index of: "+ index);
        //get a random food
        int randomIndex = Random.Range(0, foodPrefabs.Length);
        GameObject newFood = Instantiate(foodPrefabs[randomIndex], new Vector2(x - spacingX, boardHeight - spacingY), Quaternion.identity);
        newFood.transform.SetParent(foodParent.transform);
        //set indicies
        newFood.GetComponent<Foods>().SetIndicies(x, index);
        //set it on the board
        boardManager[x, index] = new Node(true, newFood);
        //move it to that location
        Vector3 targetPosition = new Vector3(newFood.transform.position.x, newFood.transform.position.y - locationToMoveTo, newFood.transform.position.z);
        newFood.GetComponent<Foods>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for(int y = 6; y >= 0; y--)
        {
            if(boardManager[x, y].food == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }


    #endregion

    void DestroyFoods()
    {
        if (foodsToDestroy != null)
        {
            foreach (GameObject food in foodsToDestroy)
            {
                Destroy(food);
            }
            foodsToDestroy.Clear();
        }
    }
}

