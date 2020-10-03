using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongFactory : MonoBehaviour
{
    [SerializeField] private GameObject _drumTrackPrefab = null;
    [SerializeField] private GameObject _pianoTrackPrefab = null;

    public Track CreateTrack(TrackData trackData)
    {
        var trackGO = Instantiate(trackData.Range <= 0 ? _drumTrackPrefab : _pianoTrackPrefab);

        var track = trackGO.GetComponent<Track>();

        track.TrackData = trackData;

        track.TrackAudio.clip = trackData.AudioClip;

        track.UpdateSprites();

        return track;
    }
}
