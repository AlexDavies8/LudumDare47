using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject _launchPrefab = null;

    public void Launch()
    {
        var go = Instantiate(_launchPrefab, transform.position, transform.rotation);
    }
}
