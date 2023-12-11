using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIWindow
{
    public static event Action PerformUndo;
    public static event Action PerformPause;
    public static event Action PerformRestart;

    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _undoButton;
    [SerializeField]
    private Button _pauseButton;

    private void OnEnable()
    {
        _undoButton.onClick.AddListener(() => PerformUndo?.Invoke());
        _restartButton.onClick.AddListener(() => PerformRestart?.Invoke());
        _pauseButton.onClick.AddListener(() => PerformPause?.Invoke());
    }

    private void OnDisable()
    {
        _undoButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
        _pauseButton.onClick.RemoveAllListeners();
    }
}
