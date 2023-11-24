using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HelperNewFigure : MonoBehaviour
{
    [SerializeField] private GameObject PinnChecker, BlackFigureCanvas, WhiteFiguresCanvas, MainCanvas, WhiteKing, BlackKing;
    public static GameObject StatPinnChecker, StatBlackFigureCanvas, StatWhiteFiguresCanvas, StatMainCanvas, StatWhiteKing, StatBlackKing;
    private void Awake()
    {
        StatPinnChecker = PinnChecker;
        StatBlackFigureCanvas=BlackFigureCanvas;
        StatWhiteFiguresCanvas=WhiteFiguresCanvas;
        StatMainCanvas=MainCanvas;
        StatWhiteKing= WhiteKing;
        StatBlackKing=BlackKing;
    }
}
