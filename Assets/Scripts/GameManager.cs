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
    public SpinButton spinButton;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int _moves, int _goal)
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
        if (substractMoves)
        {
            moves--;
        }

        if (points >= goal)
        {
            //you've won the game
            isGameEnded = true;
            //Display a victory screen
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            return;
        }
        if (moves == 0)
        {
            //lose the game
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            return;
        }
    }

    // Call this method to restart or reset the game
    public void RestartGame()
    {
        // Remove all match objects from the scene
        RemoveMatchObjects();

        // Reset any other game-related logic here, e.g., score, moves, etc.
        points = 0;
        moves = 5; // Or whatever the initial number of moves should be
        victoryPanel.SetActive(false); // Hide the victory panel
        losePanel.SetActive(false); // Hide the lose panel

        // Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Remove all match objects in the scene
    private void RemoveMatchObjects()
    {
        // Assuming your match objects are tagged with "MatchObject"
        GameObject[] matchObjects = GameObject.FindGameObjectsWithTag("MatchObject");

        foreach (GameObject matchObject in matchObjects)
        {
            Destroy(matchObject); // Destroy the match object
        }
    }
}
