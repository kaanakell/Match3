using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Static reference

    public GameObject backgroundPanel; // Grey background
    public GameObject victoryPanel;
    public GameObject losePanel;

    public int goal; // The amount of points you need to get to win
    public int moves; // The number of turns you can take
    public int points; // The current points you have earned

    public bool isGameEnded;

    public TextMeshPro pointsTxt;
    public TextMeshPro movesTxt;
    public TextMeshPro goalTxt;
    public SpinButton spinButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure panels are hidden at the start
        ResetPanels();
    }

    public void Initialize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    private void Update()
    {
        // Update the UI elements
        pointsTxt.text = "Points: " + points.ToString();
        movesTxt.text = "Moves: " + moves.ToString();
        goalTxt.text = "Goal: " + goal.ToString();
    }

    public void ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        points += pointsToGain;
        if (subtractMoves)
        {
            moves--;
        }

        if (points >= goal)
        {
            // You've won the game
            EndGame(true);
            return;
        }

        if (moves == 0)
        {
            // Lose the game
            EndGame(false);
            return;
        }
    }

    private void EndGame(bool won)
    {
        isGameEnded = true;

        // Display the appropriate end game panel
        if (won)
        {
            victoryPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
    }

    private void ResetPanels()
    {
        // Hide all panels at the start of the game
        victoryPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Attached to a button to change scene when winning or losing
    public void RestartGame()
    {
        isGameEnded = false; // Reset game state

        // Reload the scene to reset all game elements
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
