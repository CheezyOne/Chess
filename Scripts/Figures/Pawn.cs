using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pawn : Figure
{
    public int MoveMultiplier = 2;
    private GameObject EnPassantSquare;
    private new void OnEnable()
    {
        Figure.OnMove += OnMoveFunction;
        Figure.OnMove += FindAllDangersForEnemyKing;
    }
    private void OnDisable()
    {
        Figure.OnMove -= OnMoveFunction;
        Figure.OnMove -= FindAllDangersForEnemyKing;
    }

    public void FindAllDangersForEnemyKing()
    {
        CoverKingOnce = false;
        DangerForEnemyKingCount = 0;
        OccupiedSquaresCount = 0;
        for (int i = 0; i < AllSquares.Length; i++)
        {
            OccupiedSquares[i] = null;
            DangerForEnemyKing[i] = null;
            if (CheckIfSpotIsOccupiedByFriend(i) || CheckIfSpotIsOccupiedByEnemy(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if (IsWhite)
            {
                if ((Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) == 1) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y == 1))
                {
                    DangerForEnemyKing[DangerForEnemyKingCount] = AllSquares[i];
                    DangerForEnemyKingCount++;
                }
            }
            else
            {
                if ((Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) == 1) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y == -1) )
                {
                    DangerForEnemyKing[DangerForEnemyKingCount] = AllSquares[i];
                    DangerForEnemyKingCount++;
                }
            }
        }
    }
    private bool IsSquareAheadOccupied()
    {
        for(int i=0;i<AllSquares.Length;i++)
        {
            if (IsWhite)
            {
                if (BoardSquareComponents[i].Y - BoardSquareComponent.Y == -1 && BoardSquareComponents[i].X == BoardSquareComponent.X)
                {
                    if (BoardSquareComponents[i].Occupied)
                        return true;
                }
            }
            else
            {
                if (BoardSquareComponents[i].Y - BoardSquareComponent.Y == 1 && BoardSquareComponents[i].X == BoardSquareComponent.X)
                {
                    if (BoardSquareComponents[i].Occupied)
                        return true;
                }
            }
        }
        return false;
    }
    public override void FindAllAvailableSquares()
    {
        FindAllDangersForEnemyKing();
        if (IsWhite != WhiteToMove)
        {
            return;
        }
        DangerForEnemyKingCount = 0;
        AvailableSquaresCounter = 0;
        OccupiedSquaresCount = 0;
        for (int i = 0; i < AllSquares.Length; i++)
        {
            OccupiedSquares[i] = null;
            AvailableSquares[i] = null;
            DangerForEnemyKing[i] = null;
            if (CheckIfSpotIsOccupiedByFriend(i) || CheckIfSpotIsOccupiedByEnemy(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        EnPassantSquare = EnPassantController.EnPassantedSquare;
        for (int i = 0; i < AllSquares.Length; i++)
        {

            if (IsWhite)
            {
                if ((BoardSquareComponent.X == BoardSquareComponents[i].X) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y <= MoveMultiplier) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y>=0))
                {
                    if(IsSquareAheadOccupied())
                        continue;
                    if (CheckIfSpotIsOccupiedByFriend(i) || CheckIfSpotIsOccupiedByEnemy(i))
                        continue;
                    if (IsNotAvailable(i))
                        continue;
                    AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                    AvailableSquaresCounter++;
                }
                else if((Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X)==1) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y == 1)&& (CheckIfSpotIsOccupiedByEnemy(i)|| AvailableForEnPassant(i)))
                {
                    if (IsNotAvailable(i))
                        continue;
                    AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                    AvailableSquaresCounter++;
                }
            }
            else
            {
                if ((BoardSquareComponent.X == BoardSquareComponents[i].X) && (BoardSquareComponents[i].Y - BoardSquareComponent.Y <= MoveMultiplier) && (BoardSquareComponents[i].Y - BoardSquareComponent.Y >= 0))
                {
                    if (IsSquareAheadOccupied())
                        continue;
                    if (CheckIfSpotIsOccupiedByFriend(i) || CheckIfSpotIsOccupiedByEnemy(i))
                        continue;
                    if (IsNotAvailable(i))
                        continue;
                    AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                    AvailableSquaresCounter++;
                }
                else if ((Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) == 1) && (BoardSquareComponent.Y - BoardSquareComponents[i].Y == -1) && (CheckIfSpotIsOccupiedByEnemy(i) || AvailableForEnPassant(i)))
                {
                    if (IsNotAvailable(i))
                        continue;
                    AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                    AvailableSquaresCounter++;
                }
            }
        }
    }
    public bool AvailableForEnPassant(int i)
    {
        if (AllSquares[i] == EnPassantSquare && EnPassantController.IsAvailable)
        {
            return true;
        }
        return false;
    }
    private bool CheckIfSpotIsOccupiedByFriend(int i)
    {
        if (BoardSquareComponents[i].Occupied)
        {
            if (BoardSquareComponents[i].FigureOnTheSpot.GetComponent<Figure>().IsWhite == this.IsWhite)
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckIfSpotIsOccupiedByEnemy(int i)
    {
        if (BoardSquareComponents[i].Occupied)
        {
            if (BoardSquareComponents[i].FigureOnTheSpot.GetComponent<Figure>().IsWhite != this.IsWhite)
            {
                return true;
            }
        }
        return false;
    }
    public override bool PawnIsGoingToReachTheLastSquare()
    {
        int PawnY = CurrentSquare.GetComponent<BoardSquare>().Y;
        if ((PawnY == 1 && IsWhite) || (PawnY == 6 && !IsWhite))
        {
            PawnLastSquare = CurrentSquare;
            return true;
        }
        return false;
    }
    public override bool PawnReachedTheEndingSquare()
    {
        int PawnY = CurrentSquare.GetComponent<BoardSquare>().Y;
        if (PawnY == 0 || PawnY==7)
        {
            PawnLastSquare = CurrentSquare;
            return true;
        }
        return false;
    }

}
