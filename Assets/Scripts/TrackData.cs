using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Track Data", fileName = "Track Data")]
public class TrackData : ScriptableObject
{
    public string id = "track";

    [Header("Audio")]
    public AudioClip AudioClip = null;

    [Header("Notes")]
    public float LengthMultiplier = 0.25f; // Length of each unit of 'time'
    public int Range = 0; // 0 = Drum Sequencer, Else use Piano Roll

    public Note[] Notes = new Note[0];

    [Header("Appearance")]
    public Color BackgroundLow = Color.white;
    public Color OffLow = Color.white;
    public Color OnLow = Color.white;
    public Color BackgroundHigh = Color.white;
    public Color OffHigh = Color.white;
    public Color OnHigh = Color.white;
}
