using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipesManager : MonoBehaviour
{
    private Grid<SwipeableObjectData> _grid;
    private Camera _camera;
    private SwipeableObject _start;
    private SwipeableObject _end;

    public static event Action<SwipeableObject, SwipeableObject> TriggerStackMoevement;
    public static SwipeDirection SwipeDirection;

    private void Awake() => _camera = Camera.main;

    private void OnEnable()
    {
        _grid = GridHandler.Grid;
        ResetSwipes();
        StacksAnimator.AnimationEnded += ResetSwipes;
    }

    private void OnDisable() => StacksAnimator.AnimationEnded -= ResetSwipes;

    private void Update() => SwipesCheck();

    private void SwipesCheck()
    {

        if (_start != null && _end != null) return;
        if (Input.GetMouseButtonDown(0))
        {
            TryCacheSwipeData(Input.mousePosition, out _start);
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_start == null) return;
            _end = GetNeighbour(Input.mousePosition);
            if (_end != null) TriggerStackMoevement?.Invoke(_start, _end);
            else ResetSwipes();
            return;
        }


    }

    private void TryCacheSwipeData(Vector2 screenPos, out SwipeableObject swipeData)
    {
        Vector3 pointedWorldPosition = GetScreenToWorld(screenPos);
        if (_grid.GetGridObject(pointedWorldPosition) != null)
            swipeData = _grid.GetGridObject(pointedWorldPosition).This;
        else swipeData = null;
    }

    private SwipeableObject GetNeighbour(Vector2 screenPos)
    {
        Vector3 swipeDirection = (GetScreenToWorld(screenPos) - _grid.GetWorldPosition(_start.Data.Column, _start.Data.Row)).normalized;
        SwipeableObjectData pointedObj = null;
        if (Vector3.Dot(swipeDirection, Vector3.right) > 0.8f)
        {
            pointedObj = _grid.GetGridObject(_start.Data.Row, _start.Data.Column + 1);
            SwipeDirection = SwipeDirection.Right;
        }
        else if (Vector3.Dot(swipeDirection, Vector3.left) > 0.8f)
        {
            pointedObj = _grid.GetGridObject(_start.Data.Row, _start.Data.Column - 1);
            SwipeDirection = SwipeDirection.Left;
        }
        else if ((Vector3.Dot(swipeDirection, Vector3.forward) > 0.8f))
        {
            pointedObj = _grid.GetGridObject(_start.Data.Row + 1, _start.Data.Column);
            SwipeDirection = SwipeDirection.Up;
        }
        else if ((Vector3.Dot(swipeDirection, Vector3.back) > 0.8f))
        {
            pointedObj = _grid.GetGridObject(_start.Data.Row - 1, _start.Data.Column);
            SwipeDirection = SwipeDirection.Down;
        }

        if (pointedObj != null)
            return pointedObj.This;
        else return null;
    }

    private Vector3 GetScreenToWorld(Vector2 screenPos)
    {
        Ray ray = _camera.ScreenPointToRay(screenPos);
        Physics.Raycast(ray, out RaycastHit hit);
        return new Vector3(hit.point.x, 0f, hit.point.z);
    }

    private void ResetSwipes()
    {
        _start = null;
        _end = null;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() => DisplayPointerDebug();
    private void DisplayPointerDebug()
    {
        Debug.DrawRay(_camera.ScreenPointToRay(Input.mousePosition).origin, _camera.ScreenPointToRay(Input.mousePosition).direction * 100f, Color.green);
    }
#endif



}

public enum SwipeDirection
{
    Up,
    Down,
    Right,
    Left
}
