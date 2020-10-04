using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour
{
    public float _movementSpeed = 3f;
    public LayerMask _groundMask = 0;

    public Vector2 _moveVector;

    public GameObject _breakEffect = null;

    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.velocity = _moveVector * _movementSpeed;
        _rb.rotation += (_movementSpeed * 360 * 2 * Time.deltaTime);

        if (Physics2D.Raycast(transform.position, _moveVector, 0.55f, _groundMask))
        {
            Instantiate(_breakEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
