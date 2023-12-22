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
    public PauseState Pause;
    public PlayState Play;
    public WinState Win;

    public int CurrentLevelIndex { get => _currentLevelIndex; }
    private int _currentLevelIndex;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        _UIManager = FindObjectOfType(typeof(UIManager)) as UIManager;
        _swipesManager = FindObjectOfType(typeof(SwipesManager)) as SwipesManager;
        InitializeStateMachine();
    }

    private void OnEnable()
    {
        HUD.PerformPause += () => _stateMachine.ChangeState(Pause);
        SwipeableObject.GameWon += () => _stateMachine.ChangeState(Win);
        LevelLoader.LevelLoaded += (LevelData data) => _currentLevelIndex = data.LevelIndex;
    }

    private void Update() => _stateMachine.CurrentState.OnUpdate(this);

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine<GameManager>(this);
        InitializeStates();
        _stateMachine.AddState(Play);
        _stateMachine.AddState(Pause);
        _stateMachine.AddState(Win);
        _stateMachine.RunStateMachine(Pause, this);
    }

    private void InitializeStates()
    {
        Pause = new PauseState(Constants.PAUSE, _stateMachine);
        Play = new PlayState(Constants.PLAY, _stateMachine);
        Win = new WinState(Constants.WIN, _stateMachine);
    }

}
