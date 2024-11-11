using UnityEngine;

public class Foods : MonoBehaviour
{
    public FoodType foodType;

    public int xIndex;
    public int yIndex;
    public bool isMatched;
    public bool isMoving;

    private Vector2 currentPos;
    private Vector2 targetPos;

    public Foods(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

}

public enum FoodType
{
    Apple,
    Cherry,
    Peach,
    Honey,
    WaterMelon
}
