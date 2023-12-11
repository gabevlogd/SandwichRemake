using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static event Action<Transform, GameObject, Quaternion, Vector3, Vector3, Vector3> RunAnimation;
    private List<InverseMove> _inverseMovesList;
    private bool _canPerformUndo;


    private void Awake()
    {
        _inverseMovesList = new List<InverseMove>();
        StacksAnimator.RegisterMove += RegisterMove;
        StacksAnimator.AnimationEnded += () => _canPerformUndo = true;
        HUD.PerformUndo += UndoLastMove;
        LevelLoader.LevelLoaded += (LevelData value) => _inverseMovesList = new List<InverseMove>();
    }


    public void UndoLastMove()
    {
        if (_inverseMovesList.Count == 0 || !_canPerformUndo) return;
        _canPerformUndo = false;
        InverseMove tmp = _inverseMovesList[^1];
        RunAnimation?.Invoke(tmp.Parent, tmp.Stack, tmp.TargetRotation, tmp.RotationPivotPosition, tmp.StartingPoint, tmp.FinalPoint);
        tmp.Itself.Data.Stack = tmp.Stack;
        tmp.Itself.Data.StackCount = tmp.StackCount;
        tmp.Itself.Data.This = tmp.Itself;
        tmp.CameFrom.Data.StackCount -= tmp.StackCount;
        _inverseMovesList.RemoveAt(_inverseMovesList.Count - 1);
    }

    private void RegisterMove(SwipeableObject from, SwipeableObject to, Vector3 pivotPosition)
    {
        _inverseMovesList.Add(new InverseMove(from, to, GetStartingPoint(from, to), pivotPosition, GetTargetRotation()));
    }

    private Vector3 GetStartingPoint(SwipeableObject from, SwipeableObject to)
    {
        float singleStackHeight = from.Data.Stack.transform.lossyScale.y;
        if (from.Data.StackCount <= to.Data.StackCount)
            return to.transform.position + Vector3.up * (singleStackHeight * (from.Data.StackCount + to.Data.StackCount - 1));
        else return to.transform.position + Vector3.up * (singleStackHeight * (2 * from.Data.StackCount - 1));

    }

    private Quaternion GetTargetRotation()
    {
        return SwipesManager.SwipeDirection switch
        {
            SwipeDirection.Up => Quaternion.Euler(180f, 0f, 0f),
            SwipeDirection.Down => Quaternion.Euler(-180f, 0f, 0f),
            SwipeDirection.Right => Quaternion.Euler(0f, 0f, -180f),
            SwipeDirection.Left => Quaternion.Euler(0f, 0f, 180f),
            _ => Quaternion.identity,
        };
    }
}
