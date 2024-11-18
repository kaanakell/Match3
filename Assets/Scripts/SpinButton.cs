using UnityEngine;

public class SpinButton : MonoBehaviour
{
    private SpinManager spinManager;

    private void Start()
    {
        // Find the SpinManager in the scene
        spinManager = GameObject.Find("SpinManager").GetComponent<SpinManager>();

        if (spinManager == null)
        {
            Debug.LogError("SpinManager not found in the scene.");
        }
    }

    private void OnMouseDown()
    {
        // Trigger column stopping and resetting
        if (spinManager != null)
        {
            spinManager.StopAllColumns();
        }
    }
}
