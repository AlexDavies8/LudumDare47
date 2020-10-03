using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Song Data", fileName = "Song Data")]
public class SongData : ScriptableObject
{
    public float BPM = 120;
    public TrackData[] Tracks = new TrackData[1];
}
