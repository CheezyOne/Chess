using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnBoardForBlack : MonoBehaviour
{
    private void OnEnable()
    {
        StartAGame.onGameStart += TurnTheBoard;
    }
    private void OnDisable()
    {
        StartAGame.onGameStart -= TurnTheBoard;
    }

    private void TurnTheBoard()
    {
        Multiplayer multiplayer = FindObjectOfType<Multiplayer>();
        if (multiplayer.GetUser().Name != StartAGame.BlackPlayerName)
            return;
        Camera.main.transform.rotation= Quaternion.Euler(0, 0, 180);
        GameObject GameBoard = GameObject.FindGameObjectWithTag("Game");
        GameBoard.transform.Find("Canvas").Find("Timers").transform.rotation = Quaternion.Euler(0, 0, 180);
        GameBoard.transform.Find("Canvas").Find("Canvas").Find("NewBlackFigureButtons").transform.rotation= Quaternion.Euler(0, 0, 180);
        Vector3 WhiteTimerPosition = GameBoard.transform.Find("Canvas").Find("Timers").Find("WhiteTimer").position;
        GameBoard.transform.Find("Canvas").Find("Timers").Find("WhiteTimer").position = GameBoard.transform.Find("Canvas").Find("Timers").Find("BlackTimer").position;
        GameBoard.transform.Find("Canvas").Find("Timers").Find("BlackTimer").position = WhiteTimerPosition;
        Transform WhitePieces = GameBoard.transform.Find("WhitePieces");
        for (int i= 0; i< WhitePieces.childCount;i++)
        {
            WhitePieces.GetChild(i).rotation= Quaternion.Euler(0, 0, 180);
        }
        Transform BlackPieces = GameBoard.transform.Find("BlackPieces");
        for (int i = 0; i < BlackPieces.childCount; i++)
        {
            BlackPieces.GetChild(i).rotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
