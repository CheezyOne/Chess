using UnityEngine;

public class Queen : Figure
{
    private bool HasFigureInBetween = false;
    private new void OnEnable()
    {
        Figure.OnMove += OnMoveFunction;
        Figure.OnMove += FindAllDangersForEnemyKing;
        Figure.OnMove += IsCheckingEnemyKing;
    }
    private void OnDisable()
    {
        Figure.OnMove -= OnMoveFunction;
        Figure.OnMove -= FindAllDangersForEnemyKing;
        Figure.OnMove -= IsCheckingEnemyKing;
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
            if (Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) == Mathf.Abs(BoardSquareComponent.Y - BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWayBishopStyle(i);
                if (HasFigureInBetween)
                {
                    HasFigureInBetween = false;
                    continue;
                }
                DangerForEnemyKing[DangerForEnemyKingCount] = AllSquares[i];
                DangerForEnemyKingCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if ((BoardSquareComponent.X == BoardSquareComponents[i].X) || (BoardSquareComponent.Y == BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWayRookStyle(i);
                if (HasFigureInBetween)
                {
                    HasFigureInBetween = false;
                    continue;
                }
                DangerForEnemyKing[DangerForEnemyKingCount] = AllSquares[i];
                DangerForEnemyKingCount++;
            }
        }
    }
    public override void FindAllAvailableSquares()
    {
        
        FindAllDangersForEnemyKing();
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
            if (CheckIfSpotIsOccupiedByFriend(i) || CheckIfSpotIsOccupiedByEnemy(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if (Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X) == Mathf.Abs(BoardSquareComponent.Y - BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWayBishopStyle(i);
                if (HasFigureInBetween)
                {
                    HasFigureInBetween = false;
                    continue;
                }
                if (CheckIfSpotIsOccupiedByFriend(i))
                    continue;
                if (IsNotAvailable(i))
                    continue;
                AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                AvailableSquaresCounter++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if ((BoardSquareComponent.X == BoardSquareComponents[i].X) || (BoardSquareComponent.Y == BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWayRookStyle(i);
                if (HasFigureInBetween)
                {
                    HasFigureInBetween = false;
                    continue;
                }
                if (CheckIfSpotIsOccupiedByFriend(i))
                    continue;
                if (IsNotAvailable(i))
                    continue;
                AvailableSquares[AvailableSquaresCounter] = AllSquares[i];
                AvailableSquaresCounter++;
            }
        }

    }
    private void CheckIfSomethingIsInTheWayBishopStyle(int i)
    {
        for (int j = 0; j < OccupiedSquaresCount; j++)
        {
            OccupiedSquareComponent = OccupiedSquares[j].GetComponent<BoardSquare>();
            if (!(Mathf.Abs(BoardSquareComponent.X - OccupiedSquareComponent.X) == Mathf.Abs(BoardSquareComponent.Y - OccupiedSquareComponent.Y)))
            {
                continue;
            }
            if (BoardSquareComponent.X > OccupiedSquareComponent.X)
            {
                if (BoardSquareComponent.Y > OccupiedSquareComponent.Y)
                {
                    if (OccupiedSquareComponent.Y > BoardSquareComponents[i].Y && OccupiedSquareComponent.X > BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }//Left down
                else if (BoardSquareComponent.Y < OccupiedSquareComponent.Y)
                {
                    if (OccupiedSquareComponent.Y < BoardSquareComponents[i].Y && OccupiedSquareComponent.X > BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                } //left up
            }
            else if (BoardSquareComponent.X < OccupiedSquareComponent.X)
            {
                if (BoardSquareComponent.Y > OccupiedSquareComponent.Y)
                {
                    if (OccupiedSquareComponent.Y > BoardSquareComponents[i].Y && OccupiedSquareComponent.X < BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }//Right up
                else if (BoardSquareComponent.Y < OccupiedSquareComponent.Y)
                {
                    if (OccupiedSquareComponent.Y < BoardSquareComponents[i].Y && OccupiedSquareComponent.X < BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                } //Right down
            }
        }
    }
    private void CheckIfSomethingIsInTheWayRookStyle(int i)
    {
        for (int j = 0; j < OccupiedSquaresCount; j++)
        {
            OccupiedSquareComponent = OccupiedSquares[j].GetComponent<BoardSquare>();
            if (BoardSquareComponent.X == OccupiedSquareComponent.X)
            {
                if (BoardSquareComponent.Y > BoardSquareComponents[i].Y)
                {
                    if (OccupiedSquareComponent.Y < BoardSquareComponent.Y && OccupiedSquareComponent.Y > BoardSquareComponents[i].Y)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }
                else if (BoardSquareComponent.Y < BoardSquareComponents[i].Y)
                {
                    if (OccupiedSquareComponent.Y > BoardSquareComponent.Y && OccupiedSquareComponent.Y < BoardSquareComponents[i].Y)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }
            }
            else if (BoardSquareComponent.Y == OccupiedSquareComponent.Y)
            {
                if (BoardSquareComponent.X > BoardSquareComponents[i].X)
                {
                    if (OccupiedSquareComponent.X < BoardSquareComponent.X && OccupiedSquareComponent.X > BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }
                else if (BoardSquareComponent.X < BoardSquareComponents[i].X)
                {
                    if (OccupiedSquareComponent.X > BoardSquareComponent.X && OccupiedSquareComponent.X < BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                        return;
                    }
                }
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
