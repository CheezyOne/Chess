using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Alteruna;
using System.Runtime.CompilerServices;

public class Figure : Synchronizable
{
    public static Action OnGameEnd;
    [SynchronizableField] public GameObject CurrentSquare;
    [SerializeField] private bool IsButton = false;
    public static bool WhiteToMove = true, WhiteKingIsChecked = false, BlackKingIsChecked = false;
    public static Action OnMove, OnKill, OnPawnMove, OnNewFigureSpawn, ChangeNewFigureSync;
    public bool AvailableMarksAreActive = false, GameIsOn = false;
    private protected int AvailableSquaresCounter = 0, OccupiedSquaresCount = 0, DangerForEnemyKingCount = 0;
    public GameObject NextSquare;
    public GameObject[] AllSquares;
    [SerializeField] private protected GameObject[] AllMarks;
    private protected BoardSquare[] BoardSquareComponents = new BoardSquare[64];
    private protected BoardSquare BoardSquareComponent;
    public GameObject[] AvailableSquares;
    public GameObject[] DangerForEnemyKing;
    public List<BoardSquare> UnAvailableSquares;
    [SerializeField] private protected GameObject[] OccupiedSquares;
    private protected BoardSquare OccupiedSquareComponent;//Check this one
    private bool IsMoving = false;
    [SerializeField] private float LerpingSpeed = 10f; //Will be used for opponents moves
    private float ScaleSize;
    [SerializeField] private float NormalScaleSize = 2f;
    private protected int SquareX, SquareY;
    public static GameObject CheckingSquare = null, SecondCheckingSquare = null, KingSquare = null;
    public static bool WhiteIsMated = false, BlackIsMated = false, IsDraw = false, GameEndedOnce = false;
    private protected bool CoverKingOnce = false;
    public bool IsWhite = true;
    [Header("Pawns")]
    public static bool IsChoosingPawn = false;
    [SerializeField] private GameObject NewFiguresBar, NewBlackFiguresBar;
    [SerializeField] private GameObject PinnChecker;
    [SerializeField] private GameObject CanvasForFigures;
    [Header("Buttons")]
    [SerializeField] private GameObject NewFigure;
    [SerializeField] private int NewFigureNumber;
    public static GameObject PawnLastSquare;
    [SerializeField] private GameObject EnemyKing;
    [SerializeField] private GameObject TheGame;
    private void Awake()
    {

        StartAGame.onGameStart += LetFiguresGo;
        if (IsButton)
            return;
        if (CurrentSquare != null)
        {
            BoardSquareComponent = CurrentSquare.GetComponent<BoardSquare>();
            BoardSquareComponent.Occupied = true;
            SetCoordinates();
            for (int i = 0; i < 64; i++)
            {
                BoardSquareComponents[i] = AllSquares[i].GetComponent<BoardSquare>();
            }
        }
        if (TryGetComponent<King>(out King king))
        {
            StartCoroutine(CheckMatePat());
        }
        ScaleSize = NormalScaleSize + NormalScaleSize * 0.2f;
    }
    private void Start()
    {
        StartCoroutine(UntagAndStartFigure());
    }
    private IEnumerator UntagAndStartFigure()
    {
        yield return new WaitForSeconds(0.3f);
        if(tag=="NewFigure")
        tag = "Untagged";
        FindAllAvailableSquares();
    }

    private protected void OnMoveFunction() //Using this to wait for 0.05 second until enpassant is defined for sure
    {
        StartCoroutine(CoroutineForOneFrame());
    }
    private IEnumerator CoroutineForOneFrame() //Waiting here
    {
        yield return new WaitForSeconds(0.05f);
        FindAllAvailableSquares();
        IsCheckingEnemyKing();
    }
    private void SetCoordinates()
    {
        SquareX = BoardSquareComponent.X;
        SquareY = BoardSquareComponent.Y;
        BoardSquareComponent.Occupied = true;
        BoardSquareComponent.FigureOnTheSpot = gameObject;
    }
    private void LetFiguresGo()
    {
        GameIsOn = true;
    }
    public void MarksOnAvailvableSpots() //Usable squares
    {
        for (int i = 0; i < AvailableSquares.Length; i++)
        {
            if (AvailableSquares[i] == null)
                return;
            for (int j = 0; j < AllSquares.Length; j++)
            {
                if (AvailableSquares[i] == AllSquares[j])
                {
                    AllMarks[j].gameObject.SetActive(true);
                }
            }
        }
    }

