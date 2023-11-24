using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoomsNameFix : AttributesSync
{
    [SerializeField] private Button StartButton;
    private string PlayerName;
    private IEnumerator CheckAvailableRoomNames()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < Multiplayer.AvailableRooms.Count; i++)
        {
            if (PlayerName == Multiplayer.AvailableRooms[i].Name)
            {
                StartButton.interactable = false;
                yield return CheckAvailableRoomNames();
            }
        }
        StartButton.interactable = true;
        yield return CheckAvailableRoomNames();
    }
    private void Awake()
    {
        Multiplayer = FindObjectOfType<Multiplayer>();
        PlayerName = PlayerPrefs.GetString("Name");
        Multiplayer.SetUsername(PlayerName);
        //StartCoroutine(CheckAvailableRoomNames());
    }
}
