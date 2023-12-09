using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class GameManager : MonoBehaviour
{
    private StateMachine<GameManager> _stateMachine;
    public readonly PauseState Pause = new PauseState(Constants.PAUSE);
    public readonly PlayState Play = new PlayState(Constants.PLAY);
    public readonly WinState Win = new WinState(Constants.WIN);

    private void Awake()
    {
        _stateMachine = new StateMachine<GameManager>(this);
        _stateMachine.AddState(Play);
        _stateMachine.AddState(Pause);
        _stateMachine.AddState(Win);
        _stateMachine.RunStateMachine(Pause);
    }

    private void Update() => _stateMachine.CurrentState.OnUpdate(this);

}