    private void DeleteMarks()
    {
        for (int i = 0; i < AllMarks.Length; i++)
        {
            AllMarks[i].gameObject.SetActive(false);
        }
    }
    public virtual void FindAllAvailableSquares() //override in children
    {

    }
    public void SelectFigure() //Click selection of the figure
    {
        if (WhiteToMove != IsWhite)
            return;
        IsMoving = true;
        if (TryGetComponent<King>(out King king))
            DontGoAlongCheck();
    }
    private void MoveFigureAfterMouse() //Moving the figure with the mouse
    {
        transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
    }
    private void SetOrderInLayer(int Order)
    {
        GetComponent<SpriteRenderer>().sortingOrder = Order;
    }
    private void ScaleFigure(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }
    public void MoveToNextSquare()  //Lerp to next square for opponents perspective
    {
        transform.position = Vector2.Lerp(transform.position, NextSquare.transform.position, LerpingSpeed * Time.deltaTime);
    }

    private void DestroyEnemyPiece(int i)
    {
        if (AvailableSquares[i].GetComponent<BoardSquare>().Occupied)
        {
            if (AvailableSquares[i].GetComponent<BoardSquare>().FigureOnTheSpot == null)
                return;
            if (AvailableSquares[i].GetComponent<BoardSquare>().FigureOnTheSpot.TryGetComponent<King>(out King king))
            {
                if (king.IsWhite)
                    WhiteIsMated = true;
                else
                    BlackIsMated = true;
            }
            Destroy(AvailableSquares[i].GetComponent<BoardSquare>().FigureOnTheSpot);
            OnKill?.Invoke();
        }


    }
    private void Castling(King KingComponent) //Check if king is 2 squares away from his original position
    {
        if (KingComponent.BoardSquareComponent.X == 2)
        {
            for (int i = 0; i < AllSquares.Length; i++)
            {
                if (AllSquares[i] == KingComponent.CurrentSquare)
                {
                    KingComponent.LeftRook.transform.position = AllSquares[i + 1].transform.position;
                    Rook CastledRook = KingComponent.LeftRook.GetComponent<Rook>();
                    CastledRookNewPosition(CastledRook, true, i);
                }
            }
        }
        else if (KingComponent.BoardSquareComponent.X == 6)
        {
            for (int i = 0; i < AllSquares.Length; i++)
            {
                if (AllSquares[i] == KingComponent.CurrentSquare)
                {
                    KingComponent.RightRook.transform.position = AllSquares[i - 1].transform.position;
                    Rook CastledRook = KingComponent.RightRook.GetComponent<Rook>();
                    CastledRookNewPosition(CastledRook, false, i);
                }
            }
        }
    }
    private void CastledRookNewPosition(Rook CastledRook, bool Left, int i)
    {
        if (Left)
        {
            CastledRook.CurrentSquare = AllSquares[i + 1];
            CastledRook.BoardSquareComponent.Occupied = false;
            CastledRook.BoardSquareComponent.FigureOnTheSpot = null;
            CastledRook.BoardSquareComponent = AllSquares[i + 1].GetComponent<BoardSquare>();
        }
        else
        {
            CastledRook.CurrentSquare = AllSquares[i - 1];
            CastledRook.BoardSquareComponent.Occupied = false;
            CastledRook.BoardSquareComponent.FigureOnTheSpot = null;
            CastledRook.BoardSquareComponent = AllSquares[i - 1].GetComponent<BoardSquare>();
        }
        CastledRook.BoardSquareComponent.Occupied = true;
        CastledRook.BoardSquareComponent.FigureOnTheSpot = gameObject;
        CastledRook.SetCoordinates();
        CastledRook.FindAllAvailableSquares();
        CastledRook.IsCheckingEnemyKing();
    }
    private void PawnClickOnLastPosition() //Same as below, just for pawns
    {
        IsMoving = false;
        for (int i = 0; i < AvailableSquares.Length; i++) //Finding if clicked on an available square
        {
            if (AvailableSquares[i] == null)
                break;
            if (Mathf.Abs(transform.position.x - AvailableSquares[i].transform.position.x) < 0.5f && Mathf.Abs(transform.position.y - AvailableSquares[i].transform.position.y) < 0.5f)
            {
                IsChoosingPawn = true;
                BoardSquareComponent.Occupied = false;
                BoardSquareComponent.FigureOnTheSpot = null;
                transform.position = AvailableSquares[i].transform.position;
                CurrentSquare = AvailableSquares[i];
                BoardSquareComponent = CurrentSquare.GetComponent<BoardSquare>();
                BoardSquareComponent.Occupied = true;
                DestroyEnemyPiece(i);
                BoardSquareComponent.FigureOnTheSpot = gameObject;
                SetCoordinates();
                DeleteMarks();
                NetworkNewPosition();
                AvailableMarksAreActive = false;
                if (WhiteKingIsChecked)
                {
                    WhiteKingIsChecked = !WhiteKingIsChecked;
                }
                if (BlackKingIsChecked)
                {
                    BlackKingIsChecked = !BlackKingIsChecked;
                }
                OnMove?.Invoke();
                return;
            }
        }
        DeleteMarks();
        AvailableMarksAreActive = false;
        transform.position = CurrentSquare.transform.position;// If none are available - return back to current
    }
    public void PawnClickOnLastPosition(GameObject NewPosition, int i) //For FigurePositionHolder
    {
        IsMoving = false;
        IsChoosingPawn = true;
        BoardSquareComponent.Occupied = false;
        BoardSquareComponent.FigureOnTheSpot = null;
        transform.position = NewPosition.transform.position;
        CurrentSquare = NewPosition;
        PawnLastSquare = CurrentSquare;
        BoardSquareComponent = CurrentSquare.GetComponent<BoardSquare>();
        BoardSquareComponent.Occupied = true;
        DestroyEnemyPiece(i);
        BoardSquareComponent.FigureOnTheSpot = gameObject;
        SetCoordinates();
        DeleteMarks();
        NetworkNewPosition();
        AvailableMarksAreActive = false;
        if (WhiteKingIsChecked)
        {
            WhiteKingIsChecked = !WhiteKingIsChecked;
        }
        if (BlackKingIsChecked)
        {
            BlackKingIsChecked = !BlackKingIsChecked;
        }
        //OnMove?.Invoke();
    }
    private void NetworkNewPosition()
    {
        for (int i = 0; i < AllSquares.Length; i++)
        {
            if (AllSquares[i] == GetComponent<Figure>().CurrentSquare)
            {
                try
                {
                    GetComponent<FigurePositionHolder>().NetworkPosition = i;
                    GetComponent<FigurePositionHolder>().MovedFigure = name;
                }
                catch { }
                break;
            }
        }
    }
    public void AssignNewPosition(GameObject NewPosition, int i)
    {
        BoardSquareComponent.Occupied = false;
        BoardSquareComponent.FigureOnTheSpot = null;
        transform.position = NewPosition.transform.position;
        CurrentSquare = NewPosition;
        BoardSquareComponent = CurrentSquare.GetComponent<BoardSquare>();
        BoardSquareComponent.Occupied = true;
        DestroyEnemyPiece(i);
        BoardSquareComponent.FigureOnTheSpot = gameObject;
        SetCoordinates();
        FindAllAvailableSquares();
        DeleteMarks();
        AvailableMarksAreActive = false;
        NetworkNewPosition();
        WhiteToMove = !WhiteToMove;
        if (WhiteKingIsChecked)
        {
            WhiteKingIsChecked = !WhiteKingIsChecked;
        }
        if (BlackKingIsChecked)
        {
            BlackKingIsChecked = !BlackKingIsChecked;
        }
        IsCheckingEnemyKing();
        if (TryGetComponent<Pawn>(out Pawn Pawn))
        {
            if (CurrentSquare == EnPassantController.EnPassantedSquare)
            {
                BoardSquare MovedPawnSquareComponent = EnPassantController.MovedPawn.GetComponent<Pawn>().CurrentSquare.GetComponent<BoardSquare>();
                MovedPawnSquareComponent.Occupied = false;
                MovedPawnSquareComponent.FigureOnTheSpot = null;
                Destroy(EnPassantController.MovedPawn);
            }

        }
        else if (TryGetComponent<King>(out King King))
        {
            if (King.CanCastle)
            {
                Castling(King);
                King.CanCastle = false;
            }
        }
        else if (TryGetComponent<Rook>(out Rook Rook))
        {
            Rook.CanCastle = false;
        }
        OnMove?.Invoke();
        if (TryGetComponent<Pawn>(out Pawn pawn))
        {
            if (pawn.MoveMultiplier == 2)
            {
                pawn.MoveMultiplier = 1;
                EnPassantController.IsAvailable = true;
                EnPassantController.MovedPawn = gameObject;
            }
        }
        else
        {
            EnPassantController.IsAvailable = false;
        }
        OnMove?.Invoke();
    }

