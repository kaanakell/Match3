using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public List<Foods> connectedFoods;
    public MatchDirection direction;

    private BoardManager boardManager;

    void Start()
    {
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    public bool CheckBoard()
    {
        Debug.Log("Checking");
        bool hasMatched = false;
        List<Foods> foodsToRemove = new();

        for (int x = 0; x < boardManager.boardWidth; x++)
        {
            for (int y = 0; y < boardManager.boardHeight; y++)
            {
                if (boardManager.boardManager[x, y].isUsable)
                {
                    Foods food = boardManager.boardManager[x, y].food.GetComponent<Foods>();

                    if (!food.isMatched)
                    {
                        MatchManager matchedFoods = IsConnected(food);

                        if(matchedFoods.connectedFoods.Count >= 3)
                        {
                            //complex matching...

                            foodsToRemove.AddRange(matchedFoods.connectedFoods);

                            foreach(Foods foods in matchedFoods.connectedFoods)
                            {
                                foods.isMatched = true;
                            }
                            hasMatched = true;
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    MatchManager IsConnected(Foods foods)
    {
        List<Foods> connectedFoods = new();
        FoodType foodType = foods.foodType;

        connectedFoods.Add(foods);

        //right
        CheckDirection(foods, new Vector2Int(1, 0), connectedFoods);
        //left
        CheckDirection(foods, new Vector2Int(-1, 0), connectedFoods);

        //3match?(horizontal)
        if(connectedFoods.Count == 3)
        {
            Debug.Log("we have a normal horizontal match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Horizontal,
            };
        }
        //more than 3?(long horizontal)
        else if(connectedFoods.Count > 3)
        {
            Debug.Log("we have a long horizontal match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.LongHorizontal,
            };
        }
        //clear out the connected foods
        connectedFoods.Clear();
        // read our initail pos
        connectedFoods.Add(foods);
        //check up
        CheckDirection(foods, new Vector2Int(0, 1), connectedFoods);

        //check down
        CheckDirection(foods, new Vector2Int(0, -1), connectedFoods);

        //3match?(vertical)
        if(connectedFoods.Count == 3)
        {
            Debug.Log("we have a normal vertical match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Vertical,
            };
        }
        //more than 3?(long vertical)
        else if(connectedFoods.Count > 3)
        {
            Debug.Log("we have a long vertical match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.LongVertical,
            };
        }
        else
        {
            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.None,
            };
        }
    }

    void CheckDirection(Foods food, Vector2Int direction, List<Foods> connectedFoods)
    {
        FoodType foodType= food.foodType;
        int x = food.xIndex + direction.x;
        int y = food.yIndex + direction.y;

        //check we are in boundries
        while(x >= 0 && x < boardManager.boardWidth && y >= 0 && y < boardManager.boardHeight)
        {
            if(boardManager.boardManager[x, y].isUsable)
            {
                Foods neighbourFood = boardManager.boardManager[x, y].food.GetComponent<Foods>();

                //does our positionType Match?
                if(!neighbourFood.isMatched && neighbourFood.foodType == foodType)
                {
                    connectedFoods.Add(neighbourFood);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
}


public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}
