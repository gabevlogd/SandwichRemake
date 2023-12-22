using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject> 
{
    private int m_width;
    private int m_height;
    private float m_cellSize;
    private Vector3 m_originPosition;
    private TGridObject[,] m_gridArray;


    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, TGridObject> createGridObject) 
    {
        m_width = width;
        m_height = height;
        m_cellSize = cellSize;
        m_originPosition = originPosition;

        m_gridArray = new TGridObject[height, width];

        for (int x = 0; x < m_gridArray.GetLength(0); x++) 
        {
            for (int y = 0; y < m_gridArray.GetLength(1); y++) 
            {
                m_gridArray[x, y] = createGridObject(x, y);
            }
        }
    }

    public int GetWidth() => m_width;

    public int GetHeight() => m_height;

    public float GetCellSize() => m_cellSize;

    /// <summary>
    /// Returns the corresponding world position of the passed coordinates
    /// </summary>
    public Vector3 GetWorldPosition(int x, int y)
    {
        //Debug.Log(new Vector3(x, y, 0f) * m_cellSize + new Vector3(1f, 1f, 0f) * m_cellSize * .5f + m_originPosition);
        return new Vector3(x, 0f, y) * m_cellSize + new Vector3(1f, 0f, 1f) * m_cellSize * .5f + m_originPosition; // changed x,0,z instead of x,y,0
    }
    
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - m_originPosition).z / m_cellSize);
        int y = Mathf.FloorToInt((worldPosition - m_originPosition).x / m_cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Returns the corresponding grid coordinates of the passed world position
    /// </summary>
    public void GetXY(Vector3 worldPosition, out int x, out int y) 
    {
        x = Mathf.FloorToInt((worldPosition - m_originPosition).z / m_cellSize); // changed to z instead of y
        y = Mathf.FloorToInt((worldPosition - m_originPosition).x / m_cellSize); 
    }

    /// <summary>
    /// Return the grid object placed at the passed coordinates
    /// </summary>
    public TGridObject GetGridObject(int x, int y) 
    {
        if (x >= 0 && y >= 0 && x < m_height && y < m_width) return m_gridArray[x, y];
        else return default;
    }

    /// <summary>
    /// Returns the grid object placed at the coordinates obtained from the past world position
    /// </summary>
    public TGridObject GetGridObject(Vector3 worldPosition) 
    {
        GetXY(worldPosition, out int x, out int y);
        return GetGridObject(x, y);
    }


    

}

