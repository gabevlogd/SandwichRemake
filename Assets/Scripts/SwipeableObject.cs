using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    public SwipeableObjectData Data;

    public static event Action<string, SwipeableObject, SwipeableObject> RunAnimation;

    private void Awake() => SwipesManager.TriggerStackMoevement += TryMoveStack;

    public void InitializeObject()
    {
        Data.This = this;
        Data.Stack = transform.GetChild(0).gameObject;
        Data.StackCount = 1;
    }

    public void TryMoveStack(SwipeableObject from, SwipeableObject to)
    {
        if (from == Data.This)
        {
            //CalculateAnimationData?.Invoke(from, to);
            RunAnimation?.Invoke(Constants.INVALID_MOVE, from, to);
            //TransferData(from, to);
            //RunAnimation?.Invoke(false);
        }
    }


    private void TransferData(SwipeableObject from, SwipeableObject to)
    {
        //_newParent = to.Data.Stack.transform;
        to.Data.StackCount += GetStackCount(from.Data.Stack.transform.position);
        from.Data.Stack = null;
        from.Data.StackCount = 0;
        from.Data.This = null;
    }

    private int GetStackCount(Vector3 bottom)
    {
        return Physics.RaycastAll(bottom, Vector3.up).Length + 1;
    }
}


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
