using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class CheckIfFigureIsPinned : MonoBehaviour
{
    [SerializeField] private LayerMask Mask;
    public List<GameObject> WhiteFigures ,BlackFigures;
    private List<GameObject> HitFigures;
    private List<Vector2> Vectors;
    [SerializeField] private GameObject WhiteKing, BlackKing;
    private List<GameObject> WhiteKingHitFigures, BlackKingHitFigures;
    //private List<GameObject> WhiteKingFigures, BlackKingFigures;
    private void OnEnable()
    {
        Figure.OnMove += MakeARay;
        //Figure.OnMove += ClearUnavailableSquares;
    }
    private void OnDisable()
    {
        Figure.OnMove -= MakeARay;
        //Figure.OnMove -= ClearUnavailableSquares;
    }
    private List<Vector2> FindAllVectors(GameObject Figure)
    {
        List<Vector2> AllVectors =new List<Vector2>();
        if(Figure.TryGetComponent<King>(out King king) || Figure.TryGetComponent<Queen>(out Queen queen) || Figure.TryGetComponent<Bishop>(out Bishop bishop))
        {
            AllVectors.Add(Vector2.up + Vector2.right);
            AllVectors.Add(Vector2.down + Vector2.right);
            AllVectors.Add(Vector2.up + Vector2.left);
            AllVectors.Add(Vector2.down + Vector2.left);
        }
        if (Figure.TryGetComponent<King>(out king) || Figure.TryGetComponent<Queen>(out queen) || Figure.TryGetComponent<Rook>(out Rook rook))
        {
            AllVectors.Add(Vector2.down);
            AllVectors.Add(Vector2.up);
            AllVectors.Add(Vector2.left);
            AllVectors.Add(Vector2.right);
        }
        return AllVectors;
    }
  
    private void ClearUnavailableSquares()
    {
        if (!Figure.WhiteToMove)
        {
            foreach (GameObject figure in WhiteFigures)
            {
                try
                {
                    figure.GetComponent<Queen>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Rook>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Horse>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Bishop>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Pawn>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
            }
        }
        else
        {
            foreach (GameObject figure in BlackFigures)
            {
                try
                {
                    figure.GetComponent<Queen>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Rook>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Horse>().UnAvailableSquares= new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Bishop>().UnAvailableSquares= new List<BoardSquare>();
                }
                catch { }
                try
                {
                    figure.GetComponent<Pawn>().UnAvailableSquares = new List<BoardSquare>();
                }
                catch { }
            }
        }
    }
    private List<GameObject> FindAllHitFigures(GameObject Figure)
    {
        List<GameObject> HitFigures = new List<GameObject>();
        try //remove when done
        {
            Figure.transform.GetChild(0).gameObject.SetActive(false);
        }
        catch
        {

        }
        Vectors = FindAllVectors(Figure);
        for (int j = 0; j < Vectors.Count; j++)
        {
            RaycastHit2D hit = Physics2D.Raycast(Figure.transform.position, Vectors[j], 10f, Mask);
            if (hit.collider != null)
            {
                HitFigures.Add(hit.transform.parent.gameObject);
            }
        }
        try //remove when done
        {
            Figure.transform.GetChild(0).gameObject.SetActive(true);
        }
        catch
        {

        }
        return HitFigures;
    }
    private void IfFigureIsBetweenThem(GameObject CheckFigure, GameObject PinnedFigure, GameObject King, bool ShouldBeWhite)
    {
        if (CheckFigure.TryGetComponent<King>(out King CheckingKing))
            return;
        Figure PinnedFigureComponent = null;
        Figure HelperComponent = null;
        {
            try
            {
                HelperComponent = PinnedFigure.GetComponent<Queen>();
            }
            catch { }
            if (HelperComponent != null)
            {
                PinnedFigureComponent = HelperComponent;
            }
            try
            {
                HelperComponent = PinnedFigure.GetComponent<Rook>();
            }
            catch { }
            if (HelperComponent != null)
            {
                PinnedFigureComponent = HelperComponent;
            }
            try
            {
                HelperComponent = PinnedFigure.GetComponent<Bishop>();
            }
            catch { }
            if (HelperComponent != null)
            {
                PinnedFigureComponent = HelperComponent;
            }
            try
            {
                HelperComponent = PinnedFigure.GetComponent<Horse>();

            }
            catch { }
            if (HelperComponent != null)
            {
                PinnedFigureComponent = HelperComponent;
            }
            try
            {
                HelperComponent = PinnedFigure.GetComponent<Pawn>();
            }
            catch { }
            if (HelperComponent != null)
            {
                PinnedFigureComponent = HelperComponent;
            }
        }
        if (PinnedFigureComponent.IsWhite != ShouldBeWhite)
            return;
        King KingComponent = King.GetComponent<King>();
        BoardSquare PinnedSquare = PinnedFigureComponent.CurrentSquare.GetComponent<BoardSquare>();
        BoardSquare KingSquare = KingComponent.CurrentSquare.GetComponent<BoardSquare>();
        if (CheckFigure.TryGetComponent<Rook>(out Rook CheckComponent))
        {
            BoardSquare CheckSquare = CheckComponent.CurrentSquare.GetComponent<BoardSquare>();
            if (CheckSquare.X > PinnedSquare.X)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y == PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.Y != KingSquare.Y)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y == PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.Y != KingSquare.Y)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X == PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.X != KingSquare.X)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if(CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X == PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.X != KingSquare.X)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
        }
        else if (CheckFigure.TryGetComponent<Bishop>(out Bishop CheckComponent1))
        {
            BoardSquare CheckSquare = CheckComponent1.CurrentSquare.GetComponent<BoardSquare>();
            if (CheckSquare.X > PinnedSquare.X && CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (KingSquare.X >= AvailableSquare.X || KingSquare.Y >= AvailableSquare.Y || (KingSquare.X - AvailableSquare.X != KingSquare.Y - AvailableSquare.Y))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X && CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (KingSquare.X <= AvailableSquare.X || KingSquare.Y <= AvailableSquare.Y || (KingSquare.X - AvailableSquare.X != KingSquare.Y - AvailableSquare.Y))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X && CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (KingSquare.X <= AvailableSquare.X || KingSquare.Y >= AvailableSquare.Y || (Mathf.Abs(KingSquare.X - AvailableSquare.X) != Mathf.Abs(KingSquare.Y - AvailableSquare.Y)))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X > PinnedSquare.X && CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (KingSquare.X >= AvailableSquare.X || KingSquare.Y <= AvailableSquare.Y || (Mathf.Abs(KingSquare.X - AvailableSquare.X) != Mathf.Abs(KingSquare.Y - AvailableSquare.Y)))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
        }
        else if(CheckFigure.TryGetComponent<Queen>(out Queen CheckComponent2))
        {
            BoardSquare CheckSquare = CheckComponent2.CurrentSquare.GetComponent<BoardSquare>();
            if (CheckSquare.X > PinnedSquare.X && CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if ( KingSquare.X >= AvailableSquare.X || KingSquare.Y >= AvailableSquare.Y || (KingSquare.X - AvailableSquare.X != KingSquare.Y - AvailableSquare.Y))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X && CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if ( KingSquare.X <= AvailableSquare.X || KingSquare.Y <= AvailableSquare.Y || (KingSquare.X - AvailableSquare.X != KingSquare.Y - AvailableSquare.Y))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X && CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if ( KingSquare.X <= AvailableSquare.X || KingSquare.Y >= AvailableSquare.Y || (Mathf.Abs(KingSquare.X - AvailableSquare.X) != Mathf.Abs(KingSquare.Y - AvailableSquare.Y)))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X > PinnedSquare.X && CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        //
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (KingSquare.X >= AvailableSquare.X || KingSquare.Y <= AvailableSquare.Y || (Mathf.Abs(KingSquare.X - AvailableSquare.X) != Mathf.Abs(KingSquare.Y - AvailableSquare.Y)))
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X > PinnedSquare.X)
            {
                if (KingSquare.X < PinnedSquare.X && KingSquare.Y == PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.Y != KingSquare.Y)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.X < PinnedSquare.X)
            {
                if (KingSquare.X > PinnedSquare.X && KingSquare.Y == PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.Y != KingSquare.Y)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if (CheckSquare.Y > PinnedSquare.Y)
            {
                if (KingSquare.X == PinnedSquare.X && KingSquare.Y < PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.X != KingSquare.X)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
            else if(CheckSquare.Y < PinnedSquare.Y)
            {
                if (KingSquare.X == PinnedSquare.X && KingSquare.Y > PinnedSquare.Y)
                {
                    for (int i = 0; i < PinnedFigureComponent.AvailableSquares.Length; i++)
                    {
                        if (PinnedFigureComponent.AvailableSquares[i] == null)
                            break;
                        BoardSquare AvailableSquare = PinnedFigureComponent.AvailableSquares[i].GetComponent<BoardSquare>();
                        if (AvailableSquare.X != KingSquare.X)
                        {
                            PinnedFigureComponent.AvailableSquares[i] = null;
                            PinnedFigureComponent.UnAvailableSquares.Add(AvailableSquare);
                        }
                    }
                }
            }
        }
    }
    private void MakeARay()
    {
        ClearUnavailableSquares();
        WhiteKingHitFigures = FindAllHitFigures(WhiteKing);
        BlackKingHitFigures = FindAllHitFigures(BlackKing);
        for (int i = 0; i < WhiteFigures.Count; i++)
        {
            if (WhiteFigures[i] == null)
                continue;
            HitFigures = FindAllHitFigures(WhiteFigures[i]);
            if (HitFigures.Count == 0)
                continue;
            for (int j = 0; j < HitFigures.Count; j++)
            {
                for (int o = 0; o < BlackKingHitFigures.Count; o++)
                {
                    if (BlackKingHitFigures[o] == HitFigures[j])
                    {
                        IfFigureIsBetweenThem(WhiteFigures[i], HitFigures[j], BlackKing, false);
                    }
                }
            }
        }
        for (int i = 0; i < BlackFigures.Count; i++)
        {
            if (BlackFigures[i] == null)
                continue;
            HitFigures = FindAllHitFigures(BlackFigures[i]);
            if (HitFigures.Count == 0)
                continue;
            for (int j = 0; j < HitFigures.Count; j++)
            {
                for (int o = 0; o < WhiteKingHitFigures.Count; o++)
                {
                    if (WhiteKingHitFigures[o] == HitFigures[j])
                    {
                        IfFigureIsBetweenThem(BlackFigures[i], HitFigures[j], WhiteKing, true);
                    }
                }
            }
        }
    }
}
