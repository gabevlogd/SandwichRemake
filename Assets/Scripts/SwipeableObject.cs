using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeableObject : MonoBehaviour
{
    public SwipeableObjectData Data;
    public static event Action OnSwipeableObjMovementEnded;
    private Action _onPerformMovement;

    [SerializeField]
    private Transform _rotationPivot;
    private Transform _newParent;
    private GameObject _stackToMove;
    private Vector3 _landingPoint;
    private Quaternion _targetRotation;

    private float _angularSpeed = 500f;
    private float _speed = 5f;

    public static event Action<bool> TriggerAnimation;
    public static event Action<SwipeableObject, SwipeableObject> CalculateAnimationData;

    private void Awake() => SwipesManager.TriggerSwipeableObjMoevement += StartPerformingMovement;

    private void Update() => _onPerformMovement?.Invoke();

    public void InitializeObject()
    {
        Data.This = this;
        Data.Stack = transform.GetChild(0).gameObject;
        Data.StackCount = 1;
    }

    public void StartPerformingMovement(SwipeableObject from, SwipeableObject to)
    {
        if (from == Data.This)
        {
            _stackToMove = from.Data.Stack;
            _landingPoint = GetLandingPoint(from, to);
            _rotationPivot.position = GetRotationPivotPosition(from, to);
            _rotationPivot.rotation = Quaternion.identity;
            from.Data.Stack.transform.parent = _rotationPivot;
            if (NeedAlignment(from, to))
                _onPerformMovement = AligneWithRotationPivot;
            else _onPerformMovement = Rotate;
            TransferData(from, to);
        }
    }



    private void AligneWithRotationPivot()
    {
        Vector3 targetPos = new Vector3(_stackToMove.transform.position.x, _rotationPivot.position.y - _stackToMove.transform.lossyScale.y * 0.5f, _stackToMove.transform.position.z);
        _stackToMove.transform.position = Vector3.MoveTowards(_stackToMove.transform.position, targetPos, Time.deltaTime * _speed);
        if (Vector3.Distance(_stackToMove.transform.position, targetPos) < 0.01f)
        {
            _stackToMove.transform.position = targetPos;
            _onPerformMovement = Rotate;
        }
    }

    private void Rotate()
    {
        _rotationPivot.rotation = Quaternion.RotateTowards(_rotationPivot.rotation, _targetRotation, Time.deltaTime * _angularSpeed);
        if (Quaternion.Dot(_rotationPivot.rotation, _targetRotation) >= 0.99f)
        {
            _rotationPivot.rotation = _targetRotation;
            _onPerformMovement = Translate;
        }
    }

    private void Translate()
    {
        _stackToMove.transform.position = Vector3.MoveTowards(_stackToMove.transform.position, _landingPoint, Time.deltaTime * _speed);
        if (Vector3.Distance(_stackToMove.transform.position, _landingPoint) < 0.01f)
        {
            _stackToMove.transform.position = _landingPoint;
            _stackToMove.transform.parent = _newParent;
            _onPerformMovement = null;
            OnSwipeableObjMovementEnded?.Invoke();
        }
    }

    



    private Vector3 GetRotationPivotPosition(SwipeableObject from, SwipeableObject to)
    {
        float rotationPivotY = from.Data.Stack.transform.lossyScale.y * Mathf.Max(from.Data.StackCount, to.Data.StackCount);
        switch (SwipesManager.SwipeDirection)
        {
            case SwipeDirection.Up:
                _targetRotation = Quaternion.Euler(-180f, 0f, 0f);
                return new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z + GridHandler.Grid.GetCellSize() * 0.5f);
            case SwipeDirection.Down:
                _targetRotation = Quaternion.Euler(180f, 0f, 0f);
                return new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z - GridHandler.Grid.GetCellSize() * 0.5f);
            case SwipeDirection.Right:
                _targetRotation = Quaternion.Euler(0f, 0f, 180f);
                return new Vector3(from.transform.position.x + GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z);
            case SwipeDirection.Left:
                _targetRotation = Quaternion.Euler(0f, 0f, -180f);
                return new Vector3(from.transform.position.x - GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z);
            default:
                return Vector3.zero;
        }
    }

    private bool NeedAlignment(SwipeableObject from, SwipeableObject to) => from.Data.StackCount < to.Data.StackCount;




    private Vector3 GetLandingPoint(SwipeableObject from, SwipeableObject to)
    {
        Transform tmp = to.Data.Stack.transform;
        return new Vector3(tmp.position.x, tmp.position.y + tmp.lossyScale.y * (to.Data.StackCount + from.Data.StackCount - 1), tmp.position.z);
    }


    private void TransferData(SwipeableObject from, SwipeableObject to)
    {
        _newParent = to.Data.Stack.transform;
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
