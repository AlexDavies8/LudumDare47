using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMoveBlock : MonoBehaviour
{
    [SerializeField] private float[] _levels = null;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _movementFactor = 2f;

    float _target;

    Rigidbody2D _rb;

    private void Awake()
    {
        _target = _levels[0];

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float current = _rb.position.y;
        float speed = _movementSpeed * Time.deltaTime * Mathf.Max(1f, Mathf.Abs(current - _target) * _movementFactor);
        _rb.position = new Vector2(_rb.position.x, Mathf.MoveTowards(_rb.position.y, _target, speed));
    }

    public void SetTargetIndex(int index)
    {
        _target = _levels[index];
    }
}
