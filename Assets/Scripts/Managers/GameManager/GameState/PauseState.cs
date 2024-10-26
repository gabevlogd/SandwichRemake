using Gabevlogd.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : StateBase<GameManager>
{

    public PauseState(string stateID, StateMachine<GameManager> stateMachine) : base(stateID, stateMachine)
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
#if PLATFORM_WEBGL
        if (Vector2.Dot(PointerDeltaHandler.DeltaPos.normalized, Vector2.right) > 0.99f && PointerDeltaHandler.DeltaPos.magnitude > 30f)
#else
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f && touch.deltaPosition.magnitude > 30f)
#endif
        {
            SoundManager.Play(Constants.SWIPE);
            _stateMachine.ChangeState(context.Play);
        }
    }

}
