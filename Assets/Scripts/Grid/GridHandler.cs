using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHandler 
{
    public static Grid<SwipeableObjectData> Grid;

    public static void CreateGrid(int height, int width, float cellSize)
    {
        Grid = new Grid<SwipeableObjectData>(height, width, cellSize, GetGridOrigin(height, width), (int x, int y) => new SwipeableObjectData(x, y));
    }

    private static Vector3 GetGridOrigin(int height, int width) => new Vector3(-width * 0.5f, 0f, -height * 0.5f);

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
                Vector3 tileCenter = Grid.GetWorldPosition(row, column);
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
