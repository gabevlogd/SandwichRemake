using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//it's a class and not a struct for the usual value and reference problems (C# merda)
[System.Serializable]
public class SwipeableObjectData
{
    public int Row;
    public int Column;
    public int StackCount;
    public bool Edge;
    public SwipeableObject This;
    public GameObject Stack;

    public SwipeableObjectData(int x, int y)
    {
        Row = y;
        Column = x;
    }
}
