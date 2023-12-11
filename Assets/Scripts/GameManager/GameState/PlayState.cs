using Gabevlogd.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : StateBase<GameManager>
{
    public PlayState(string stateID) : base(stateID)
    {
    }

    public override void OnEnter(GameManager context)
    {
        base.OnEnter(context);
        context.SwipesManager.enabled = true;
        context.UIManager.ChangeWindow(context.UIManager.HUD);
    }
}
