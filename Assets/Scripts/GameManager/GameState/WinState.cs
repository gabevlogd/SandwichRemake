using Gabevlogd.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : StateBase<GameManager>
{
    public static event Action<int> LoadNextLevel;

    public WinState(string stateID) : base(stateID)
    {
    }

    public override void OnEnter(GameManager context)
    {
        base.OnEnter(context);
        context.SwipesManager.enabled = false;
        context.UIManager.ChangeWindow(context.UIManager.WinHUD);
    }

    public override void OnUpdate(GameManager context)
    {
        //base.OnUpdate(context);
        CheckNextLevelInput(context);
    }

    public override void OnExit(GameManager context)
    {
        base.OnExit(context);
        LoadNextLevel?.Invoke(context.CurrentLevelIndex + 1);
    }

    private void CheckNextLevelInput(GameManager context)
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f)
            ChangeState(context.Play);
    }
}

