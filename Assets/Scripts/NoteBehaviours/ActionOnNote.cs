using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionOnNote : MonoBehaviour
{
    public string trackID = "track";
    public UnityEvent<int> action = new UnityEvent<int>(); 

    private void Start()
    {
        FindObjectOfType<SongManager>().RegisterNoteCallback(trackID, value => action.Invoke(value));
    }
}
