using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3f;
    [SerializeField] private float _jumpHeight = 3f;

    [SerializeField] private Transform _wallCheck = null;
    [SerializeField] private float _wallCheckRadius = 0.2f;
    [SerializeField] private Transform _groundCheck = null;
    [SerializeField] private float _groundCheckRadius = 0.2f;

    [SerializeField] private GameObject _killEffect = null;
    [SerializeField] private GameObject _winEffect = null;

    [SerializeField] private LayerMask _killMask = 0;
    [SerializeField] private LayerMask _winMask = 0;

    [SerializeField] private string _nextLevelName = "Scene";

    [SerializeField] private LayerMask _groundMask = 0;

    Rigidbody2D _rb;

    int _direction = 1;

    Vector2 old;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.velocity = new Vector2(_direction * _movementSpeed, _rb.velocity.y);

        if (Physics2D.OverlapCircle(_wallCheck.transform.position, _wallCheckRadius, _groundMask.value))
        {
            _direction = -_direction;

            transform.rotation = Quaternion.Euler(0, 90 - _direction * 90, 0);
        }
    }

    public void Jump()
    {
        if (Physics2D.OverlapCircle(_groundCheck.transform.position, _groundCheckRadius, _groundMask.value))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpHeight);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _killMask) > 0)
        {
            FindObjectOfType<SceneController>().ReloadScene(1f);
            Instantiate(_killEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _winMask) > 0)
        {
            FindObjectOfType<SceneController>().LoadScene(_nextLevelName, 1f);
            Instantiate(_winEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
