using UnityEngine;

public class ButtonRaycastHandler : MonoBehaviour
{
    public LayerMask buttonLayerMask; // Set this to "ButtonLayer" in the Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Cast a ray and check if it hits an object on the "ButtonLayer"
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buttonLayerMask))
            {
                // Check if the clicked object has a GameButton component
                GameButton button = hit.collider.GetComponent<GameButton>();
                if (button != null)
                {
                    // Call RestartGame on the GameManager
                    GameManager.Instance.RestartGame();
                }
            }
        }
    }
}
