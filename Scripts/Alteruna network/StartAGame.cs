using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class StartAGame : AttributesSync
{
    public static GameObject NewGame;
    public static Action onGameStart, onPlayerJoin;
    [SynchronizableField] public static string WhitePlayerName, BlackPlayerName;
    [SerializeField] private GameObject RoomMenu, GameCanvas;
    private bool IsToDisappear = false;
    [SerializeField] private GameObject LeaveButton, DrawButton, ForfeitButton, EndingText, SpaceForLeaveButtons, NetworkManager;
    [SynchronizableField] private int RandomNumber=-1;
    [SynchronizableField] public string FirstPlayerName="", SecondPlayerName="";

    [SynchronizableMethod] private void AllPlayersJoined()
    {
        DrawButton.GetComponent<DrawAndForfeit>().DrawPropositionCounter = 0;
        StartCoroutine(Waiting());
    }
    [SynchronizableMethod] private void GetRandomNumber()
    {
        RandomNumber = UnityEngine.Random.Range(0, 100);
    }
    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(1f); 
        InvokeRemoteMethod("GetRandomNumber");
        yield return new WaitForSeconds(2f);
        NewGame = GameObject.FindWithTag("Game");
        AssignNewGame();
        OnGameStartInvoke();
    }
    private void OnGameStartInvoke()
    {
        onGameStart?.Invoke();   
    }
    private void AssignNewGame()
    {
       
        if (RandomNumber > 50)
        {
             WhitePlayerName = FirstPlayerName;
             BlackPlayerName = SecondPlayerName;
        }
        else
        {
             WhitePlayerName = SecondPlayerName;
             BlackPlayerName = FirstPlayerName;
        }
        Multiplayer multiplayer = FindObjectOfType<Multiplayer>();
        LeaveButton.transform.position = SpaceForLeaveButtons.transform.position;
        DrawButton.GetComponent<Button>().interactable = true;
        ForfeitButton.GetComponent<Button>().interactable = true;
        GameEnd NewGameComponent = NewGame.transform.GetChild(3).GetChild(5).GetComponent<GameEnd>();
        NewGameComponent.EndingText = EndingText;
        NewGameComponent.DrawButton = DrawButton;
        NewGameComponent.ExitButton = LeaveButton;
        NewGameComponent.ForfeitButton = ForfeitButton;
    }
    
    private void MoveTheCamera(float Xpos)
    {
        Camera.main.transform.position = new Vector3(Xpos, 0, 10);
    }

    private void Update()
    {
        if (IsToDisappear)
        {
            GameCanvas.SetActive(true);
            RoomMenu.GetComponent<CanvasGroup>().alpha = 0;
            GameCanvas.GetComponent<CanvasGroup>().alpha += Time.deltaTime * 3;
        }
        else
        {
            RoomMenu.GetComponent<CanvasGroup>().alpha += Time.deltaTime;
            
            GameCanvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime * 3;
            if(GameCanvas.GetComponent<CanvasGroup>().alpha<=0)
                GameCanvas.SetActive(false);
            
        }
        
    }
    public void PlayerJoin()
    { 
        
        Multiplayer multiplayer = FindObjectOfType<Multiplayer>();
        LeaveButton.transform.position = SpaceForLeaveButtons.transform.position;
        DrawButton.GetComponent<Button>().interactable = false;
        ForfeitButton.GetComponent<Button>().interactable = false;
        MoveTheCamera(17.8f);
        IsToDisappear = true;
        if (multiplayer.CurrentRoom.GetUserCount() == 2)
        {
            FirstPlayerName = multiplayer.GetUsers()[0].Name;
            SecondPlayerName = multiplayer.GetUsers()[1].Name;
            multiplayer.LockRoom();
            NewGame = NetworkManager.GetComponent<Spawner>().Spawn(0);
            BroadcastRemoteMethod("AllPlayersJoined");
        }
        /*
        if (multiplayer.CurrentRoom.GetUserCount() == 1)
        {
            FirstPlayerName = multiplayer.GetUsers()[0].Name;
            SecondPlayerName = multiplayer.GetUsers()[0].Name;
            multiplayer.LockRoom();
            NewGame = NetworkManager.GetComponent<Spawner>().Spawn(0);
            BroadcastRemoteMethod("AllPlayersJoined");
        }
        */
    }
    public void PlayerExited()
    {
        MoveTheCamera(0f);
        IsToDisappear = false;
    }
}
