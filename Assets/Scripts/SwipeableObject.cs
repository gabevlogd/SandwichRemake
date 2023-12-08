using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    public SwipeableObjectData Data;
    public static event Action OnSwipeableObjMovementEnded;

    private GameObject _stackToMove;
    private Vector3 _landingPoint;
    private Action _onPerformMovement;

    private void Awake()
    {
        SwipesManager.TriggerSwipeableObjMoevement += StartPerformingMovement;
    }

    private void Update()
    {
        if (_stackToMove != null)
            PerformMovement();
    }

    public void StartPerformingMovement(SwipeableObject from, SwipeableObject to)
    {
        if (from == Data.This)
        {
            //Debug.Log($"row: {Data.Row}, column: {Data.Column}");
            _stackToMove = from.Data.Stack;
            _landingPoint = GetLandingPoint(from, to);
            UpdateData(from, to);
            //OnSwipeableObjMovementEnded?.Invoke();
        }
    }

    private void PerformMovement()
    {
        _stackToMove.transform.position = _landingPoint;
        _stackToMove.transform.rotation *= Quaternion.Euler(180f, 0f, 0f);
        OnSwipeableObjMovementEnded?.Invoke();
        _stackToMove = null;



    }

    private Vector3 GetLandingPoint(SwipeableObject from, SwipeableObject to)
    {
        Transform tmp = to.Data.Stack.transform;
        Debug.Log(new Vector3(tmp.position.x, tmp.position.y + tmp.lossyScale.y * from.Data.StackCount, tmp.position.z));
        return new Vector3(tmp.position.x, tmp.position.y + tmp.lossyScale.y * from.Data.StackCount, tmp.position.z);
    }

    private void UpdateData(SwipeableObject from, SwipeableObject to)
    {
        from.Data.Stack.transform.parent = to.Data.Stack.transform;
        to.Data.StackCount += GetChildCount(from.Data.Stack.transform, 1);
        from.Data.Stack = null;
        from.Data.StackCount = 0;
        from.Data.This = null;
    }

    public void InitializeObject()
    {
        Data.This = this;
        Data.Stack = transform.GetChild(0).gameObject;
        Data.StackCount = 1;
    }

    private int GetChildCount(Transform parent, int counter)
    {
        if (parent.childCount == 0)
            return counter;
        else return GetChildCount(parent.GetChild(0).transform, counter + 1);
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
