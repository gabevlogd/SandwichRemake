using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    public static LevelData LevelToLoad;
    private Grid<SwipeableObjectData> _grid;

    private void Awake()
    {
        LevelToLoad = Resources.Load<LevelData>("TestLevel");
        GridHandler.CreateGrid(LevelToLoad.GridHeight, LevelToLoad.GridWidth, 1);
        _grid = GridHandler.Grid;
        SpawnSwipeableObjects();
    }

    private void SpawnSwipeableObjects()
    {
        
        foreach(Vector2Int coo in LevelToLoad.SpawnCoordinates)
        {
            SwipeableObjectData newSwipeableData = _grid.GetGridObject(coo.x, coo.y);
            if (AreEdgeCoordinates(newSwipeableData.Row, newSwipeableData.Column)) newSwipeableData.Edge = true;
            SwipeableObject newSwipeable = Instantiate(LevelToLoad.SwipeableObjectPrefab, _grid.GetWorldPosition(newSwipeableData.Column, newSwipeableData.Row), Quaternion.identity);
            newSwipeable.Data = newSwipeableData;
            newSwipeable.InitializeObject();
        }
    }

    private bool AreEdgeCoordinates(int row, int column) => (row == LevelToLoad.HeadCoordinates.x && column == LevelToLoad.HeadCoordinates.y) || (row == LevelToLoad.BottomCoordinates.x && column == LevelToLoad.BottomCoordinates.y);



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        GridHandler.VisualizeGrid();
    }
#endif
}

