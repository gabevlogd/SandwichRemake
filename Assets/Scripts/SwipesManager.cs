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
    public static event Action<SwipeableObject, SwipeableObject> TriggerSwipeableObjMoevement;

    private void Awake()
    {
        _camera = Camera.main;
        _grid = GridHandler.Grid;
        SwipeableObject.OnSwipeableObjMovementEnded += ResetSwipes;
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
                if (_start != null)
                    TryCacheSwipeData(touch, out _end);
                break;
            default:
                return;
        }

        if (_start != null && _end != null)
        {
            if (_start != _end && AreNeighbour(_start, _end))
                TriggerSwipeableObjMoevement?.Invoke(_start, _end);
            else ResetSwipes();
        }
            
    }

    private void TryCacheSwipeData(Touch touch, out SwipeableObject swipeData)
    {
        Vector3 pointedWorldPosition = GetScreenToWorld(touch.position);
        if (_grid.GetGridObject(pointedWorldPosition) != null)
            swipeData = _grid.GetGridObject(pointedWorldPosition).This;
        
        else swipeData = null;
    }

    private bool AreNeighbour(SwipeableObject objA, SwipeableObject objB) => Vector2Int.Distance(new Vector2Int(objA.Data.Row, objA.Data.Column), new Vector2Int(objB.Data.Row, objB.Data.Column)) <= _grid.GetCellSize();
    

    private Vector3 GetScreenToWorld(Vector2 screenPos)
    {
        Ray ray = _camera.ScreenPointToRay(screenPos);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
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
