using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class StacksAnimator : MonoBehaviour
{
    public static event Action AnimationEnded;

    public GameObject TargetStack { get => _targetStack; }
    private GameObject _targetStack;
    public Transform OriginalParent { get => _originalParent; }
    private Transform _originalParent;
    public Transform RotationPivot { get => _rotationPivot; }
    private Transform _rotationPivot;

    public Quaternion TargetRotation { get => _targetRotation; }
    private Quaternion _targetRotation;
    public Vector3 StartingPoint { get => _startingPoint; }
    private Vector3 _startingPoint;
    public Vector3 FinalPoint { get => _finalPoint; }
    private Vector3 _finalPoint;

    public float Speed { get => _speed; }
    [SerializeField]
    private float _speed;
    public float AngularSpeed { get => _angularSpeed; }
    [SerializeField]
    private float _angularSpeed;
    private float _singleStackHeight;


    private StateMachine<StacksAnimator> _stateMachine;
    public readonly SleepState Sleep = new SleepState(Constants.SLEEP);
    public readonly StackMoveState StackMove = new StackMoveState(Constants.STACK_MOVE);
    public readonly InvalidStackMoveState InvalidStackMove = new InvalidStackMoveState(Constants.INVALID_MOVE);
    


    private void Awake()
    {
        _rotationPivot = transform.GetChild(0);
        _stateMachine = new StateMachine<StacksAnimator>(this);
        _stateMachine.AddState(Sleep);
        _stateMachine.AddState(StackMove);
        _stateMachine.AddState(InvalidStackMove);
        SwipeableObject.RunAnimation += RunAnimation;
    }

    private void Start() => _stateMachine.RunStateMachine(Sleep);

    private void Update() => _stateMachine.CurrentState.OnUpdate(this);

    private void CalculateAnimationData(string animation, SwipeableObject from, SwipeableObject to)
    {
        //get stack to move
        _targetStack = from.Data.Stack;
        if (animation == InvalidStackMove.StateID)
            _originalParent = from.Data.Stack.transform.parent;
        else _originalParent = to.Data.Stack.transform;
        //get the height of the single element of a stack
        _singleStackHeight = _targetStack.transform.lossyScale.y;
        _rotationPivot.position = GetRotationPivotPosition(from, to);
        _targetRotation = GetTargetRotation();
        //get the first position which to traslate the stack
        _startingPoint = GetStartingPoint(from, to);
        //get the last position which to traslate the stack
        _finalPoint = GetFinalPoint(from, to);
    }

    public void RunAnimation(string animation, SwipeableObject from, SwipeableObject to)
    {
        StateBase<StacksAnimator> animationToTrigger = null;
        foreach(var state in _stateMachine.StateList)
        {
            if (animation != state.StateID) continue;
            animationToTrigger = state;
        }
        if (animationToTrigger == null)
        {
            Debug.LogError("Invalid animation");
            return;
        }

        CalculateAnimationData(animation, from, to);
        _stateMachine.ChangeState(animationToTrigger);
    }

    public void AnimationCompleted() => AnimationEnded?.Invoke();

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

    private Quaternion GetTargetRotation()
    {
        switch (SwipesManager.SwipeDirection)
        {
            case SwipeDirection.Up:
                return Quaternion.Euler(-180f, 0f, 0f);
            case SwipeDirection.Down:
                return Quaternion.Euler(180f, 0f, 0f);
            case SwipeDirection.Right:
                return Quaternion.Euler(0f, 0f, 180f);
            case SwipeDirection.Left:
                return Quaternion.Euler(0f, 0f, -180f);
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
}
