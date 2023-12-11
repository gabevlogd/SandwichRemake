using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    public static event Action<string, SwipeableObject, SwipeableObject> RunAnimation;
    public static event Action GameWon;
    public SwipeableObjectData Data;
    private static int _lastSkin = 0;
    private int _slicesNumber;

    private void OnEnable()
    {
        SwipesManager.TriggerStackMoevement += TryMoveStack;
        LevelLoader.LevelLoaded += SetSlicesNumber;
        WinState.LoadNextLevel += DestroyItself;
    }
    private void OnDisable()
    {
        SwipesManager.TriggerStackMoevement -= TryMoveStack;
        LevelLoader.LevelLoaded -= SetSlicesNumber;
        WinState.LoadNextLevel -= DestroyItself;
    }

    private void OnDestroy() => Resources.UnloadUnusedAssets();

    public void InitializeObject()
    {
        Data.This = this;
        Data.Stack = transform.GetChild(0).gameObject;
        Data.StackCount = 1;
        SetSkin();
    }

    private void SetSkin()
    {
        if (Data.Edge) return;
        Data.Stack.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Mat_{GetRandomSkin()}");
    }

    private int GetRandomSkin()
    {
        int currentSkin = UnityEngine.Random.Range(1, 5);
        if (_lastSkin == currentSkin)
            return GetRandomSkin();
        else
        {
            _lastSkin = currentSkin;
            return currentSkin;
        }
    }

    private void TryMoveStack(SwipeableObject from, SwipeableObject to)
    {
        if (from == Data.This)
        {
            if (from.Data.Edge && to.Data.Edge)
            {
                if (from.Data.StackCount + to.Data.StackCount != _slicesNumber)
                    RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
                else
                {
                    Debug.Log("WIN");
                    RunAnimation?.Invoke(Constants.STACK_MOVE, from, to);
                    GameWon?.Invoke();
                }
            }
            else if (from.Data.Edge)
                RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
            else
            {
                RunAnimation?.Invoke(Constants.STACK_MOVE, from, to);
                TransferData(from, to);
            }
        }
    }

    private void SetSlicesNumber(LevelData levelData) => _slicesNumber = levelData.SpawnCoordinates.Count;


    private void TransferData(SwipeableObject from, SwipeableObject to)
    {
        to.Data.StackCount += from.Data.StackCount;
        from.Data.Stack = null;
        from.Data.StackCount = 0;
        from.Data.This = null;
    }

    private void DestroyItself(int value) => Destroy(gameObject);
    
}



