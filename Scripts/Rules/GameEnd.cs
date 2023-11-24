using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
public class GameEnd : AttributesSync
{
    public GameObject EndingText, ExitButton, ForfeitButton, DrawButton;
    private void Start()
    {
        Figure.OnGameEnd += GameEnded;
    }
    private void GameEnded()
    {
        //FindObjectOfType<Multiplayer>().LockRoom();
        BroadcastRemoteMethod("ShowText");
        BroadcastRemoteMethod("HandleButtons");
    }

    [SynchronizableMethod] private void HandleButtons()
    {

        ExitButton.transform.position = Camera.main.transform.position;
        ForfeitButton.GetComponent<Button>().interactable = false;
        DrawButton.GetComponent<Button>().interactable = false;
    }
    [SynchronizableMethod] private void ShowText()
    {
        string PlayerName = FindObjectOfType<Multiplayer>().GetUser().Name;
        //Debug.Log("White is mated " + Figure.WhiteIsMated+" My name is" + PlayerName +" White player name "+ StartAGame.WhitePlayerName +" Black is mated "+ Figure.BlackIsMated + " Black player name" + StartAGame.BlackPlayerName);
        if ((Figure.WhiteIsMated && PlayerName == StartAGame.WhitePlayerName) || (Figure.BlackIsMated && PlayerName == StartAGame.BlackPlayerName))
            EndingText.GetComponent<Text>().text = "Вы проиграли";
        else if ((Figure.WhiteIsMated && PlayerName != StartAGame.WhitePlayerName) || (Figure.BlackIsMated && PlayerName != StartAGame.BlackPlayerName))
            EndingText.GetComponent<Text>().text = "Вы выиграли";
        else if (Figure.IsDraw)
            EndingText.GetComponent<Text>().text = "Ничья";
        else
            EndingText.GetComponent<Text>().text = "Ошибка";
        EndingText.SetActive(true);
    }
}
