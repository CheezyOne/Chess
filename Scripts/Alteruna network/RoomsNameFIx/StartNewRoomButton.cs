using Alteruna;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewRoomButton : AttributesSync
{
    [SynchronizableMethod]
    private void TestFun()
    {
        SceneManager.LoadScene(2);
    }
    public void TestConnet(Multiplayer mul, Room room, User user)
    {
        if (room.GetUserCount()>1)
        {
            BroadcastRemoteMethod("TestFun");
        }
    }    
    public void StartNewGame()
    {
        //InvokeRemoteMethod("TestFun");
        //SceneManager.LoadScene(2);
    }
    private void Update()
    {
    }
}
