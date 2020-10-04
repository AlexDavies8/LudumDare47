using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBlock : MonoBehaviour
{
    [SerializeField] private Vector2 _offPosition = Vector2.zero;
    [SerializeField] private Vector2 _onPosition = Vector2.up;
    [SerializeField] private float _movementSpeed = 5f;

    bool on;

    private void Update()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, on ? _onPosition : _offPosition, _movementSpeed * Time.deltaTime);
    }

    public void Toggle()
    {
        on = !on;
    }

    public void SetOn()
    {
        on = true;
    }

    public void SetOff()
    {
        on = false;
    }
}
