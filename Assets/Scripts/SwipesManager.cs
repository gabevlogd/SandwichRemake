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

    private void Awake()
    {
        _camera = Camera.main;
        _grid = GridHandler.Grid;
        StacksAnimator.AnimationEnded += ResetSwipes;

    }

    private void Update() => SwipesCheck();

    private void SwipesCheck()
    {
        if (Input.touchCount != 1) return;
        if (_start != null && _end != null) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TryCacheSwipeData(touch, out _start);
                break;
            case TouchPhase.Ended:

                if (_start == null) return;
                _end = GetNeighbour(touch);
                if (_end != null) TriggerStackMoevement?.Invoke(_start, _end);
                else ResetSwipes();

                break;
            default:
                return;
        }
            
    }

    private void TryCacheSwipeData(Touch touch, out SwipeableObject swipeData)
    {
        Vector3 pointedWorldPosition = GetScreenToWorld(touch.position);
        if (_grid.GetGridObject(pointedWorldPosition) != null)
            swipeData = _grid.GetGridObject(pointedWorldPosition).This;
        else swipeData = null;
    }

    private SwipeableObject GetNeighbour(Touch touch)
    {
        Vector3 swipeDirection = (GetScreenToWorld(touch.position) - _grid.GetWorldPosition(_start.Data.Column, _start.Data.Row)).normalized;
        SwipeableObjectData pointedObj = null;
        if (Vector3.Dot(swipeDirection, Vector3.right) > 0.8f)
        {
            pointedObj = _grid.GetGridObject(_start.Data.Column + 1, _start.Data.Row);
            SwipeDirection = SwipeDirection.Right;
        }
        if (Vector3.Dot(swipeDirection, Vector3.left) > 0.8f)
        {
            pointedObj = _grid.GetGridObject(_start.Data.Column - 1, _start.Data.Row);
            SwipeDirection = SwipeDirection.Left;
        }
        if ((Vector3.Dot(swipeDirection, Vector3.forward) > 0.8f))
        {
            pointedObj = _grid.GetGridObject(_start.Data.Column, _start.Data.Row + 1);
            SwipeDirection = SwipeDirection.Up;
        }
        if ((Vector3.Dot(swipeDirection, Vector3.back) > 0.8f))
        {
            pointedObj = _grid.GetGridObject(_start.Data.Column, _start.Data.Row - 1);
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
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);
        Debug.DrawRay(_camera.ScreenPointToRay(touch.position).origin, _camera.ScreenPointToRay(touch.position).direction * 100f, Color.green);
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