    private int FindCurrentSquareNumber(GameObject CurrentSquare)
    {
        for (int i=0;i<AllSquares.Length;i++)
        {
            if (AllSquares[i] == CurrentSquare)
                return i;
        }
        return 0;
    }
    private void ClickOnNewPosition() //Constantly checkig for clicks
    {
        IsMoving = false;
        for (int i = 0; i < AvailableSquares.Length; i++) //Finding if clicked on an available square
        {
            if (AvailableSquares[i] == null)
                break;
            if (Mathf.Abs(transform.position.x - AvailableSquares[i].transform.position.x) < 0.5f && Mathf.Abs(transform.position.y - AvailableSquares[i].transform.position.y) < 0.5f)
            {
                GetComponent<FigurePositionHolder>().NetworkColorFirstSquare = FindCurrentSquareNumber(CurrentSquare);
                GetComponent<FigurePositionHolder>().NetworkColorSecondSquare = FindCurrentSquareNumber(AvailableSquares[i]);
                AssignNewPosition(AvailableSquares[i], i);
                return;
            }
        }
        //Debug.Log("ASDFAD");
        //StartCoroutine(FailedNewPosition());
        DeleteMarks();
        AvailableMarksAreActive = false;
        transform.position = CurrentSquare.transform.position;// If none are available - return back to current
    }
    private IEnumerator FailedNewPosition()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = CurrentSquare.transform.position;
        StopCoroutine(FailedNewPosition());
    }
    private protected void IsCheckingEnemyKing()
    {
        for (int i = 0; i < AvailableSquares.Length; i++)
        {
            if (AvailableSquares[i] == null)
                return;
            if (AvailableSquares[i].GetComponent<BoardSquare>().Occupied)
            {
                if (AvailableSquares[i].GetComponent<BoardSquare>().FigureOnTheSpot.TryGetComponent<King>(out King king))
                {
                    KingSquare = king.CurrentSquare;
                    if (IsWhite && !king.IsWhite)
                    {
                        if (CheckingSquare == null)
                            CheckingSquare = CurrentSquare;
                        else if (CheckingSquare != CurrentSquare)
                            SecondCheckingSquare = CurrentSquare;
                        BlackKingIsChecked = true;
                    }
                    else if (!IsWhite && king.IsWhite)
                    {
                        if (CheckingSquare == null)
                            CheckingSquare = CurrentSquare;
                        else if (CheckingSquare != CurrentSquare)
                            SecondCheckingSquare = CurrentSquare;
                        WhiteKingIsChecked = true;
                    }
                    return;
                }
            }
        }
    } //If King is Checked
    private void DontGoAlongCheck()
    {
        //BoardSquare SecondCheckingPosition;
        if (CheckingSquare == null)
            return;
        BoardSquare KingPosition = CurrentSquare.GetComponent<BoardSquare>();
        BoardSquare CheckingPosition = CheckingSquare.GetComponent<BoardSquare>();
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < AvailableSquaresCounter; i++)
            {
                //Debug.Log(CheckingPosition.X + " " + CheckingPosition.Y +" "+ KingPosition.X + " " + KingPosition.Y);
                if (AvailableSquares[i] == null)
                    continue;
                BoardSquare AvailablePosition = AvailableSquares[i].GetComponent<BoardSquare>();
                if (KingPosition.X == CheckingPosition.X && KingPosition.Y < CheckingPosition.Y)
                {
                    if (AvailablePosition.Y < KingPosition.Y && AvailablePosition.X == KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //King Vertical  up
                }
                if (KingPosition.X == CheckingPosition.X && KingPosition.Y > CheckingPosition.Y)
                {
                    if (AvailablePosition.Y > KingPosition.Y && AvailablePosition.X == KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //Vertical  down
                }
                if (KingPosition.X > CheckingPosition.X && KingPosition.Y == CheckingPosition.Y)
                {
                    if (AvailablePosition.Y == KingPosition.Y && AvailablePosition.X > KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //Right
                }
                if (KingPosition.X < CheckingPosition.X && KingPosition.Y == CheckingPosition.Y)
                {
                    if (AvailablePosition.Y == KingPosition.Y && AvailablePosition.X < KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //Left
                }
                if (KingPosition.Y > CheckingPosition.Y && KingPosition.X > CheckingPosition.X)
                {
                    if (AvailablePosition.Y > KingPosition.Y && AvailablePosition.X > KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //King is Down right from Check
                }
                if (KingPosition.Y > CheckingPosition.Y && KingPosition.X < CheckingPosition.X)
                {
                    if (AvailablePosition.Y > KingPosition.Y && AvailablePosition.X < KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //King is Down left from Check
                }
                if (KingPosition.Y < CheckingPosition.Y && KingPosition.X > CheckingPosition.X)
                {
                    if (AvailablePosition.Y < KingPosition.Y && AvailablePosition.X > KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //King is up right from Check
                }
                if (KingPosition.Y < CheckingPosition.Y && KingPosition.X < CheckingPosition.X)
                {
                    if (AvailablePosition.Y < KingPosition.Y && AvailablePosition.X < KingPosition.X)
                    {
                        AvailableSquares[i] = null;
                    }
                    //King is up left from Check
                }
            }
            if (SecondCheckingSquare != null)
            {
                //Debug.Log(SecondCheckingSquare);
                CheckingPosition = SecondCheckingSquare.GetComponent<BoardSquare>();
            }
            else
                break;
        }
        int Counter = 0;
        for (int i = 0; i < AvailableSquares.Length; i++)
        {
            if (AvailableSquares[i] != null)
            {
                AvailableSquares[Counter] = AvailableSquares[i];
                Counter++;
            }
        }
        return;
    }
    private void CoverTheKing()
    {
        if (TryGetComponent<King>(out King king))
        {
            bool stop = false;
            if (stop)
                return;
            for (int i = 0; i < king.EnemyPieces.Count; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<Bishop>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<King>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<Queen>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }

                    catch { }
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<Horse>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<Rook>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (king.EnemyPieces[i].GetComponent<Pawn>().AvailableSquares[j] != null)
                        {
                            stop = true;
                            break;
                        }
                    }
                    catch { }
                }
                if (stop)
                    break;
            }
            if (!stop && (WhiteKingIsChecked || BlackKingIsChecked))
            {
                if (WhiteKingIsChecked)
                    WhiteIsMated = true;
                else
                    BlackIsMated = true;
            }
            else if (!stop && WhiteToMove != IsWhite)
                IsDraw = true;
            DontGoAlongCheck();
            return;
        }
        if (CheckingSquare != null && SecondCheckingSquare != null)
        {
            for (int i = 0; i < AvailableSquares.Length; i++)
            {
                AvailableSquares[i] = null;
            }
            return;
        }
        BoardSquare KingsBoardSquare = KingSquare.GetComponent<BoardSquare>();
        BoardSquare CheckingBoardSquare = CheckingSquare.GetComponent<BoardSquare>();
        for (int i = 0; i < AvailableSquaresCounter; i++)
        {
            if (AvailableSquares[i] == null)
                continue;
            AvailableSquares[i].TryGetComponent<BoardSquare>(out BoardSquare AvailableSquare);
            if (TryGetComponent<Pawn>(out Pawn pawn)) //If pawn can enPassantCheckingPawn
            {
                if (IsWhite)
                {
                    if (pawn.AvailableForEnPassant(i) && AvailableSquare.Y - CheckingBoardSquare.Y == -1)
                    {
                        continue;
                    }
                }
                else
                {
                    if (pawn.AvailableForEnPassant(i) && AvailableSquare.Y - CheckingBoardSquare.Y == 1)
                    {
                        continue;
                    }
                }
            }
            if (AvailableSquare.X == CheckingBoardSquare.X && AvailableSquare.Y == CheckingBoardSquare.Y)
            {
                continue;
            }
            else if (KingsBoardSquare.X == CheckingBoardSquare.X && KingsBoardSquare.Y < CheckingBoardSquare.Y)
            {
                if (AvailableSquare.Y < CheckingBoardSquare.Y && AvailableSquare.Y > KingsBoardSquare.Y && AvailableSquare.X == KingsBoardSquare.X)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Vertical  up
            }
            else if (KingsBoardSquare.X == CheckingBoardSquare.X && KingsBoardSquare.Y > CheckingBoardSquare.Y)
            {
                if (AvailableSquare.Y > CheckingBoardSquare.Y && AvailableSquare.Y < KingsBoardSquare.Y && AvailableSquare.X == KingsBoardSquare.X)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Vertical  down
            }
            else if (KingsBoardSquare.Y == CheckingBoardSquare.Y && KingsBoardSquare.X < CheckingBoardSquare.X)
            {
                if (AvailableSquare.X < CheckingBoardSquare.X && AvailableSquare.X > KingsBoardSquare.X && AvailableSquare.Y == KingsBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Horizontal  Left
            }
            else if (KingsBoardSquare.Y == CheckingBoardSquare.Y && KingsBoardSquare.X > CheckingBoardSquare.X)
            {
                if (AvailableSquare.X > CheckingBoardSquare.X && AvailableSquare.X < KingsBoardSquare.X && AvailableSquare.Y == KingsBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Horizontal  Right
            }
            else if (KingsBoardSquare.Y > CheckingBoardSquare.Y && KingsBoardSquare.X > CheckingBoardSquare.X)
            {
                if (!(Mathf.Abs(CheckingBoardSquare.X - AvailableSquare.X) == Mathf.Abs(CheckingBoardSquare.Y - AvailableSquare.Y)))
                {
                    AvailableSquares[i] = null;
                }
                if (AvailableSquare.X < KingsBoardSquare.X && AvailableSquare.Y < KingsBoardSquare.Y && AvailableSquare.X > CheckingBoardSquare.X && AvailableSquare.Y > CheckingBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //King is Down right from Check
            }
            else if (KingsBoardSquare.Y > CheckingBoardSquare.Y && KingsBoardSquare.X < CheckingBoardSquare.X)
            {
                if (!(Mathf.Abs(CheckingBoardSquare.X - AvailableSquare.X) == Mathf.Abs(CheckingBoardSquare.Y - AvailableSquare.Y)))
                {
                    AvailableSquares[i] = null;
                }
                if (AvailableSquare.X > KingsBoardSquare.X && AvailableSquare.Y < KingsBoardSquare.Y && AvailableSquare.X < CheckingBoardSquare.X && AvailableSquare.Y > CheckingBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Down left
            }
            else if (KingsBoardSquare.Y < CheckingBoardSquare.Y && KingsBoardSquare.X > CheckingBoardSquare.X)
            {
                if (!(Mathf.Abs(CheckingBoardSquare.X - AvailableSquare.X) == Mathf.Abs(CheckingBoardSquare.Y - AvailableSquare.Y)))
                {
                    AvailableSquares[i] = null;
                }
                if (AvailableSquare.X < KingsBoardSquare.X && AvailableSquare.Y > KingsBoardSquare.Y && AvailableSquare.X > CheckingBoardSquare.X && AvailableSquare.Y < CheckingBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Up right
            }
            else if (KingsBoardSquare.Y < CheckingBoardSquare.Y && KingsBoardSquare.X < CheckingBoardSquare.X)
            {
                if (!(Mathf.Abs(CheckingBoardSquare.X - AvailableSquare.X) == Mathf.Abs(CheckingBoardSquare.Y - AvailableSquare.Y)))
                {
                    AvailableSquares[i] = null;
                }
                if (AvailableSquare.X > KingsBoardSquare.X && AvailableSquare.Y > KingsBoardSquare.Y && AvailableSquare.X < CheckingBoardSquare.X && AvailableSquare.Y < CheckingBoardSquare.Y)
                {
                    continue;
                }
                AvailableSquares[i] = null;
                //Up left
            }
            //if (AvailableSquares[i].GetComponent<BoardSquare>().X)
        }
        int Counter = 0;
        for (int i = 0; i < AvailableSquares.Length; i++)
        {
            if (AvailableSquares[i] != null)
            {
                AvailableSquares[Counter] = AvailableSquares[i];
                Counter++;
            }
        }
    } //Try to Block enemy figure from capturing the king
    private IEnumerator CheckMatePat()
    {
        yield return new WaitForSeconds(1f);
        if (TryGetComponent<King>(out King king))
            CoverTheKing();
        yield return CheckMatePat();
    } //Every second checking for mates
    public virtual bool PawnReachedTheEndingSquare()
    {
        return false;
    }
    public virtual bool PawnIsGoingToReachTheLastSquare()
    {
        return false;
    }

    private void ShowNewFiguresVariants()
    {
        if (WhiteToMove)
        {
            NewFiguresBar.gameObject.SetActive(true);
            NewFiguresBar.transform.position = new Vector2(gameObject.transform.position.x + 0.95f, gameObject.transform.position.y - 1.65f);
        }
        else
        {
            NewBlackFiguresBar.gameObject.SetActive(true);
            NewBlackFiguresBar.transform.position = new Vector2(gameObject.transform.position.x + 0.95f, gameObject.transform.position.y - 1.65f);
        }
        CanvasForFigures.gameObject.SetActive(true);
    }
    private void CoverTheKings()
    {
        if (IsWhite && WhiteKingIsChecked && !CoverKingOnce)
        {
            CoverKingOnce = true;
            CoverTheKing();
        }
        else if (!IsWhite && BlackKingIsChecked && !CoverKingOnce)
        {
            CoverKingOnce = true;
            CoverTheKing();
        }
    }
    private void CheckForMateOrDraw()
    {
        if (WhiteIsMated || BlackIsMated || IsDraw)
        {
            if (!GameEndedOnce)
            {
                GameEndedOnce = true;
                OnGameEnd?.Invoke();
            }
            ScaleFigure(NormalScaleSize);
        }
    }
    private bool CheckPlayersColor()
    {
        Multiplayer multiplayer = FindObjectOfType<Multiplayer>();
        //Debug.Log(multiplayer.GetUser().Name + " " + StartAGame.WhitePlayerName + " " + StartAGame.BlackPlayerName);
        if (multiplayer.GetUser().Name == StartAGame.WhitePlayerName && IsWhite)
            return true;
        else if (multiplayer.GetUser().Name == StartAGame.BlackPlayerName && !IsWhite)
            return true;
        return false;
    }
    private void Update()
    {
        if (!GameIsOn)
            return;

        if (IsButton || IsChoosingPawn)
            return;
        CoverTheKings();
        CheckForMateOrDraw();
        if (!IsMoving)
        {
            ScaleFigure(NormalScaleSize);
            SetOrderInLayer(3);
            return;
        }
        if (WhiteToMove != IsWhite)
            return;

        if (!CheckPlayersColor())
            return;


        if (!AvailableMarksAreActive)
        {
            MarksOnAvailvableSpots();
        }
        AvailableMarksAreActive = true;
        ScaleFigure(ScaleSize);
        SetOrderInLayer(5);
        MoveFigureAfterMouse();
        if (Input.GetMouseButtonUp(0)) //When releasing a button- check for block beneath
        {
            if (CheckingSquare != null)
                CheckingSquare = null;
            if (SecondCheckingSquare != null)
                SecondCheckingSquare = null;
            if (TryGetComponent<Pawn>(out Pawn pawn))
            {
                OnPawnMove?.Invoke();
                if ((IsWhite && pawn.CurrentSquare.GetComponent<BoardSquare>().Y == 1) || (!IsWhite && pawn.CurrentSquare.GetComponent<BoardSquare>().Y == 6))
                    PawnClickOnLastPosition();
                else
                    ClickOnNewPosition();
                if (PawnReachedTheEndingSquare())
                {
                    ShowNewFiguresVariants();
                }
                return;//Maybe just delete it later
            }
            ClickOnNewPosition();
        }
    }
    private protected bool IsNotAvailable(int i)
    {
        for (int j = 0; j < UnAvailableSquares.Count; j++)
        {
            if (BoardSquareComponents[i] == UnAvailableSquares[j])
                return true;
        }
        return false;
    }

    public void CreateNewFigureForButtons()
    {
        FigurePositionHolder.NewFigureToCreate = NewFigureNumber;
        ChangeNewFigureSync?.Invoke();
        StartCoroutine(WaitingBeforeNewFigureCreation());
    }
    private IEnumerator WaitingBeforeNewFigureCreation()
    {
        yield return new WaitForSeconds(0.2f);
        OnNewFigureSpawn?.Invoke();
    }
    public void AssignNewFigure(GameObject newFigure)
    {
        newFigure.transform.SetParent(TheGame.transform);
        newFigure.transform.localScale = new Vector3(2,2,1);
        Multiplayer multiplayer = FindObjectOfType<Multiplayer>();
        if (multiplayer.GetUser().Name == StartAGame.BlackPlayerName)
            newFigure.transform.rotation = Quaternion.Euler(0, 0, 180);
        Figure newFigureComponent = newFigure.GetComponent<Figure>();
        newFigureComponent.CurrentSquare = PawnLastSquare;
        newFigureComponent.AllMarks = AllMarks;
        newFigureComponent.AllSquares = AllSquares;
        for (int i = 0; i < 64; i++)
        {
            newFigureComponent.BoardSquareComponents[i] = AllSquares[i].GetComponent<BoardSquare>();
        }
        newFigure.transform.position = PawnLastSquare.transform.position;
        newFigureComponent.BoardSquareComponent = PawnLastSquare.GetComponent<BoardSquare>();
        newFigureComponent.BoardSquareComponent.Occupied = true;
        newFigureComponent.BoardSquareComponent.FigureOnTheSpot = newFigure;
        newFigureComponent.SetCoordinates();
        if(IsWhite)
        {
            EnemyKing = HelperNewFigure.StatBlackKing;
        }
        else
        {
            EnemyKing = HelperNewFigure.StatWhiteKing;
        }
        CanvasForFigures = HelperNewFigure.StatMainCanvas;
        NewBlackFiguresBar = HelperNewFigure.StatBlackFigureCanvas;
        NewFiguresBar = HelperNewFigure.StatWhiteFiguresCanvas;
        PinnChecker = HelperNewFigure.StatPinnChecker;
        EnemyKing.GetComponent<King>().EnemyPieces.Add(newFigure);
        if (WhiteToMove)
        {
            PinnChecker.GetComponent<CheckIfFigureIsPinned>().WhiteFigures.Add(newFigure);
        }
        else
        {
            PinnChecker.GetComponent<CheckIfFigureIsPinned>().BlackFigures.Add(newFigure);
        }
        newFigureComponent.FindAllAvailableSquares();
        newFigureComponent.DeleteMarks();
        newFigureComponent.AvailableMarksAreActive = false;
        newFigureComponent.IsCheckingEnemyKing();
        IsChoosingPawn = false;
        WhiteToMove = !WhiteToMove;
        CanvasForFigures.gameObject.SetActive(false);
        NewBlackFiguresBar.gameObject.SetActive(false);
        NewFiguresBar.gameObject.SetActive(false);
        newFigureComponent.GameIsOn = true;
        /*
        int NewFigureNumber = 0;
        if (!IsWhite)
            NewFigureNumber += 4;
        if (newFigure.TryGetComponent<Bishop>(out Bishop Bishop))
            NewFigureNumber++;
        else if (newFigure.TryGetComponent<Rook>(out Rook Rook))
            NewFigureNumber += 2;
        else if (newFigure.TryGetComponent<Queen>(out Queen Queen))
            NewFigureNumber += 3;
        FigurePositionHolder.NewFigureToCreate = NewFigureNumber;
        */
        OnMove?.Invoke();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        throw new NotImplementedException();
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        throw new NotImplementedException();
    }
}





