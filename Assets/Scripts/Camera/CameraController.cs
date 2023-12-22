using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _targetPosition;
    [SerializeField]
    private float _speed;
    private bool _canMove;
    private Vector3 _startPosition;

    private void Awake() => _startPosition = transform.position;

    private void OnEnable()
    {
        LevelLoader.LevelLoaded += ResetPosition;
        SwipeableObject.TriggerCamera += TriggerMovement;
    }

    private void OnDisable()
    {
        LevelLoader.LevelLoaded -= ResetPosition;
        SwipeableObject.TriggerCamera -= TriggerMovement;
    }

    private void Update()
    {
        if (_canMove) MoveCamera();
    }

    private void MoveCamera()
    {
        if (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
        else
        {
            transform.position = _targetPosition;
            _canMove = false;
        }

    }

    private void TriggerMovement(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _canMove = true;
    }

    private void ResetPosition(LevelData value) => transform.position = _startPosition;
}
