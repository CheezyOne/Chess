using UnityEngine;

public class Horse : Figure
{
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
            if (CheckIfSpotIsOccupiedByFriend(i))
            {
                OccupiedSquares[OccupiedSquaresCount] = AllSquares[i];
                OccupiedSquaresCount++;
            }
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if ((DifferenceInX(i) == 1 && DifferenceInY(i) == 2) || (DifferenceInX(i) == 2 && DifferenceInY(i) == 1))
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
        /*
        if (IsWhite != WhiteToMove)
            return;
        */
        AvailableSquaresCounter = 0;
        for (int i = 0; i < AllSquares.Length; i++)
        {
            AvailableSquares[i] = null;
        }
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if ((DifferenceInX(i) == 1 && DifferenceInY(i)==2) || (DifferenceInX(i) == 2 && DifferenceInY(i) == 1))
            {
                if (BoardSquareComponent.X == BoardSquareComponents[i].X && BoardSquareComponent.Y == BoardSquareComponents[i].Y)
                    continue;
                if (CheckIfSpotIsOccupiedByFriend(i))
                    continue;
                if (IsNotAvailable(i))
                    continue;
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
    private int DifferenceInX(int i)
    {
        return Mathf.Abs(BoardSquareComponent.X - BoardSquareComponents[i].X);
    }
    private int DifferenceInY(int i)
    {
        return Mathf.Abs(BoardSquareComponent.Y - BoardSquareComponents[i].Y);
    }
}
