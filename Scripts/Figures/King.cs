using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class King : Figure
{
    public List<GameObject> EnemyPossibleSquares;
    public List<GameObject> EnemyPieces;
    public bool CanCastle = true;
    public GameObject LeftRook, RightRook;
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
    private bool CanCastleRight(int i)
    {
        if (RightRook == null)
            return false;
        if (CanCastle && RightRook.GetComponent<Rook>().CanCastle)
        {
            if (!(BoardSquareComponents[i].X - BoardSquareComponent.X == 2 && BoardSquareComponents[i].Y == BoardSquareComponent.Y))
            {
                return false;
            }
            if (BlackKingIsChecked && !IsWhite)
            {
                return false;
            }
            if (CheckIfTheSquareIsDangerous(i - 1) || CheckIfTheSquareIsDangerous(i))
            {
                return false;
            }
            if (CheckIfSpotIsOccupiedByFriend(i - 1) || CheckIfSpotIsOccupiedByFriend(i))
            {
                return false;
            }
            if (CheckIfSpotIsOccupiedByEnemy(i - 1) || CheckIfSpotIsOccupiedByEnemy(i))
            {
                return false;
            }
            return true;
        }
        else
            return false;
    }
    private bool CanCastleLeft(int i)
    {
        if (LeftRook == null)
            return false;
        if (CanCastle && LeftRook.GetComponent<Rook>().CanCastle)
        {
            if (!(BoardSquareComponents[i].X - BoardSquareComponent.X == -2 && BoardSquareComponents[i].Y == BoardSquareComponent.Y))
            {
                return false;
            }
            if (BlackKingIsChecked && !IsWhite)
            {
                return false;
            }
            if (CheckIfTheSquareIsDangerous(i + 1) || CheckIfTheSquareIsDangerous(i)|| CheckIfTheSquareIsDangerous(i - 1))
            {
                return false;
            }
            if (CheckIfSpotIsOccupiedByFriend(i + 1) || CheckIfSpotIsOccupiedByFriend(i ) || CheckIfSpotIsOccupiedByFriend(i-1))
            {
                return false;
            }
            if (CheckIfSpotIsOccupiedByEnemy(i + 1) || CheckIfSpotIsOccupiedByEnemy(i) || CheckIfSpotIsOccupiedByEnemy(i-1))
            {
                return false;
            }
            return true;
        }
        return false;
    }
    private void FindAllDangerousSquares()
    {
        if (IsWhite != WhiteToMove)
            return;
        EnemyPossibleSquares = new List<GameObject>();
        for(int  i = 0; i < EnemyPieces.Count; i++)
        {
            try
            {
                for (int j = 0; j < EnemyPieces[i].GetComponent<Figure>().DangerForEnemyKing.Length; j++)
                {
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<Bishop>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<Bishop>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<Rook>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<Rook>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<Queen>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<Queen>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<King>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<King>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<Horse>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<Horse>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                    try
                    {
                        if (EnemyPossibleSquares.Contains(EnemyPieces[i].GetComponent<Pawn>().DangerForEnemyKing[j]))//Get not a figure...
                            continue;
                        EnemyPossibleSquares.Add(EnemyPieces[i].GetComponent<Pawn>().DangerForEnemyKing[j]);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
        }
    }
    private bool CheckIfTheSquareIsDangerous(int i)
    {
        for(int j=0;j<EnemyPossibleSquares.Count;j++)
        {
            try
            {
                if (EnemyPossibleSquares[j].GetComponent<BoardSquare>() == BoardSquareComponents[i])
                {
                    return true;
                }
            }
            catch
            {

            }
            }
        return false;
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
            if (CheckIfSpotIsOccupiedByFriend(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if (Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) <= 1 && Mathf.Abs(BoardSquareComponent.Y - BoardSquareComponents[i].Y) <= 1)
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                DangerForEnemyKing[DangerForEnemyKingCount] = AllSquares[i];
                DangerForEnemyKingCount++;
            }
        }
    }
    public override void FindAllAvailableSquares()
    {
        FindAllDangersForEnemyKing();
        FindAllDangerousSquares();
        /*
        if (IsWhite != WhiteToMove)
            return;
        */
        AvailableSquaresCounter = 0;
        OccupiedSquaresCount = 0;
        for (int i = 0; i < AllSquares.Length; i++)
        {
            OccupiedSquares[i] = null;
            AvailableSquares[i] = null;
            if (CheckIfSpotIsOccupiedByFriend(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if (Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) <= 1 && Mathf.Abs(BoardSquareComponent.Y - BoardSquareComponents[i].Y)<=1)
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                {
                        continue;
                }
                if (CheckIfSpotIsOccupiedByFriend(i))
                    continue;

                if (CheckIfTheSquareIsDangerous(i))
                    continue;
                AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                AvailableSquaresCounter++;
            }
            else if(CanCastleLeft(i)||CanCastleRight(i))
            {
                AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                AvailableSquaresCounter++;
            }    
        }
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
}
