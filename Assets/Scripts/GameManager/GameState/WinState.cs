using Gabevlogd.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : StateBase<GameManager>
{
    public static event Action LoadNextLevel;

    public WinState(string stateID, StateMachine<GameManager> stateMachine) : base(stateID, stateMachine)
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
        LoadNextLevel?.Invoke();
    }

    private void CheckNextLevelInput(GameManager context)
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f && touch.deltaPosition.magnitude > 30f)
            _stateMachine.ChangeState(context.Play);
    }
}

