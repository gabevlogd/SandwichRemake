using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static event Action<LevelData> LevelLoaded;

    [SerializeField]
    private SwipeableObject _swipeableObjectPrefab;
    [Min(2)]
    [SerializeField]
    private int _gridWidth;
    [Min(2)]
    [SerializeField]
    private int _gridHeight;

    private LevelData _levelToLoad;
    private Grid<SwipeableObjectData> _grid;

    private void OnEnable() => WinState.LoadNextLevel += LoadLevel;
    private void OnDisable() => WinState.LoadNextLevel -= LoadLevel;
    private void Start() => LoadLevel();

    private void LoadLevel()
    {
        int slicesNumber = UnityEngine.Random.Range(3, _gridWidth * _gridHeight);

        _levelToLoad = LevelGenerator.GenerateLevel(slicesNumber, _gridWidth, _gridHeight);
        _grid = GridHandler.CreateGrid(_levelToLoad.GridWidth, _levelToLoad.GridHeight, 1);

        _levelToLoad.LevelIndex = PlayerPrefs.GetInt(Constants.LAST_LEVEL_INDEX);
        PlayerPrefs.SetInt(Constants.LAST_LEVEL_INDEX, _levelToLoad.LevelIndex + 1);

        SpawnSwipeableObjects();
        LevelLoaded?.Invoke(_levelToLoad);
    }

    private void SpawnSwipeableObjects()
    {
        
        foreach(Vector2Int coo in _levelToLoad.SpawnCoordinates)
        {
            SwipeableObjectData newSwipeableData = _grid.GetGridObject(coo.x, coo.y);
            if (AreEdgeCoordinates(newSwipeableData.Row, newSwipeableData.Column)) newSwipeableData.Edge = true;
            SwipeableObject newSwipeable = Instantiate(_swipeableObjectPrefab, _grid.GetWorldPosition(newSwipeableData.Column, newSwipeableData.Row), Quaternion.identity);
            newSwipeable.Data = newSwipeableData;
            newSwipeable.InitializeObject();
        }
    }

    private bool AreEdgeCoordinates(int row, int column) => (row == _levelToLoad.HeadCoordinates.x && column == _levelToLoad.HeadCoordinates.y) || (row == _levelToLoad.BottomCoordinates.x && column == _levelToLoad.BottomCoordinates.y);



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        GridHandler.VisualizeGrid();
    }
#endif
}

