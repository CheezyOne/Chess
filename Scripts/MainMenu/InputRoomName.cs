using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRoomName : MonoBehaviour
{
    [SerializeField] private GameObject TextInput, PlayButton;
    public string PlayersName;

    private bool HasAnyLetters(string Name)
    {
        if (Name.Length <1)
            return false;
        for (int i = 0; i < Name.Length; i++)
        {
            if (char.IsLetter(Name[i]) || char.IsDigit(Name[i]))
            {
                return true;
            }
        }
        return false;
    }
    public void OnDataChange(string PlayerName)
    {
        PlayersName = PlayerName;
        if (!HasAnyLetters(PlayersName))
        {
            PlayButton.GetComponent<Button>().interactable = false;
            return;
        }
        PlayButton.GetComponent<Button>().interactable = true;
    }
}