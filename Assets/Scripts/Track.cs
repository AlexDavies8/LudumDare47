using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private int _barCount = 8;

    [SerializeField] private LayeredSprite _barSprites = null;
    [SerializeField] private LayeredSprite _noteSprites = null;

    [SerializeField] private AudioSource _trackAudio = null;

    [SerializeField] private Rect _collisionRect = new Rect(new Vector2(-8, -1), new Vector2(16, 2));

    [Header("Appearance")]
    [SerializeField] private Transform _playbar = null;
    [SerializeField] private Transform _playAreaStart = null, _playAreaEnd = null;
    [SerializeField] private LayeredSprite _trackHeaderSprite = null;

    [SerializeField] private Sprite _barSprite = null;

    [SerializeField] private TrackData _trackData = null;

    int _loopStart, _loopLength;
    float _progress = 0f;

    [SerializeField] int _loopStartQueued, _loopLengthQueued;
    [SerializeField] bool _loopQueued = false;

    int _noteIndex = 0;

    Camera _camera;
    bool dragging = false;
    float _dragStart;

    public AudioSource TrackAudio { get => _trackAudio; set => _trackAudio = value; }
    public TrackData TrackData { get => _trackData; set => _trackData = value; }
    public LayeredSprite NoteSprites { get => _noteSprites; set => _noteSprites = value; }
    public Transform PlayAreaStart { get => _playAreaStart; set => _playAreaStart = value; }
    public Transform PlayAreaEnd { get => _playAreaEnd; set => _playAreaEnd = value; }

    public Action<int> OnPlayNote;

    private void Start()
    {
        _camera = Camera.main;

        TimeSync.RegisterBeatTimer(1f, OnBeat);
    }

    public void UpdateSprites()
    {
        _trackHeaderSprite.SetLayerColour(0, TrackData.BackgroundHigh);
        _trackHeaderSprite.SetLayerColour(1, TrackData.OnHigh);
        _trackHeaderSprite.SetLayerColour(2, TrackData.OnLow);
    }

    private void Update()
    {
        _progress += TimeSync.DeltaBeat;
        float progressFrac = (_progress + _loopStart) / _barCount;
        if (progressFrac >= 0 && progressFrac <= 1)
            _playbar.position = Vector2.Lerp(PlayAreaStart.position, PlayAreaEnd.position, progressFrac);
        if (TrackData.Notes.Length > 0 && _noteIndex < TrackData.Notes.Length && _progress + _loopStart - 0.05f >= TrackData.Notes[_noteIndex].time * TrackData.LengthMultiplier)
        {
            PlayNote(_noteIndex);
            _noteIndex = _noteIndex + 1;
        }


        if (Input.GetMouseButtonDown(0) || dragging)
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var localMousePos = transform.InverseTransformPoint(mousePos);
            Debug.DrawRay(mousePos, Vector2.one, Color.red);
            if (_collisionRect.Contains(localMousePos))
            {
                dragging = true;
                if (Input.GetMouseButtonDown(0))
                {
                    _dragStart = ((localMousePos.x / _barCount) + 1) / 2 * _barCount;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                var loopEndQueued = ((localMousePos.x / _barCount) + 1) / 2 * _barCount;
                var loopStartQueued = _dragStart;
                var max = Mathf.CeilToInt(Mathf.Max(loopStartQueued, loopEndQueued));
                var min = Mathf.FloorToInt(Mathf.Min(loopStartQueued, loopEndQueued));
                _loopStartQueued = min;
                _loopLengthQueued = Mathf.Abs(max - min);
                _loopQueued = true;
                UpdateBars();
                UpdateNotes();
            }
        }
    }

    public void UpdateBars()
    {
        if (_barSprites.Layers.Length != _barCount)
        {
            _barSprites.Layers = new LayeredSprite.Layer[_barCount];
            for (int i = 0; i < _barCount; i++)
            {
                _barSprites.Layers[i] = new LayeredSprite.Layer() { Sprite = _barSprite, Offset = (Vector2)PlayAreaStart.localPosition + i * 2 * Vector2.right };
            }
            _barSprites.RebuildLayers();
        }

        for (int i = 0; i < _barCount; i++)
        {
            _barSprites.SetLayerColour(i, _loopStartQueued <= i && _loopStartQueued + _loopLengthQueued > i ? TrackData.BackgroundHigh : TrackData.BackgroundLow);
        }
    }

    public void UpdateNotes()
    {
        for (int i = 0; i < TrackData.Notes.Length; i++)
        {
            float timeStart = TrackData.Notes[i].time * TrackData.LengthMultiplier;
            bool noteHigh = timeStart >= _loopStartQueued && timeStart < _loopStartQueued + _loopLengthQueued;
            _noteSprites.SetLayerColour(i, noteHigh ? TrackData.OnHigh : TrackData.OffHigh);
        }
    }

    void PlayNote(int index)
    {
        var time = TrackData.Notes[index].time * TrackData.LengthMultiplier;
        if (time < _loopStart || time >= _loopStart + _loopLength) return;
        OnPlayNote?.Invoke(TrackData.Notes[index].value);
    }

    void OnBeat(float delta)
    {
        var oldProgress = _progress;
        _progress = (Mathf.Round(_progress) + delta) % _loopLength;
        if (float.IsNaN(_progress)) _progress = 0;

        if (_loopQueued)
        {
            _progress = 0;
            _loopStart = _loopStartQueued;
            _loopLength = _loopLengthQueued;
            _loopQueued = false;

            UpdateBars();
            UpdateNotes();
        }

        if (TrackAudio)
        {
            TrackAudio.Stop();
            var trackTime = Mathf.Clamp((_loopStart + _progress) / (TimeSync.BPM / 60f), 0f, TrackAudio.clip.length - 0.01f);
            if (float.IsNaN(trackTime)) trackTime = 0;
            TrackAudio.time = trackTime;
            TrackAudio.Play();
        }

        int prevIndex = _noteIndex;
        FindNoteIndex();
        if (prevIndex < _noteIndex)
        {
            PlayNote(prevIndex);
        }
        if (prevIndex > _noteIndex || (prevIndex == _noteIndex && oldProgress > _progress + delta))
        {
            PlayNote((_noteIndex + TrackData.Notes.Length - 1) % TrackData.Notes.Length);
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + _collisionRect.position + _collisionRect.size * 0.5f, _collisionRect.size);
    }
}
