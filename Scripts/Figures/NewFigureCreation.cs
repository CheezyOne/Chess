using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFigureCreation : AttributesSync
{
    private GameObject NewFigure;
    private new void OnEnable()
    {
        Figure.OnNewFigureSpawn += InvokeSpawner;
    }
    private void OnDisable()
    {
        Figure.OnNewFigureSpawn -= InvokeSpawner;
    }
    private void InvokeSpawner()
    {
        InvokeRemoteMethod("SpawnNewFigureSync");
    }
    [SynchronizableMethod]
    public void SpawnNewFigureSync()//Figures in spawner are white first , queen,rook,bishop,horse order
    {//Публичная статичная NewFigureToCreate не синхронизируется, а жаль
        int FigureNumber = FigurePositionHolder.NewFigureToCreate + 1;
        Spawner spawner = GameObject.FindGameObjectWithTag("MultiplayerManager").GetComponent<Spawner>();
        spawner.Despawn(Figure.PawnLastSquare.GetComponent<BoardSquare>().FigureOnTheSpot);
        BroadcastRemoteMethod("DeletePawn");
        spawner.Spawn(FigureNumber);
        BroadcastRemoteMethod("AssignNewFigureSync");
    }
    [SynchronizableMethod] private void DeletePawn()
    {
        Destroy(Figure.PawnLastSquare.GetComponent<BoardSquare>().FigureOnTheSpot);
    }    
    [SynchronizableMethod] public void AssignNewFigureSync()
    {
        NewFigure = GameObject.FindWithTag("NewFigure");
        NewFigure.tag = "Untagged";
        GetComponent<Figure>().AssignNewFigure(NewFigure);
    }
}