using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    public static event Action<string, SwipeableObject, SwipeableObject> RunAnimation;
    public SwipeableObjectData Data;
    private static int _lastSkin = 0;

    private void Awake() => SwipesManager.TriggerStackMoevement += TryMoveStack;

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

    public void TryMoveStack(SwipeableObject from, SwipeableObject to)
    {
        if (from == Data.This)
        {
            if (from.Data.Edge && to.Data.Edge)
            {
                if (from.Data.StackCount + to.Data.StackCount != LevelLoader.LevelToLoad.PawnNumber)
                    RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
                else
                {
                    Debug.Log("WIN");
                    RunAnimation?.Invoke(Constants.STACK_MOVE, from, to);
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


    private void TransferData(SwipeableObject from, SwipeableObject to)
    {
        to.Data.StackCount += GetStackCount(from.Data.Stack.transform.position);
        from.Data.Stack = null;
        from.Data.StackCount = 0;
        from.Data.This = null;
    }

    private int GetStackCount(Vector3 bottom) => Physics.RaycastAll(bottom, Vector3.up).Length + 1;
    
}



