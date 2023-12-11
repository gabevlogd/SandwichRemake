using Gabevlogd.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : StateBase<GameManager>
{
    public PauseState(string stateID) : base(stateID)
    {
    }

    public override void OnEnter(GameManager context)
    {
        base.OnEnter(context);
        context.SwipesManager.enabled = false;
        context.UIManager.ChangeWindow(context.UIManager.Menu);
    }

    public override void OnUpdate(GameManager context)
    {
        //base.OnUpdate(context);
        ChecksForResumeGame(context);
    }

    private void ChecksForResumeGame(GameManager context)
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f)
            ChangeState(context.Play);
    }

}
