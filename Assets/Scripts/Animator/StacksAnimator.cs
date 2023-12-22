using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class StacksAnimator : MonoBehaviour
{
    public static event Action AnimationEnded;
    public static event Action<SwipeableObject, SwipeableObject, Vector3> RegisterMove;

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
    public SleepState Sleep;
    public StackMoveState StackMove;
    public InvalidStackMoveState InvalidStackMove;
    


    private void Awake()
    {
        _rotationPivot = transform.GetChild(0);
        InitializeStateMachine();
    }

    private void OnEnable()
    {
        SwipeableObject.RunAnimation += RunAnimation;
        UndoManager.RunAnimation += RunAnimation;
    }

    private void OnDisable()
    {
        SwipeableObject.RunAnimation -= RunAnimation;
        UndoManager.RunAnimation -= RunAnimation;
    }

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
        if (animation == Constants.STACK_MOVE) RegisterMove?.Invoke(from, to, _rotationPivot.position);
    }

    private void RunAnimation(string animation, SwipeableObject from, SwipeableObject to)
    {
        StateBase<StacksAnimator> animationToTrigger = null;
        foreach(var state in _stateMachine.StateList)
        {
            if (animation != state.StateID) continue;
            animationToTrigger = state;
            break;
        }
        if (animationToTrigger == null)
        {
            Debug.LogError("Invalid animation");
            return;
        }

        CalculateAnimationData(animation, from, to);
        _stateMachine.ChangeState(animationToTrigger);
    }

    private void RunAnimation(Transform parent, GameObject targetStack, Quaternion targetRotation, Vector3 pivotPosition, Vector3 startingPoint, Vector3 finalPoint)
    {
        _rotationPivot.SetPositionAndRotation(pivotPosition, Quaternion.identity);
        _targetStack = targetStack;
        _originalParent = parent;
        _targetRotation = targetRotation;
        _startingPoint = startingPoint;
        _finalPoint = finalPoint;
        _stateMachine.ChangeState(StackMove);
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

        return SwipesManager.SwipeDirection switch
        {
            SwipeDirection.Up => new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z + GridHandler.Grid.GetCellSize() * 0.5f),
            SwipeDirection.Down => new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z - GridHandler.Grid.GetCellSize() * 0.5f),
            SwipeDirection.Right => new Vector3(from.transform.position.x + GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z),
            SwipeDirection.Left => new Vector3(from.transform.position.x - GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z),
            _ => Vector3.zero,
        };
    }

    private Quaternion GetTargetRotation()
    {
        return SwipesManager.SwipeDirection switch
        {
            SwipeDirection.Up => Quaternion.Euler(-180f, 0f, 0f),
            SwipeDirection.Down => Quaternion.Euler(180f, 0f, 0f),
            SwipeDirection.Right => Quaternion.Euler(0f, 0f, 180f),
            SwipeDirection.Left => Quaternion.Euler(0f, 0f, -180f),
            _ => Quaternion.identity,
        };
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

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine<StacksAnimator>(this);
        InitializeStates();
        _stateMachine.AddState(Sleep);
        _stateMachine.AddState(StackMove);
        _stateMachine.AddState(InvalidStackMove);
        _stateMachine.RunStateMachine(Sleep, this);
    }

    private void InitializeStates()
    {
        Sleep = new SleepState(Constants.SLEEP, _stateMachine);
        StackMove = new StackMoveState(Constants.STACK_MOVE, _stateMachine);
        InvalidStackMove = new InvalidStackMoveState(Constants.INVALID_MOVE, _stateMachine);
    }
}
