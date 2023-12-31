using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public List<Vector2Int> SpawnCoordinates;
    public Vector2Int HeadCoordinates; 
    public Vector2Int BottomCoordinates;
    public int LevelIndex;
    public int GridWidth;
    public int GridHeight;
}
