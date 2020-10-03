using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    [SerializeField] private SongData _songData = null;
    [SerializeField] private SongFactory _songFactory = null;
    [SerializeField] private TimeSync _timeSync = null;

    [SerializeField] private Transform _trackSpawnOrigin = null;

    public SongData SongData { get => _songData; set => _songData = value; }

    Track[] _tracks;

    public Track GetTrackById(string id)
    {
        for (int i = 0; i < _tracks.Length; i++)
        {
            if (id == _tracks[i].TrackData.id) return _tracks[i];
        }

        return null;
    }

    public void RegisterNoteCallback(string trackID, Action<int> callback)
    {
        var track = GetTrackById(trackID);
        if (track) track.OnPlayNote += callback;
    }

    private void Awake()
    {
        if (SongData) LoadSong(SongData);
    }

    public void LoadSong(SongData songData)
    {
        if (_tracks != null) DestroyTracks();

        SongData = songData;

        Vector2 prevTrackOffset = Vector2.zero;
        Vector2 trackPosition = _trackSpawnOrigin.position;

        _tracks = new Track[SongData.Tracks.Length];
        for (int i = 0; i < _tracks.Length; i++)
        {
            _tracks[i] = _songFactory.CreateTrack(SongData.Tracks[i]);

            Vector2 trackOffset = _tracks[i].TrackData.Range == 0 ? Vector2.up * 0.5f : Vector2.up;
            trackPosition -= prevTrackOffset + trackOffset;
            prevTrackOffset = trackOffset;

            _tracks[i].transform.position = trackPosition;
        }
        _timeSync.Bpm = SongData.BPM;
    }

    [ContextMenu("Reload Song")]
    public void ReloadSong()
    {
        LoadSong(SongData);
    }

    void DestroyTracks()
    {
        for (int i = 0; i < _tracks.Length; i++)
        {
            Destroy(_tracks[i].gameObject);
        }
    }
}
