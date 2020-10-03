using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private Bar[] _bars = new Bar[8];

    [SerializeField] private AudioSource _trackAudio = null;

    [Header("Appearance")]
    [SerializeField] private Transform _playbar = null;
    [SerializeField] private Transform _playAreaStart = null, _playAreaEnd = null;
    [SerializeField] private LayeredSprite _trackHeaderSprite = null;

    [SerializeField] private TrackData _trackData = null;

    int _loopStart, _loopLength;
    float _progress = 0f;

    [SerializeField] int _loopStartQueued, _loopLengthQueued;
    [SerializeField] bool _loopQueued = false;

    int _noteIndex = 0;

    public AudioSource TrackAudio { get => _trackAudio; set => _trackAudio = value; }
    public TrackData TrackData { get => _trackData; set => _trackData = value; }

    public Action<int> OnPlayNote;

    private void Start()
    {
        OnBeat(0f);
        TimeSync.RegisterBeatTimer(1f, OnBeat);
    }

    public void UpdateSprites()
    {
        _trackHeaderSprite.SetLayerColour(0, TrackData.BackgroundHigh);
        _trackHeaderSprite.SetLayerColour(1, TrackData.OnHigh);
        _trackHeaderSprite.SetLayerColour(2, TrackData.OnLow);

        UpdateBars();
    }

    private void Update()
    {
        _progress += TimeSync.DeltaBeat;
        float progressFrac = (_progress + _loopStart) / _bars.Length;
        if (progressFrac >= 0 && progressFrac <= 1)
            _playbar.position = Vector2.Lerp(_playAreaStart.position, _playAreaEnd.position, progressFrac);
        if (TrackData.Notes.Length > 0 && _noteIndex < TrackData.Notes.Length && _progress + _loopStart >= TrackData.Notes[_noteIndex].time * TrackData.LengthMultiplier)
        {
            PlayNote(TrackData.Notes[_noteIndex].value);
            _noteIndex = _noteIndex + 1;
        }
    }

    void PlayNote(int value)
    {
        OnPlayNote?.Invoke(value);
    }

    void OnBeat(float delta)
    {
        _progress = (Mathf.Round(_progress) + delta) % _loopLength;
        if (float.IsNaN(_progress)) _progress = 0;

        if (_loopQueued)
        {
            var totalProgress = _progress + _loopStart;
            if (totalProgress < _loopStartQueued || totalProgress > _loopStartQueued + _loopLengthQueued)
                _progress = 0;
            _loopStart = _loopStartQueued;
            _loopLength = _loopLengthQueued;
            _loopQueued = false;
        }

        if (TrackAudio)
        {
            TrackAudio.Stop();
            TrackAudio.time = (_loopStart + _progress) / (TimeSync.BPM / 60f);
            TrackAudio.Play();
        }

        FindNoteIndex();
    }

    void FindNoteIndex()
    {
        float totalProgress = _progress + _loopStart;

        for (int i = 0; i < TrackData.Notes.Length; i++)
        {
            float noteTime = TrackData.Notes[i].time * TrackData.LengthMultiplier;
            if (noteTime >= totalProgress)
            {
                _noteIndex = i;
                return;
            }
        }
    }

    public void UpdateBars()
    {
        for (int i = 0; i < _bars.Length; i++)
        {
            if (_bars[i] == null) continue;

            var layeredSprite = _bars[i].LayeredSprite;
            if (_loopStart <= i && _loopStart + _loopLength > i)
            {
                layeredSprite.SetLayerColour(0, TrackData.BackgroundHigh);
            }
            else
            {
                layeredSprite.SetLayerColour(0, TrackData.BackgroundLow);
            }
        }
    }

    public void SetLoopStart(int loopStart)
    {
        _loopQueued = true;
        _loopStartQueued = loopStart;
    }

    public void SetLoopLength(int loopLength)
    {
        _loopQueued = true;
        _loopLengthQueued = loopLength;
    }
}
