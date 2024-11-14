using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public List<Foods> connectedFoods;
    public MatchDirection direction;

    private BoardManager boardManager;

    [SerializeField] private Foods selectedFood;

    public bool isProcessingMove;
 
    [SerializeField] List<Foods> foodsToRemove = new();
    void Start()
    {
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }

    public bool CheckBoard()
    {
        if(GameManager.Instance.isGameEnded)
        {
            return false;
        }
        Debug.Log("Checking");
        bool hasMatched = false;

        foodsToRemove.Clear();

        foreach (Node nodeFood in boardManager.boardManager)
        {
            if (nodeFood.food != null)
            {
                nodeFood.food.GetComponent<Foods>().isMatched = false;
            }
        }

        for (int x = 0; x < boardManager.boardWidth; x++)
        {
            for (int y = 0; y < boardManager.boardHeight; y++)
            {
                //checking if food node is usable
                if (boardManager.boardManager[x, y].isUsable)
                {
                    //then proceed to get food class in node
                    Foods food = boardManager.boardManager[x, y].food.GetComponent<Foods>();

                    //ensure it's not matched
                    if (!food.isMatched)
                    {
                        //run some matching logic
                        MatchManager matchedFoods = IsConnected(food);

                        if (matchedFoods.connectedFoods.Count >= 3)
                        {
                            //complex matching...
                            MatchManager superMatchedFoods = SuperMatch(matchedFoods);

                            foodsToRemove.AddRange(superMatchedFoods.connectedFoods);

                            foreach (Foods foods in superMatchedFoods.connectedFoods)
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

    public IEnumerator ProcessTurnOnMatchedBoard(bool substractMoves)
    {
        foreach (Foods foodToRemove in foodsToRemove)
        {
            foodToRemove.isMatched = false;
        }

        boardManager.RemoveAndRefill(foodsToRemove);
        GameManager.Instance.ProcessTurn(foodsToRemove.Count, substractMoves);
        yield return new WaitForSeconds(.4f);

        if(CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
    }

    private MatchManager SuperMatch(MatchManager matchedResults)
    {
        //if we have horizontal or long horizontal match
        if (matchedResults.direction == MatchDirection.Horizontal || matchedResults.direction == MatchDirection.LongHorizontal)
        {
            //for each food
            foreach (Foods food in matchedResults.connectedFoods)
            {
                List<Foods> extraConnectedFoods = new();
                //check up
                CheckDirection(food, new Vector2Int(0, 1), extraConnectedFoods);
                //check down
                CheckDirection(food, new Vector2Int(0, -1), extraConnectedFoods);
                //do we have 2 or more foods that have been matched against this current food.
                if (extraConnectedFoods.Count >= 2)
                {
                    Debug.Log("I have a super Horizontal match");
                    extraConnectedFoods.AddRange(matchedResults.connectedFoods);
                    //return our super match
                    return new MatchManager
                    {
                        connectedFoods = extraConnectedFoods,
                        direction = MatchDirection.Super
                    };
                }
            }
            //we didn't have a super match, so return our normal match
            return new MatchManager
            {
                connectedFoods = matchedResults.connectedFoods,
                direction = matchedResults.direction
            };
        }
        //if we have vertical or long vertical match
        else if (matchedResults.direction == MatchDirection.Vertical || matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Foods food in matchedResults.connectedFoods)
            {
                List<Foods> extraConnectedFoods = new();
                //check up
                CheckDirection(food, new Vector2Int(1, 0), extraConnectedFoods);
                //check down
                CheckDirection(food, new Vector2Int(-1, 0), extraConnectedFoods);
                //do we have 2 or more foods that have been matched against this current food.
                if (extraConnectedFoods.Count >= 2)
                {
                    Debug.Log("I have a super vertical match");
                    extraConnectedFoods.AddRange(matchedResults.connectedFoods);
                    //return our super match
                    return new MatchManager
                    {
                        connectedFoods = extraConnectedFoods,
                        direction = MatchDirection.Super
                    };
                }
            }
            //we didn't have a super match, so return our normal match
            return new MatchManager
            {
                connectedFoods = matchedResults.connectedFoods,
                direction = matchedResults.direction
            };
        }

        return null;
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
        if (connectedFoods.Count == 3)
        {
            Debug.Log("we have a normal horizontal match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Horizontal,
            };
        }
        //more than 3?(long horizontal)
        else if (connectedFoods.Count > 3)
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
        if (connectedFoods.Count == 3)
        {
            Debug.Log("we have a normal vertical match, the type of my match:" + connectedFoods[0].foodType);

            return new MatchManager
            {
                connectedFoods = connectedFoods,
                direction = MatchDirection.Vertical,
            };
        }
        //more than 3?(long vertical)
        else if (connectedFoods.Count > 3)
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
        FoodType foodType = food.foodType;
        int x = food.xIndex + direction.x;
        int y = food.yIndex + direction.y;

        //check we are in boundries
        while (x >= 0 && x < boardManager.boardWidth && y >= 0 && y < boardManager.boardHeight)
        {
            if (boardManager.boardManager[x, y].isUsable)
            {
                Foods neighbourFood = boardManager.boardManager[x, y].food.GetComponent<Foods>();

                //does our positionType Match?
                if (!neighbourFood.isMatched && neighbourFood.foodType == foodType)
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

    #region Swapping

    //select foods
    public void SelectFood(Foods _food)
    {
        //if we don't have a food currently selected, then set the food i just clicked to my selectedfood
        if (selectedFood == null)
        {
            Debug.Log(_food);
            selectedFood = _food;
        }
        //if we select the same food twice, then let's make selectedfood null
        else if (selectedFood == _food)
        {
            selectedFood = null;
        }
        //if selectedfood is not null and is not the current food, attempt a swap
        //selectedfood back to null
        else if (selectedFood != _food)
        {
            SwapFood(selectedFood, _food);
            selectedFood = null;
        }
    }

    //swap -logic
    private void SwapFood(Foods currentFood, Foods targetFood)
    {
        if (!IsAdjacent(currentFood, targetFood))
        {
            return;
        }

        DoSwap(currentFood, targetFood);

        isProcessingMove = true;

        StartCoroutine(ProcessMatches(currentFood, targetFood));
    }

    //do swap
    private void DoSwap(Foods currentFood, Foods targetFood)
    {
        GameObject temp = boardManager.boardManager[currentFood.xIndex, currentFood.yIndex].food;

        boardManager.boardManager[currentFood.xIndex, currentFood.yIndex].food = boardManager.boardManager[targetFood.xIndex, targetFood.yIndex].food;
        boardManager.boardManager[targetFood.xIndex, targetFood.yIndex].food = temp;

        //update indicies.
        int tempXIndex = currentFood.xIndex;
        int tempYIndex = currentFood.yIndex;

        currentFood.xIndex = targetFood.xIndex;
        currentFood.yIndex = targetFood.yIndex;

        targetFood.xIndex = tempXIndex;
        targetFood.yIndex = tempYIndex;

        currentFood.MoveToTarget(boardManager.boardManager[targetFood.xIndex, targetFood.yIndex].food.transform.position);
        targetFood.MoveToTarget(boardManager.boardManager[currentFood.xIndex, currentFood.yIndex].food.transform.position);
    }

    private IEnumerator ProcessMatches(Foods currentFood, Foods targetFood)
    {
        yield return new WaitForSeconds(.2f);

        if (CheckBoard())
        {
            //Start a coroutine that is going to process our matches in our turn
            StartCoroutine(ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            DoSwap(currentFood, targetFood);
        }

        isProcessingMove = false;
    }

    //IsAdjacent
    private bool IsAdjacent(Foods currentFood, Foods targetFood)
    {
        return Mathf.Abs(currentFood.xIndex - targetFood.xIndex) + Mathf.Abs(currentFood.yIndex - targetFood.yIndex) == 1;
    }

    #endregion
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