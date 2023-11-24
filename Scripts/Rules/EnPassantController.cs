using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassantController : MonoBehaviour
{
    [SerializeField] private GameObject[] AllSquares;
    public static GameObject EnPassantedSquare;
    public static GameObject MovedPawn;
    public static bool IsAvailable=false;

    private void OnEnable()
    {
        Figure.OnMove += DefineEnPassantSquare;
    }
    private void OnDisable()
    {
        Figure.OnMove -= DefineEnPassantSquare;
    }
    private void DefineEnPassantSquare()
    {

        if (IsAvailable)
        {
            if (MovedPawn == null)
                return;
            BoardSquare CurrentSquare = MovedPawn.GetComponent<Pawn>().CurrentSquare.GetComponent<BoardSquare>();
            if (MovedPawn.GetComponent<Pawn>().IsWhite)
            {
                for (int i=0;i<AllSquares.Length;i++)
                {
                    if (AllSquares[i].GetComponent<BoardSquare>().Y - CurrentSquare.Y==1&& AllSquares[i].GetComponent<BoardSquare>().X == CurrentSquare.X)
                    {
                        EnPassantedSquare = AllSquares[i];
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < AllSquares.Length; i++)
                {
                    if (AllSquares[i].GetComponent<BoardSquare>().Y - CurrentSquare.Y == -1 && AllSquares[i].GetComponent<BoardSquare>().X == CurrentSquare.X)
                    {
                        EnPassantedSquare = AllSquares[i];
                        break;
                    }
                }
            }
        }
        else
        {
            EnPassantedSquare = null; 
        }
    }
}
