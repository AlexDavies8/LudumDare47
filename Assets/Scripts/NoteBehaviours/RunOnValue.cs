using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunOnValue : MonoBehaviour
{
    [SerializeField] private UnityEvent[] _actions = null;

    public void SetIndex(int index)
    {
        _actions[index].Invoke();
    }
}
