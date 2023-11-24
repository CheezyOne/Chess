using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
using System;
using Unity.VisualScripting;

public class DrawAndForfeit : AttributesSync
{
    [SynchronizableField] public int DrawPropositionCounter=0;
    [SerializeField] private GameObject DrawButton, ForfeitButton, EndingText;
    //public static Action onGameLeave;
    [SynchronizableField] private string ForfeitPlayer="";

    private void Start()
    {
        Figure.OnGameEnd += MakeButtonsUninteractable;
    }
    private void MakeButtonsUninteractable()
    {
        try
        {
            DrawButton.GetComponent<Button>().interactable = false;
        }
        catch { }
        try
        {
            ForfeitButton.GetComponent<Button>().interactable = false;
        }
        catch { }
    }
    [SynchronizableMethod] private void CheckIfDraw()
    {
        if (DrawPropositionCounter >= 2)
            Figure.IsDraw = true;
    }
    [SynchronizableMethod] private void DrawIsProposed()
    {
        if(DrawButton.GetComponent<Button>().interactable==true)
            DrawButton.transform.GetChild(0).GetComponent<Text>().text = "Согласиться на ничью";
    }
    public void DrawPress()
    {
        DrawButton.GetComponent<Button>().interactable = false;
        DrawPropositionCounter++;
        InvokeRemoteMethod("DrawIsProposed");
        BroadcastRemoteMethod("CheckIfDraw");
    }
    [SynchronizableMethod] public void Forfeit()
    {
        //Debug.Log("White is " + StartAGame.WhitePlayerName + " Black is " + StartAGame.BlackPlayerName + " Forfeiter is " + ForfeitPlayer);
        //Debug.Log(StartAGame.WhitePlayerName + " " + StartAGame.BlackPlayerName + " forfieter is " + ForfeitPlayer);
        if (StartAGame.WhitePlayerName == ForfeitPlayer)
        {
            Figure.WhiteIsMated = true;
        }
        else if (ForfeitPlayer == StartAGame.BlackPlayerName)
        {
            Figure.BlackIsMated = true;
        }
    }
    private IEnumerator WaitingBeforeForfeit()
    {
        yield return new WaitForSeconds(0.5f);
        BroadcastRemoteMethod("Forfeit");
    }    
    public void ForfeitPress()
    {
        ForfeitPlayer = FindObjectOfType<Multiplayer>().GetUser().Name;
        ForfeitButton.GetComponent<Button>().interactable = false;
        //BroadcastRemoteMethod("Forfeit");
        StartCoroutine(WaitingBeforeForfeit());
    }
    public void OtherPlayerLeft(Multiplayer multiplayer, User LeftUser)
    {
        if (!Figure.WhiteIsMated && !Figure.BlackIsMated && !Figure.IsDraw)
        {
            ForfeitPlayer = LeftUser.Name;
            Forfeit();
        }
    }
    public void LeaveTheRoom() //It may not work cause we've left the room
    {
        ResetFigureStaticVars();
        //onGameLeave?.Invoke();
    }
    private void ResetFigureStaticVars()
    {
        ForfeitButton.GetComponent<Button>().interactable = true;
        DrawButton.GetComponent<Button>().interactable = true;
        DrawButton.transform.GetChild(0).GetComponent<Text>().text = "Предложить ничью";
        EndingText.SetActive(false);
        EnPassantController.IsAvailable = false;
        Figure.WhiteToMove = true;
        Figure.WhiteKingIsChecked = false;
        Figure.BlackKingIsChecked = false;
        Figure.CheckingSquare = null;
        Figure.SecondCheckingSquare = null;
        Figure.KingSquare = null;
        Figure.WhiteIsMated = false;
        Figure.BlackIsMated = false;
        Figure.IsDraw = false;
        Figure.GameEndedOnce = false;
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        Destroy(StartAGame.NewGame);
    }
}
