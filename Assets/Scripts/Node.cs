using UnityEngine;

public class Node
{
    public bool isUsable;
    public GameObject food;

    public Node(bool _isUsable, GameObject _food)
    {
        isUsable = _isUsable;
        food = _food;
    }
}
