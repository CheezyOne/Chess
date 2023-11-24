using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject InputRoomName;
    public void Play()
    {
        PlayerPrefs.SetString("Name", InputRoomName.GetComponent<InputRoomName>().PlayersName);
        SceneManager.LoadScene(1);
    }
}
