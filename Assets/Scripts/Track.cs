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


    [Header("Colours")]
    [SerializeField] private Color _backgroundLow = Color.white;
    [SerializeField] private Color _offLow = Color.white;
    [SerializeField] private Color _onLow = Color.white;
    [SerializeField] private Color _backgroundHigh = Color.white;
    [SerializeField] private Color _offHigh = Color.white;
    [SerializeField] private Color _onHigh = Color.white;

    int _loopStart, _loopLength;
    float _progress;

    public int _loopStartQueued, _loopLengthQueued;
    public bool _loopQueued = false;

    private void Start()
    {
        _trackHeaderSprite.SetLayerColour(0, _backgroundHigh);
        _trackHeaderSprite.SetLayerColour(1, _onHigh);
        _trackHeaderSprite.SetLayerColour(2, _onLow);

        OnBeat(0f);
        TimeSync.RegisterBeatTimer(1f, OnBeat);
    }

    private void Update()
    {
        _progress = (_progress + TimeSync.DeltaBeat) % _loopLength;
        float progressFrac = (_progress + _loopStart) / _bars.Length;
        if (progressFrac >= 0 && progressFrac <= 1)
            _playbar.position = Vector2.Lerp(_playAreaStart.position, _playAreaEnd.position, progressFrac);
    }

    void OnBeat(float delta)
    {
        if (_loopQueued)
        {
            var totalProgress = _progress + _loopStart;
            if (totalProgress < _loopStartQueued || totalProgress > _loopStartQueued + _loopLengthQueued)
                _progress = 0;
            _loopStart = _loopStartQueued;
            _loopLength = _loopLengthQueued;
            _loopQueued = false;
        }
        _trackAudio.Stop();
        _trackAudio.time = (_loopStart + _progress + delta) / (TimeSync.BPM / 60f);
        _trackAudio.Play();
    }

    public void UpdateBars()
    {
        for (int i = 0; i < _bars.Length; i++)
        {
            var layeredSprite = _bars[i].LayeredSprite;
            if (_loopStart <= i && _loopStart + _loopLength > i)
            {
                layeredSprite.SetLayerColour(0, _backgroundHigh);
            }
            else
            {
                layeredSprite.SetLayerColour(0, _backgroundLow);
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
