using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;
using System;

public class StackMoveState : StateBase<StacksAnimator>
{
    private Action<StacksAnimator> _performAnimation;

    public StackMoveState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
    {
    }

    public override void OnEnter(StacksAnimator context)
    {
        base.OnEnter(context);
        SoundManager.Play(Constants.SWIPE);
        context.TargetStack.transform.parent = context.RotationPivot;
        _performAnimation = StepA;
    }

    public override void OnUpdate(StacksAnimator context)
    {
        //base.OnUpdate(context);
        _performAnimation?.Invoke(context);
    }

    public override void OnExit(StacksAnimator context)
    {
        base.OnExit(context);
        context.TargetStack.transform.parent = context.OriginalParent;
        _performAnimation = null;
        //context.AnimationCompleted();
    }

    private void StepA(StacksAnimator context)
    {
        if (Vector3.Distance(context.TargetStack.transform.position, context.StartingPoint) > 0.01f)
            context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.StartingPoint, Time.deltaTime * context.Speed);
        else
        {
            context.TargetStack.transform.position = context.StartingPoint;
            _performAnimation = StepB;
        }
    }
    private void StepB(StacksAnimator context)
    {
        //if (Quaternion.Dot(context.TargetStack.transform.rotation, context.TargetRotation) < 0.99f)
        //    context.TargetStack.transform.RotateAround(context.RotationPivot.position, Vector3.right, Time.deltaTime * context.AngularSpeed * 100f);
        if (Quaternion.Dot(context.RotationPivot.rotation, context.TargetRotation) < 0.99f)
            context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, context.TargetRotation, Time.deltaTime * context.AngularSpeed * 100);
        else
        {
            context.RotationPivot.rotation = context.TargetRotation;
            _performAnimation = StepC;
        }
    }

    private void StepC(StacksAnimator context)
    {
        if (Vector3.Distance(context.TargetStack.transform.position, context.FinalPoint) > 0.01f)
            context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.FinalPoint, Time.deltaTime * context.Speed);
        else
        {
            SoundManager.Play(Constants.SLICE);
            context.TargetStack.transform.position = context.FinalPoint;
            //ChangeState(context.Sleep);
            _stateMachine.ChangeState(context.Sleep);
        }
    }
}
