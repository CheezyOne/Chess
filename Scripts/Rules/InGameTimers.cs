using Alteruna;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameTimers : AttributesSync
{
    [SerializeField] private float BlackTime, WhiteTime;
    [SynchronizableField] private int SyncBlackTime, SyncWhiteTime;
    [SerializeField] private GameObject WhiteTimer, BlackTimer, PinnsChecker;
    private bool IsWhiteToMove = true, GameIsOn = false, CanCount=false;
    private List<GameObject> WhiteFigures, BlackFigures;
    private int minutes, seconds, counterBlack=0, counterWhite=0;
    //Should have a link to some script, that has all the figures already
    private new void OnEnable()
    {
        StartAGame.onGameStart += StartTheTimer;
        Figure.OnKill += CheckIfTheresEnoughFiguresToContinue;
        Figure.OnGameEnd += StopTheTimer;
        Figure.OnMove += DecideWhichSideIsToMove;
        Figure.OnMove += FindAllFigures;
        FindAllFigures();
    }
    private void OnDisable()
    {
        StartAGame.onGameStart -= StartTheTimer;
        Figure.OnGameEnd -= StopTheTimer;
        Figure.OnMove -= DecideWhichSideIsToMove;
        Figure.OnMove -= FindAllFigures;
    }
    private void CheckIfTheresEnoughFiguresToContinue()
    {
        if (!HasEnoughFiguresToWin(WhiteFigures) && !HasEnoughFiguresToWin(BlackFigures))
            Figure.IsDraw = true;
    }
    private void StopTheTimer()
    {
        GameIsOn = false;
    }
    private void StartTheTimer()
    {
        GameIsOn = true;
        counterWhite =Convert.ToInt32(WhiteTime);
        counterBlack= Convert.ToInt32(BlackTime);
        if (StartAGame.WhitePlayerName == FindObjectOfType<Multiplayer>().GetUser().Name)
        {
            SyncWhiteTime = counterWhite;
            SyncBlackTime = counterBlack;
            CanCount = true;
        }
    }

    private void DecideWhichSideIsToMove()
    {
        if (Figure.WhiteToMove!= IsWhiteToMove)
        {
            IsWhiteToMove = !IsWhiteToMove;
        }
    }
    private void FindAllFigures()
    {
        WhiteFigures = PinnsChecker.GetComponent<CheckIfFigureIsPinned>().WhiteFigures;
        BlackFigures = PinnsChecker.GetComponent<CheckIfFigureIsPinned>().BlackFigures;
    }
    private bool HasEnoughFiguresToWin(List<GameObject> Figures)
    {
        int BishopCounter = 0, HorseCounter=0;
        foreach(GameObject Figure in Figures)
        {
            if (Figure.TryGetComponent<Queen>(out Queen queen) || Figure.TryGetComponent<Rook>(out Rook rook) || Figure.TryGetComponent<Pawn>(out Pawn pawn))
                return true;
            else if (Figure.TryGetComponent<Bishop>(out Bishop bishop))
                BishopCounter++;
            else if (Figure.TryGetComponent<Horse>(out Horse horse))
                HorseCounter++;
            if (BishopCounter > 1)
                return true;
            else if (BishopCounter >= 1 && HorseCounter >= 1)
                return true;
            else if (HorseCounter > 2)
                return true;
        }
        //Check all the figures to find anything more than 2 horses
        return false;
    }
    [SynchronizableMethod] private void WhiteWonOnTime()
    {
        Figure.BlackIsMated = true;
    }
    [SynchronizableMethod] private void BlackWonOnTime()
    {
        Figure.WhiteIsMated = true;
    }
    [SynchronizableMethod] private void DrawOnTime()
    {
        Figure.IsDraw = true;
    }
    private void Update()
    {
        if (!GameIsOn)
            return;
        if (IsWhiteToMove)
        {
            if (WhiteTime <= 0)
            {
                WhiteTimer.GetComponent<Text>().text = "0:0";
                if (HasEnoughFiguresToWin(BlackFigures))
                {
                    BroadcastRemoteMethod("BlackWonOnTime");
                    return;
                }
                BroadcastRemoteMethod("DrawOnTime");
                return;
            }
             if(CanCount)
                WhiteTime -= Time.deltaTime;
            if (Convert.ToDouble(counterWhite)-WhiteTime > 1)
            {
                SyncWhiteTime--;
                counterWhite--;
            }
            minutes = Convert.ToInt32((Convert.ToDouble(SyncWhiteTime+1) - 30) / 60);
            seconds=Convert.ToInt32(Convert.ToDouble(SyncWhiteTime) % 60);
            if (minutes == 10)
                WhiteTimer.GetComponent<Text>().text = "9:59"; //Do not let the timer be above 10 minutes cause it becomes broken otherwise
            else
            {
                if (seconds == 60)
                    seconds = 0;
                else
                    WhiteTimer.GetComponent<Text>().text = Convert.ToString(minutes) + ":" + Convert.ToString(seconds);
            }
        }
        else
        {
            if (BlackTime <= 0)
            {
                BlackTimer.GetComponent<Text>().text = "0:0";
                if (HasEnoughFiguresToWin(WhiteFigures))
                {
                    BroadcastRemoteMethod("WhiteWonOnTime");
                    return;
                }
                BroadcastRemoteMethod("DrawOnTime");
                return;
            }
            if (CanCount)
                BlackTime -= Time.deltaTime;
            if (Convert.ToDouble(counterBlack) - BlackTime > 1)
            {
                SyncBlackTime--;
                counterBlack--;
            }
            minutes = Convert.ToInt32((Convert.ToDouble(SyncBlackTime+1) - 30) / 60);
            seconds = Convert.ToInt32(Convert.ToDouble(SyncBlackTime) % 60);
            if (minutes == 10)
            {
                BlackTimer.GetComponent<Text>().text = "9:59"; //Do not let the timer be above 10 minutes cause it becomes broken otherwise
            }
            else
            {
                if (seconds == 60)
                    seconds = 0;
                else
                    BlackTimer.GetComponent<Text>().text = Convert.ToString(minutes) + ":" + Convert.ToString(seconds);
            }

        }
    }
}
