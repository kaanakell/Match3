using System.Collections;
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

    //MoveToTarget
    public void MoveToTarget(Vector2 _targetPos)
    {
        StartCoroutine(MoveCoroutine(_targetPos));
    }
    //MoveCoroutine
    private IEnumerator MoveCoroutine(Vector2 _targetPos)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector2.Lerp(startPosition, _targetPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = _targetPos;
        isMoving = false;
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
