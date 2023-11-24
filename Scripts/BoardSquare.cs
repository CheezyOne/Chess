using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public bool Occupied = false;
    public int X, Y;
    public GameObject MarkForAvailableSpot;
    public GameObject FigureOnTheSpot;
    public Color NormalColor, YellowColor;
    private void Awake()
    {
        NormalColor = GetComponent<SpriteRenderer>().color;
        YellowColor = new Color(255, 255, 126);
    }
}
