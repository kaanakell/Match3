using UnityEngine;

public class SpinButton : MonoBehaviour
{
    private SpinManager spinManager;
    public GameObject spinText;  // The GameObject that contains "Spin" text
    public GameObject stopText;  // The GameObject that contains "Stop" text

    private Collider2D buttonCollider;  // To control button interaction

    void Awake()
    {
        spinManager = GameObject.Find("SpinManager").GetComponent<SpinManager>();
    }

    private void Start()
    {
        buttonCollider = GetComponent<Collider2D>();

        if (spinManager == null)
            Debug.LogError("SpinManager not found in the scene.");

        if (buttonCollider == null)
            Debug.LogError("Collider2D is missing on the SpinButton object.");
    }

    private void OnMouseDown()
    {
        if (buttonCollider.enabled && spinManager != null)
        {
            if (!spinManager.isStopping)
            {
                spinManager.StopAllColumns();
            }
            else
            {
                spinManager.Spin();
            }

            UpdateButtonText();
        }
    }

    public void DeactivateButton()
    {
        buttonCollider.enabled = false;
    }

    private void UpdateButtonText()
    {
        if (!spinManager.isStopping)
        {
            spinText.SetActive(false);
            stopText.SetActive(true);
        }
        else
        {
            spinText.SetActive(true);
            stopText.SetActive(false);
        }
    }
}
