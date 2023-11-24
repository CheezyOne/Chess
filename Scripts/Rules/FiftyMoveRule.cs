using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiftyMoveRule : MonoBehaviour
{
    public int CounterForEmptyMoves=1 , CounterForOddNumber=0;
    
    private void OnEnable()
    {
        Figure.OnPawnMove += ClearCounter;
        Figure.OnKill += ClearCounter;
        Figure.OnMove += CountUp;
    }
    private void OnDisable()
    {
        Figure.OnPawnMove -= ClearCounter;
        Figure.OnKill -= ClearCounter;
        Figure.OnMove -= CountUp;
    }
    private void CheckTheCounter()
    {
        if(CounterForEmptyMoves>50)
        {
            Figure.IsDraw = true;
        }
    }
    private void CountUp()
    {
        if (CounterForOddNumber == 0)
        {
            CounterForOddNumber++;
            CounterForEmptyMoves++;
            CheckTheCounter();
        }
        else
            CounterForOddNumber = 0;
    }
    private void ClearCounter()
    {
        CounterForEmptyMoves = 0;
    }
}
