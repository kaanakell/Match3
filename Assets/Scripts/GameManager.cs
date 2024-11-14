using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //static reference

    public GameObject backgroundPanel; //grey background
    public GameObject victoryPanel;
    public GameObject losePanel;

    public int goal; //the amount of points you need to get to to win
    public int moves; //the number of turns you can take
    public int points; //the current points you have earned

    public bool isGameEnded;

    public TextMeshPro pointsTxt;
    public TextMeshPro movesTxt;
    public TextMeshPro goalTxt;


    private void Awake()
    {
        Instance = this;
    }

    public void Initialize (int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    // Update is called once per frame
    void Update()
    {
        pointsTxt.text = "Points: " + points.ToString();
        movesTxt.text = "Moves: " + moves.ToString();
        goalTxt.text = "Goal: " + goal.ToString();

    }

    public void ProcessTurn(int pointsToGain, bool substractMoves)
    {
        points += pointsToGain;
        if(substractMoves)
        {
            moves--;
        }

        if(points >= goal)
        {
            //you've won the game
            isGameEnded = true;
            //Display a victory screen
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            return;
        }
        if(moves == 0)
        {
            //lose the game
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            return;
        }
    }

    //attached to a button to change scene when winning
    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    //attached to a button to change scene when losing
    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }
}
