using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class SleepState : StateBase<StacksAnimator>
{
    public SleepState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
    {
    }

    public override void OnEnter(StacksAnimator context)
    {
        base.OnEnter(context);
        context.AnimationCompleted();
    }
}
