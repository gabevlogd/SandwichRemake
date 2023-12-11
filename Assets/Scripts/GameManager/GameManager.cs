using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gabevlogd.Patterns;

public class GameManager : MonoBehaviour
{
    public SwipesManager SwipesManager { get => _swipesManager; }
    private SwipesManager _swipesManager;
    public UIManager UIManager { get => _UIManager; }
    private UIManager _UIManager;

    private StateMachine<GameManager> _stateMachine;
    public readonly PauseState Pause = new PauseState(Constants.PAUSE);
    public readonly PlayState Play = new PlayState(Constants.PLAY);
    public readonly WinState Win = new WinState(Constants.WIN);

    public int CurrentLevelIndex { get => _currentLevelIndex; }
    private int _currentLevelIndex;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        _UIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        _swipesManager = FindObjectOfType(typeof(SwipesManager)) as SwipesManager;
        _stateMachine = new StateMachine<GameManager>(this);
        _stateMachine.AddState(Play);
        _stateMachine.AddState(Pause);
        _stateMachine.AddState(Win);
        HUD.PerformPause += () => _stateMachine.ChangeState(Pause);
        SwipeableObject.GameWon += () => _stateMachine.ChangeState(Win);
        LevelLoader.LevelLoaded += (LevelData data) => _currentLevelIndex = data.LevelIndex;
    }

    private void Start() => _stateMachine.RunStateMachine(Pause);

    private void Update() => _stateMachine.CurrentState.OnUpdate(this);

}
