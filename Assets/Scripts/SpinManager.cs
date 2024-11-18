using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinManager : MonoBehaviour
{
    public float spinSpeed = 3f; // Speed for spinning
    public float decelerationDuration = 2f; // Time it takes for columns to stop
    public float resetThreshold = -5f; // Y position to reset the food item
    public float startPosition = 5f; // Y position to reset to (top of the column)
    public float startDelay = 0.2f; // Delay between starting each column's spin

    private List<List<GameObject>> columns = new List<List<GameObject>>(); // Store all columns
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>(); // Store original positions
    private bool isStopping = false; // Check if the stopping process has begun
    private BoardManager boardManager;

    private void Start()
    {
        // Find the BoardManager to access the board configuration
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();

        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found. Ensure it exists in the scene.");
            return;
        }
    }

    public void InitializeColumns()
    {
        // Initialize and populate the list of columns
        for (int x = 0; x < boardManager.boardWidth; x++)
        {
            var column = GetColumn(x);
            columns.Add(column);

            // Store initial positions
            foreach (var food in column)
            {
                initialPositions[food] = food.transform.position;
            }
        }
    }

    public void Spin()
    {
        StartCoroutine(SpinColumns());
    }

    private IEnumerator SpinColumns()
    {
        foreach (var column in columns)
        {
            StartCoroutine(SpinColumn(column));
            yield return new WaitForSeconds(startDelay);
        }
    }

    private IEnumerator SpinColumn(List<GameObject> column)
    {
        float columnSpinSpeed = spinSpeed; // Individual column spin speed

        while (!isStopping || columnSpinSpeed > 0)
        {
            foreach (GameObject food in column)
            {
                // Move the food downward
                food.transform.Translate(Vector3.down * columnSpinSpeed * Time.deltaTime);

                // Reset position if it goes below the reset threshold
                if (food.transform.position.y < resetThreshold)
                {
                    food.transform.position = new Vector3(
                        food.transform.position.x,
                        startPosition,
                        food.transform.position.z
                    );
                }
            }

            // Decelerate the spin speed if stopping
            if (isStopping)
            {
                columnSpinSpeed = Mathf.Max(0f, columnSpinSpeed - (spinSpeed / decelerationDuration) * Time.deltaTime);
            }

            yield return null;
        }

        // Reset positions to the initial positions
        foreach (GameObject food in column)
        {
            food.transform.position = initialPositions[food];
        }
    }

    public void StopAllColumns()
    {
        if (isStopping) return; // Prevent multiple triggers
        isStopping = true;

        Debug.Log("Stopping all columns...");
    }

    private List<GameObject> GetColumn(int x)
    {
        List<GameObject> column = new List<GameObject>();

        for (int y = 0; y < boardManager.boardHeight; y++)
        {
            Node node = boardManager.boardManager[x, y];
            if (node.food != null)
            {
                column.Add(node.food);
            }
        }

        return column;
    }
}
