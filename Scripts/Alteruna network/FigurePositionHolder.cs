using Alteruna;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FigurePositionHolder : AttributesSync
{
    [SynchronizableField] public string MovedFigure= " ";
    [SynchronizableField] public int NetworkPosition = -1;
    [SynchronizableField] public int NetworkColorFirstSquare = 0, NetworkColorSecondSquare=0;
    public static int NewFigureToCreate = 0;
    [SynchronizableField] private int NewFigureNumberSync = 0;

    private void Awake()
    {
        //Figure.OnMove += ColorSquares;
        Figure.ChangeNewFigureSync += ChangeNewFigureSync;
       // Figure.OnNewFigureSpawn += ChangeStaticNewFigure;
        StartCoroutine(CheckNetworkPositionCoroutine());
    }
    private void Update()
    {
        NewFigureToCreate = NewFigureNumberSync;
    }
    private void ChangeNewFigureSync()
    {
        NewFigureNumberSync = NewFigureToCreate;
    }
    private IEnumerator CheckNetworkPositionCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        CheckPosition();
        yield return CheckNetworkPositionCoroutine();
    }
    [SynchronizableMethod] private void ColorSquares()
    {
        Figure FigureComponent = GetComponent<Figure>();
        for (int i=0;i< FigureComponent.AllSquares.Length;i++)
        {
            FigureComponent.AllSquares[i].GetComponent<SpriteRenderer>().color = FigureComponent.AllSquares[i].GetComponent<BoardSquare>().NormalColor;
        }
        FigureComponent.AllSquares[NetworkColorFirstSquare].GetComponent<SpriteRenderer>().color = Color.yellow;
        FigureComponent.AllSquares[NetworkColorSecondSquare].GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    private void CheckPosition()
    {
        Figure FigureComponent = GetComponent<Figure>();
        if (name == MovedFigure)
        {
            for (int i = 0; i < FigureComponent.AllSquares.Length; i++)
            {
                if (FigureComponent.CurrentSquare == FigureComponent.AllSquares[i])
                {
                    if (i != GetComponent<FigurePositionHolder>().NetworkPosition)
                    {
                        if (FigureComponent.name.Contains("Pawn"))
                        {
                            if (FigureComponent.PawnIsGoingToReachTheLastSquare())
                            {
                                int AvailablePawnSquareNumber = FindAvailableSquareToGoTo(FigureComponent);
                                FigureComponent.PawnClickOnLastPosition(FigureComponent.AllSquares[NetworkPosition], AvailablePawnSquareNumber);
                                BroadcastRemoteMethod("ColorSquares");
                                return;
                            } 
                        }
                        int AvailableSquareNumber = FindAvailableSquareToGoTo(FigureComponent);
                        FigureComponent.AssignNewPosition(FigureComponent.AllSquares[NetworkPosition], AvailableSquareNumber);
                        BroadcastRemoteMethod("ColorSquares");

                    }
                }
            }
        }
    }
    private int FindAvailableSquareToGoTo(Figure FigureComponent)
    {
        for(int i=0;i< FigureComponent.AvailableSquares.Length;i++)
        {
            if (FigureComponent.AvailableSquares[i] == FigureComponent.AllSquares[NetworkPosition])
            {
                return i;
            }
        }
        return 0;
    }
}
