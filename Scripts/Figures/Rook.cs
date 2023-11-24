using UnityEngine;

public class Rook : Figure
{
    private bool HasFigureInBetween = false;
    public bool CanCastle=true;
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
                if (IsNotAvailable(i))
                    continue;
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if ((BoardSquareComponent.X == BoardSquareComponents[i].X) || (BoardSquareComponent.Y == BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWay(i);
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
            if ((BoardSquareComponent.X == BoardSquareComponents[i].X) || (BoardSquareComponent.Y == BoardSquareComponents[i].Y))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                CheckIfSomethingIsInTheWay(i);
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
    private void CheckIfSomethingIsInTheWay(int i)
    {
        for (int j = 0; j < OccupiedSquaresCount; j++)
        {
            OccupiedSquareComponent = OccupiedSquares[j].GetComponent<BoardSquare>();
            if (BoardSquareComponent.X == OccupiedSquareComponent.X)
            {
                if (BoardSquareComponent.Y > BoardSquareComponents[i].Y)
                {
                    if(OccupiedSquareComponent.Y < BoardSquareComponent.Y && OccupiedSquareComponent.Y > BoardSquareComponents[i].Y)
                    {
                        HasFigureInBetween = true;
                    }
                }
                else if (BoardSquareComponent.Y < BoardSquareComponents[i].Y)
                {
                    if (OccupiedSquareComponent.Y > BoardSquareComponent.Y && OccupiedSquareComponent.Y < BoardSquareComponents[i].Y)
                    {
                        HasFigureInBetween = true;
                    }
                }
            }
            else if(BoardSquareComponent.Y == OccupiedSquareComponent.Y)
            {
                if (BoardSquareComponent.X > BoardSquareComponents[i].X)
                {
                    if (OccupiedSquareComponent.X < BoardSquareComponent.X && OccupiedSquareComponent.X > BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
                    }
                }
                else if (BoardSquareComponent.X < BoardSquareComponents[i].X)
                {
                    if (OccupiedSquareComponent.X > BoardSquareComponent.X && OccupiedSquareComponent.X < BoardSquareComponents[i].X)
                    {
                        HasFigureInBetween = true;
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
