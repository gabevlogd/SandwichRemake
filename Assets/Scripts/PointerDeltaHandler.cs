using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerDeltaHandler : MonoBehaviour
{
    public static Vector2 DeltaPos { get => _deltaPos; }
    private static Vector2 _deltaPos;
    private Vector2 _startPos;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = Input.mousePosition;
                _deltaPos = Vector2.zero;
                return;
            }
            _deltaPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _startPos;
        }
        else
        {
            _deltaPos = Vector2.zero;
        }
    }


}
