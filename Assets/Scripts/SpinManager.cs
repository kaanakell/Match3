using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinManager : MonoBehaviour
{
    public float spinSpeed = 3f; // Speed for spinning
    public float decelerationDuration = 2f; // Duration of the slowdown
    public float startPosition = 5f; // The Y position where the objects start
    public float resetThreshold = -5f; // Y position to reset the object if it goes below

    private List<List<GameObject>> columns = new List<List<GameObject>>(); // Store all columns
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>(); // Store original positions
    private bool isStopping = false; // Whether the columns should stop
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
            yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        }
    }


    private IEnumerator SpinColumn(List<GameObject> column)
    {
        float columnSpinSpeed = spinSpeed; // Initial spin speed
        bool columnStopped = false;

        // Spin normally until stopping phase starts
        while (!isStopping)
        {
            foreach (GameObject food in column)
            {
                food.transform.Translate(Vector3.down * columnSpinSpeed * Time.deltaTime);

                // Reset position if it goes below the threshold
                if (food.transform.position.y < resetThreshold)
                {
                    food.transform.position = new Vector3(
                        food.transform.position.x,
                        startPosition,
                        food.transform.position.z
                    );
                }
            }

            yield return null;
        }

        // Final cycle before stopping
        columnSpinSpeed = spinSpeed; // Reset to initial speed for final round
        bool finalCycleCompleted = false;

        while (!finalCycleCompleted)
        {
            finalCycleCompleted = true; // Assume this is the final cycle

            foreach (GameObject food in column)
            {
                food.transform.Translate(Vector3.down * columnSpinSpeed * Time.deltaTime);

                if (food.transform.position.y < resetThreshold)
                {
                    // Reset position for the final round
                    food.transform.position = new Vector3(
                        food.transform.position.x,
                        startPosition,
                        food.transform.position.z
                    );
                }

                // Check if the food has reached its original position
                if (Vector3.Distance(food.transform.position, initialPositions[food]) > 0.1f)
                {
                    finalCycleCompleted = false; // Continue spinning if not aligned
                }
            }

            columnSpinSpeed = Mathf.Max(0.5f, columnSpinSpeed - (spinSpeed / (decelerationDuration / 2)) * Time.deltaTime);

            yield return null;
        }

        // Begin the smooth stopping and alignment
        StartSlotMachineStopEffect(column);
    }


    private void StartSlotMachineStopEffect(List<GameObject> column)
    {
        float maxAlignDuration = 0.5f; // Maximum duration for alignment (can be adjusted for speed)

        foreach (GameObject food in column)
        {
            Vector3 targetPosition = initialPositions[food];

            if (food.transform.position.y < targetPosition.y)
            {
                // If the item is below its original position, make a full loop first
                float distanceToLoop = startPosition - food.transform.position.y + targetPosition.y - resetThreshold;

                LeanTween.moveY(food, resetThreshold, spinSpeed)
                    .setOnComplete(() =>
                    {
                        // Smoothly align to the original position after completing the loop
                        float distance = Vector3.Distance(food.transform.position, targetPosition);
                        float alignDuration = Mathf.Clamp(distance / spinSpeed, 0.1f, maxAlignDuration);

                        LeanTween.move(food, targetPosition, alignDuration)
                            .setEase(LeanTweenType.easeOutQuad)
                            .setOnComplete(() =>
                            {
                                food.transform.position = targetPosition; // Ensure perfect alignment
                            });
                    });
            }
            else
            {
                // Align to the original position directly
                float distance = Vector3.Distance(food.transform.position, targetPosition);
                float alignDuration = Mathf.Clamp(distance / spinSpeed, 0.1f, maxAlignDuration);

                LeanTween.move(food, targetPosition, alignDuration)
                    .setEase(LeanTweenType.easeOutQuad)
                    .setOnComplete(() =>
                    {
                        food.transform.position = targetPosition; // Ensure perfect alignment
                    });
            }
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
