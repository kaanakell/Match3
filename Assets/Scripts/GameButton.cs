using UnityEngine;

public class GameButton : MonoBehaviour
{
    public enum ButtonType { Win, Lose }
    public ButtonType buttonType;

    private void OnMouseDown()
    {
        // Check button type and call the appropriate method in GameManager
        if (buttonType == ButtonType.Win)
        {
            GameManager.Instance.WinGame();
        }
        else if (buttonType == ButtonType.Lose)
        {
            GameManager.Instance.LoseGame();
        }
    }
}
