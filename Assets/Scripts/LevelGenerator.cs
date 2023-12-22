using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator 
{
    private static readonly System.Random _rand = new System.Random();

    private static readonly Vector2Int _up = new Vector2Int(1, 0);
    private static readonly Vector2Int _down = new Vector2Int(-1, 0);
    private static readonly Vector2Int _left = new Vector2Int(0, -1);
    private static readonly Vector2Int _right = new Vector2Int(0, 1);

    private static List<Vector2Int> _generatedCoordinates;


    public static LevelData GenerateLevel(int slicesNumber, int gridWidth, int gridHeight)
    {
        //checks method's parameters validity
        gridWidth = gridWidth < 2 ? 2 : gridWidth; 
        gridHeight = gridHeight < 2 ? 2 : gridHeight;
        slicesNumber = slicesNumber > gridWidth * gridHeight ? (gridWidth * gridHeight) - 1 : slicesNumber;
        //check end

        ComputeRandomLevelData(slicesNumber, gridWidth, gridHeight);

        LevelData generatedLevel = ScriptableObject.CreateInstance<LevelData>();
        generatedLevel.SpawnCoordinates = _generatedCoordinates;
        generatedLevel.HeadCoordinates = _generatedCoordinates[0];
        generatedLevel.BottomCoordinates = _generatedCoordinates[1];
        generatedLevel.GridWidth = gridWidth;
        generatedLevel.GridHeight = gridHeight;
        return generatedLevel;
    }

    private static void ComputeRandomLevelData(int slicesNumber, int gridWidth, int gridHeight)
    {
        _generatedCoordinates = new List<Vector2Int>();
        _generatedCoordinates.Add(GetRandomCoordinates(gridWidth, gridHeight));
        _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, _generatedCoordinates[0]));
        Vector2Int even = _generatedCoordinates[0];
        Vector2Int odd = _generatedCoordinates[1];
        for (int i = 0; i < slicesNumber - 2; i++)
        {
            if (i % 2 == 0)
            {
                _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, even));
                even = _generatedCoordinates[^1];
            }
            else
            {
                _generatedCoordinates.Add(GetRandomNeighbour(gridWidth, gridHeight, odd));
                odd = _generatedCoordinates[^1];
            }
        }
    }

    private static Vector2Int GetRandomCoordinates(int gridWidth, int gridHeight) => new Vector2Int(Random.Range(0, gridHeight), Random.Range(0, gridWidth));

    private static Vector2Int GetRandomNeighbour(int gridWidth, int gridHeight, Vector2Int center)
    {
        List<Vector2Int> directions = GetAllowedDirections(gridWidth, gridHeight, center);
        directions = directions.Count > 1 ? ShuffleList(directions) : directions;

        Vector2Int randomNeighbour;
        foreach (Vector2Int direction in directions)
        {
            randomNeighbour = center + direction;
            if (_generatedCoordinates.Contains(randomNeighbour)) continue;
            else return randomNeighbour;
        }

        Debug.LogWarning("neighbour not found, reiterated method");
        return GetRandomNeighbour(gridWidth, gridHeight, _generatedCoordinates[_rand.Next(_generatedCoordinates.Count)]);
    }


    private static List<Vector2Int> GetAllowedDirections(int gridWidth, int gridHeight, Vector2Int center)
    {
        List<Vector2Int> allowedDirections = new List<Vector2Int>();
        if (center.x != 0) allowedDirections.Add(_down);
        if (center.x != gridHeight - 1) allowedDirections.Add(_up);
        if (center.y != 0) allowedDirections.Add(_left);
        if (center.y != gridWidth - 1) allowedDirections.Add(_right);
        return allowedDirections;
    }

    /// <summary>
    /// Fisher-Yates Shuffle Algorithm
    /// </summary>
    private static List<T> ShuffleList<T>(List<T> listToShuffle)
    {
        if (listToShuffle == null) return listToShuffle;

        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            var k = _rand.Next(i + 1);
            var value = listToShuffle[k];
            listToShuffle[k] = listToShuffle[i];
            listToShuffle[i] = value;
        }
        return listToShuffle;
    }


}
