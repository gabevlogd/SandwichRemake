using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static event Action<LevelData> LevelLoaded;
    [SerializeField]
    private List<LevelData> _levelRoster;
    private LevelData _levelToLoad;
    private Grid<SwipeableObjectData> _grid;
    private static int _currentLevelIndex;

    private void Awake()
    {
        if (_currentLevelIndex == 0) LoadLevel(0);
    }

    private void OnEnable() => WinState.LoadNextLevel += LoadLevel;

    private void LoadLevel(int levelIndex)
    {
        if (levelIndex < _levelRoster.Count)
            _levelToLoad = _levelRoster[levelIndex];
        else _levelToLoad = _levelRoster[0]; //restart from first level

        GridHandler.CreateGrid(_levelToLoad.GridHeight, _levelToLoad.GridWidth, 1);
        _grid = GridHandler.Grid;
        SpawnSwipeableObjects();
        _currentLevelIndex = _levelToLoad.LevelIndex;
        LevelLoaded?.Invoke(_levelToLoad);
    }

    private void SpawnSwipeableObjects()
    {
        
        foreach(Vector2Int coo in _levelToLoad.SpawnCoordinates)
        {
            SwipeableObjectData newSwipeableData = _grid.GetGridObject(coo.x, coo.y);
            if (AreEdgeCoordinates(newSwipeableData.Row, newSwipeableData.Column)) newSwipeableData.Edge = true;
            SwipeableObject newSwipeable = Instantiate(_levelToLoad.SwipeableObjectPrefab, _grid.GetWorldPosition(newSwipeableData.Column, newSwipeableData.Row), Quaternion.identity);
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

