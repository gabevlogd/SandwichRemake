using System;
using Gabevlogd.Patterns;
using UnityEngine;

public class InvalidStackMoveState : StateBase<StacksAnimator>
{
    private Action<StacksAnimator> _performAnimation;
    private Vector3 _point1;
    private Vector3 _point2;

    public InvalidStackMoveState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
    {
    }

    public override void OnEnter(StacksAnimator context)
    {
        base.OnEnter(context);
        context.TargetStack.transform.parent = context.RotationPivot;
        _point2 = context.TargetStack.transform.position;
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
        if (Quaternion.Dot(context.RotationPivot.rotation, context.TargetRotation) < 0.99f)
            context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, context.TargetRotation, Time.deltaTime * context.AngularSpeed * 100);
        else
        {
            context.RotationPivot.rotation = context.TargetRotation;
            _point1 = context.TargetStack.transform.position;
            _performAnimation = StepC;
        }
    }

    private void StepC(StacksAnimator context)
    {
        if (Vector3.Distance(context.TargetStack.transform.position, context.FinalPoint) > 0.01f)
            context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.FinalPoint, Time.deltaTime * context.Speed);
        else
        {
            context.TargetStack.transform.position = context.FinalPoint;
            _performAnimation = StepD;
        }
    }

    private void StepD(StacksAnimator context)
    {
        if (Vector3.Distance(context.TargetStack.transform.position, _point1) > 0.01f)
            context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, _point1, Time.deltaTime * context.Speed);
        else
        {
            context.TargetStack.transform.position = _point1;
            _performAnimation = StepE;

        }
    }

    private void StepE(StacksAnimator context)
    {
        if (Quaternion.Dot(context.RotationPivot.rotation, Quaternion.identity) < 0.99f)
            context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, Quaternion.identity, Time.deltaTime * context.AngularSpeed * 100);
        else
        {
            context.RotationPivot.rotation = Quaternion.identity;
            _performAnimation = StepF;
        }
    }

    private void StepF(StacksAnimator context)
    {
        if (Vector3.Distance(context.TargetStack.transform.position, _point2) > 0.01f)
            context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, _point2, Time.deltaTime * context.Speed);
        else
        {
            context.TargetStack.transform.position = _point2;
            //ChangeState(context.Sleep);
            _stateMachine.ChangeState(context.Sleep);
        }
    }
}
