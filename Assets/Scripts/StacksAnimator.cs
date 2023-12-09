using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StacksAnimator : MonoBehaviour
{
    public static event Action AnimationEnded;
    private Action _performAnimation;

    public GameObject TargetStack { get => _targetStack; }
    private GameObject _targetStack;
    public Transform OriginalParent { get => _originalParent; }
    private Transform _originalParent;
    public Transform RotationPivot { get => _rotationPivot; }
    private Transform _rotationPivot;

    public Quaternion TargetRoation { get => _targetRotation; }
    private Quaternion _targetRotation;
    public Vector3 StartingPoint { get => _startingPoint; }
    private Vector3 _startingPoint;
    public Vector3 FinalPoint { get => _finalPoint; }
    private Vector3 _finalPoint;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _angularSpeed;
    private float _singleStackHeight;
    private bool _isReversed;


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
        _rotationPivot.position = GetRotationPivotPosition(from, to);
        _targetRotation = GetTargetRotation(false);
        //get the first position which to traslate the stack
        _startingPoint = GetStartingPoint(from, to);
        //get the last position which to traslate the stack
        _finalPoint = GetFinalPoint(from, to);
    }

    private void TriggerAnimation(bool reverse)
    {
        TMP2 = _targetStack.transform.position;
        _targetStack.transform.parent = _rotationPivot;
        _isReversed = reverse;
        _performAnimation = PerformStepA;
    }

    private Vector3 GetRotationPivotPosition(SwipeableObject from, SwipeableObject to)
    {
        //also reset the rotation
        _rotationPivot.rotation = Quaternion.identity;

        float rotationPivotY;
        if (from.Data.StackCount <= to.Data.StackCount)
            rotationPivotY = _singleStackHeight * to.Data.StackCount;
        else rotationPivotY = _singleStackHeight * from.Data.StackCount;

        switch (SwipesManager.SwipeDirection)
        {
            case SwipeDirection.Up:
                return new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z + GridHandler.Grid.GetCellSize() * 0.5f);
            case SwipeDirection.Down:
                return new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z - GridHandler.Grid.GetCellSize() * 0.5f);
            case SwipeDirection.Right:
                return new Vector3(from.transform.position.x + GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z);
            case SwipeDirection.Left:
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
        return to.Data.Stack.transform.position + Vector3.up * (_singleStackHeight * (from.Data.StackCount + to.Data.StackCount - 1));
    }

    private Vector3 GetStartingPoint(SwipeableObject from, SwipeableObject to)
    {
        if (from.Data.StackCount >= to.Data.StackCount)
            return from.Data.Stack.transform.position;
        else
            return from.Data.Stack.transform.position + Vector3.up * (_singleStackHeight * (to.Data.StackCount - from.Data.StackCount));
    }

    private void PerformStepA()
    {
        if (Vector3.Distance(_targetStack.transform.position, _startingPoint) > 0.01f)
            _targetStack.transform.position = Vector3.MoveTowards(_targetStack.transform.position, _startingPoint, Time.deltaTime * _speed);
        else
        {
            _targetStack.transform.position = _startingPoint;
            _performAnimation = PerformStepB;
        }
    }

    Vector3 TMP;
    Vector3 TMP2;
    private void PerformStepB()
    {
        if (Quaternion.Dot(_rotationPivot.rotation, _targetRotation) < 0.99f)
            _rotationPivot.rotation = Quaternion.RotateTowards(_rotationPivot.rotation, _targetRotation, Time.deltaTime * _angularSpeed * 100);
        else
        {
            _rotationPivot.rotation = _targetRotation;
            TMP = _targetStack.transform.position;
            _performAnimation = PerformStepC;
        }
    }

    private void PerformStepC()
    {
        if (Vector3.Distance(_targetStack.transform.position, _finalPoint) > 0.01f)
            _targetStack.transform.position = Vector3.MoveTowards(_targetStack.transform.position, _finalPoint, Time.deltaTime * _speed);
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
        //Vector3 tmp = _targetPositionB;
        Debug.Log("before B: " + _finalPoint);
        _finalPoint = TMP2;
        Debug.Log("after B: " + _finalPoint);
        Debug.Log("before A: " + _startingPoint);
        _startingPoint = TMP;
        Debug.Log("after A: " + _startingPoint);
        _targetRotation = GetTargetRotation(true);
        _isReversed = false;
    }
}
