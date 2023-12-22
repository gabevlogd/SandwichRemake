using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHandler 
{
    public static Grid<SwipeableObjectData> Grid;

    public static Grid<SwipeableObjectData> CreateGrid(int width, int height, float cellSize)
    {
        Grid = new Grid<SwipeableObjectData>(width, height, cellSize, GetGridOrigin(width, height), (int x, int y) => new SwipeableObjectData(x, y));
        return Grid;
    }

    private static Vector3 GetGridOrigin(int width, int height) => new Vector3(-width * 0.5f, 0f, -height * 0.5f);

    /// <summary>
    /// Display the grid (Call it in OnDrawGizmos)
    /// </summary>
    public static void VisualizeGrid()
    {
        if (Grid == null) return;

        for (int row = 0; row < Grid.GetHeight(); row++)
        {
            for (int column = 0; column < Grid.GetWidth(); column++)
            {
                Vector3 tileCenter = Grid.GetWorldPosition(column, row);
                Vector3 vertex1 = new Vector3(tileCenter.x - Grid.GetCellSize() * 0.5f, 0f, tileCenter.z - Grid.GetCellSize() * 0.5f);
                Vector3 vertex2 = new Vector3(tileCenter.x + Grid.GetCellSize() * 0.5f, 0f, tileCenter.z + Grid.GetCellSize() * 0.5f);
                Vector3 vertex3 = new Vector3(tileCenter.x + Grid.GetCellSize() * 0.5f, 0f, tileCenter.z - Grid.GetCellSize() * 0.5f);
                Vector3 vertex4 = new Vector3(tileCenter.x - Grid.GetCellSize() * 0.5f, 0f, tileCenter.z + Grid.GetCellSize() * 0.5f);
                Gizmos.DrawLine(vertex1, vertex4);
                Gizmos.DrawLine(vertex4, vertex2);
                Gizmos.DrawLine(vertex2, vertex3);
                Gizmos.DrawLine(vertex3, vertex1);
            }
        }
    }
}
