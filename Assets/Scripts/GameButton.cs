using UnityEngine;

public class GameButton : MonoBehaviour
{
    public enum ButtonType { Win, Lose }
    public ButtonType buttonType;

    private void OnMouseDown()
    {
        // Check button type and handle accordingly
        if (buttonType == ButtonType.Win || buttonType == ButtonType.Lose)
        {
            GameManager.Instance.RestartGame();
        }
    }
}

