using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionHolding : MonoBehaviour
{
    [SerializeField] private GameObject[] AllSquares;
    [SerializeField] private List<List<GameObject>> Positions = new List<List<GameObject>>();
    private List<GameObject> newPosition;
    private bool HaventSeenThisPosition = false;
    private int CounterForPositions = 0, CounterForOnMoves = 0;
    private void OnEnable()
    {
        Figure.OnMove += NewMove;
    }
    private void OnDisable()
    {
        Figure.OnMove -= NewMove;
    }
    private void RemberNewPosition()
    {
        newPosition = new List<GameObject>();
        for (int i = 0; i < AllSquares.Length; i++)
        {
            newPosition.Add(AllSquares[i].GetComponent<BoardSquare>().FigureOnTheSpot);
        }
    }
    private void CheckForExistingPositions()
    {
        for (int i = 0; i < Positions.Count; i++)
        {
            for (int j = 0; j < newPosition.Count; j++)
            {
                if (newPosition[j] != Positions[i][j])
                {
                    HaventSeenThisPosition = true;
                    break;
                }
            }
            if (!HaventSeenThisPosition)
            {
                CounterForPositions++;
            }
            HaventSeenThisPosition = false;
        }
        if (CounterForPositions > 1)
        {
            Figure.IsDraw = true;
            Debug.Log("Repetition");
        }
        CounterForPositions = 0;
    }
    private void AddNewPosition()
    {
        Positions.Add(newPosition);
    }
    private void NewMove()
    {
        if (CounterForOnMoves == 0)
        {
            CounterForOnMoves++;
            RemberNewPosition();
            CheckForExistingPositions();
            AddNewPosition();
        }
        else
            CounterForOnMoves = 0;
    }
    void Start()
    {
        NewMove();
    }
}
