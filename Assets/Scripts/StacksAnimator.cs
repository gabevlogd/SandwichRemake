using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StacksAnimator : MonoBehaviour
{
    private SwipeableObject _fromObj;
    private SwipeableObject _toObj;

    private GameObject _targetStack;

    private Transform _originalParent;
    private Transform _rotationPivot;

    private Quaternion _targetRotation;

    private Vector3 _targetPositionA;
    private Vector3 _targetPositionB;

    private float _singleStackHeight;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _angularSpeed;
    private bool _isReversed;

    private Action _performAnimation;

    public static event Action AnimationEnded;

    private void Awake()
    {
        _rotationPivot = transform.GetChild(0);
        SwipeableObject.CalculateAnimationData += CalculateAnimationData;
        SwipeableObject.TriggerAnimation += TriggerAnimation;
    }

    private void Update() => _performAnimation?.Invoke();

    private void CalculateAnimationData(SwipeableObject from, SwipeableObject to)
    {
        //get stack to move
        _targetStack = from.Data.Stack;
        _originalParent = to.Data.Stack.transform;
        //get the height of the single element of a stack
        _singleStackHeight = _targetStack.transform.lossyScale.y;
        SetRotationPivot(from, to);
        _targetRotation = GetTargetRotation(false);
        //get the first position which to traslate the stack
        _targetPositionA = GetStartingPoint(from);
        //get the last position which to traslate the stack
        _targetPositionB = GetFinalPoint(from, to);
    }

    private void TriggerAnimation(bool reverse)
    {
        _targetStack.transform.parent = _rotationPivot;
        _isReversed = reverse;
        _performAnimation = PerformStepA;
    }


    private void SetRotationPivot(SwipeableObject from, SwipeableObject to)
    {
        _rotationPivot.position = GetRotationPivotPosition(from, to);
        _rotationPivot.rotation = Quaternion.identity;
    }

    private Vector3 GetRotationPivotPosition(SwipeableObject from, SwipeableObject to)
    {
        float rotationPivotY = _singleStackHeight * Mathf.Max(from.Data.StackCount, to.Data.StackCount);
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

    private Quaternion GetTargetRotation(bool reverse)
    {
        switch (SwipesManager.SwipeDirection)
        {
            case SwipeDirection.Up:
                if (reverse) return Quaternion.Euler(180f, 0f, 0f);
                else return Quaternion.Euler(-180f, 0f, 0f);
            case SwipeDirection.Down:
                if (reverse) return Quaternion.Euler(-180f, 0f, 0f);
                else return Quaternion.Euler(180f, 0f, 0f);
            case SwipeDirection.Right:
                if (reverse) return Quaternion.Euler(0f, 0f, - 180f);
                else return Quaternion.Euler(0f, 0f, 180f);
            case SwipeDirection.Left:
                if (reverse) return Quaternion.Euler(0f, 0f, 180f);
                else return Quaternion.Euler(0f, 0f, -180f);
            default:
                return Quaternion.identity;
        }
    }

    private Vector3 GetFinalPoint(SwipeableObject from, SwipeableObject to)
    {
        Transform tmp = to.Data.Stack.transform;
        return new Vector3(tmp.position.x, tmp.position.y + tmp.lossyScale.y * (to.Data.StackCount + from.Data.StackCount - 1), tmp.position.z);
    }

    private Vector3 GetStartingPoint(SwipeableObject from)
    {
        return new Vector3(from.Data.Stack.transform.position.x, _rotationPivot.position.y - _singleStackHeight * 0.5f, from.Data.Stack.transform.position.z);
    }

    private void PerformStepA()
    {
        if (Vector3.Distance(_targetStack.transform.position, _targetPositionA) > 0.01f)
            _targetStack.transform.position = Vector3.MoveTowards(_targetStack.transform.position, _targetPositionA, Time.deltaTime * _speed);
        else
        {
            _targetStack.transform.position = _targetPositionA;
            _performAnimation = PerformStepB;
        }
    }

    private void PerformStepB()
    {
        if (Quaternion.Dot(_rotationPivot.rotation, _targetRotation) < 0.99f)
            _rotationPivot.rotation = Quaternion.RotateTowards(_rotationPivot.rotation, _targetRotation, Time.deltaTime * _angularSpeed);
        else
        {
            _rotationPivot.rotation = _targetRotation;
            _performAnimation = PerformStepC;
        }
    }

    private void PerformStepC()
    {
        if (Vector3.Distance(_targetStack.transform.position, _targetPositionB) > 0.01f)
            _targetStack.transform.position = Vector3.MoveTowards(_targetStack.transform.position, _targetPositionB, Time.deltaTime * _speed);
        else if (_isReversed)
        {
            ReverseAnimationData();
            _performAnimation = PerformStepA;
        }
        else
        {
            _targetStack.transform.parent = _originalParent;
            _performAnimation = null;
            AnimationEnded?.Invoke();
        }
    }

    private void ReverseAnimationData()
    {
        Vector3 tmp = _targetPositionB;
        _targetPositionB = _targetPositionA;
        _targetPositionA = tmp;
        _targetRotation = GetTargetRotation(true);
        _isReversed = false;
    }
}
