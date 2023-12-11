using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIWindow HUD { get => _HUD; }
    private UIWindow _HUD;
    public UIWindow WinHUD { get => _winHUD; }
    private UIWindow _winHUD;
    public UIWindow Menu { get => _menu; }
    private UIWindow _menu;

    private UIWindow _current;

    private void Awake()
    {
        _HUD = FindObjectOfType<HUD>(true);
        _winHUD = FindObjectOfType<WinHUD>(true);
        _menu = FindObjectOfType<Menu>(true);
    }

    public void ChangeWindow(UIWindow windowToOpen)
    {
        if (_current != null)
            _current.gameObject.SetActive(false);
        _current = windowToOpen;
        _current.gameObject.SetActive(true);
    }

}
