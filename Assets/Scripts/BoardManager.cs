using System.Collections.Generic;
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

    private MatchManager matchManager;

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
        InitializeBoard();
    }


    void InitializeBoard()
    {
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
                food.GetComponent<Foods>().SetIndicies(x, y);
                boardManager[x, y] = new Node(true, food);
            }
        }
        matchManager.CheckBoard();
    }


}

