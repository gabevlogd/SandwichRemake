using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class SleepState : StateBase<StacksAnimator>
{
    public SleepState(string stateID) : base(stateID)
    {
    }

    public override void OnUpdate(StacksAnimator context)
    {
        //base.OnUpdate(context);
    }
}
