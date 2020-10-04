using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongFactory : MonoBehaviour
{
    [SerializeField] private int _barCount = 8;

    [SerializeField] private GameObject _drumTrackPrefab = null;
    [SerializeField] private GameObject _pianoTrackPrefab = null;

    [SerializeField] private GameObject _notePrefab = null;

    [SerializeField] private Sprite _noteSpriteDrums = null;
    [SerializeField] private Sprite _noteSpritePiano = null;

    [SerializeField] private float _pianoNoteRangeMultiplier = 0.8f;

    public Track CreateTrack(TrackData trackData)
    {
        var trackGO = Instantiate(trackData.Range <= 0 ? _drumTrackPrefab : _pianoTrackPrefab);

        var track = trackGO.GetComponent<Track>();

        track.TrackData = trackData;

        track.TrackAudio.clip = trackData.AudioClip;

        CreateNotes(track);

        track.UpdateSprites();

        return track;
    }

    public void CreateNotes(Track track)
    {
        GameObject notesGO;
        notesGO = Instantiate(_notePrefab);
        notesGO.transform.SetParent(track.transform);
        notesGO.transform.localPosition = Vector2.zero;

        var layerSprite = notesGO.GetComponent<LayeredSprite>();
        layerSprite.Layers = new LayeredSprite.Layer[track.TrackData.Notes.Length];

        for (int i = 0; i < track.TrackData.Notes.Length; i++)
        {
            var timeOffset = Mathf.Lerp(track.PlayAreaStart.localPosition.x, track.PlayAreaEnd.localPosition.x, track.TrackData.Notes[i].time * track.TrackData.LengthMultiplier / _barCount);
            if (float.IsNaN(timeOffset)) timeOffset = 0;
            float heightOffset;
            if (track.TrackData.Range <= 0)
                heightOffset = 0;
            else
                heightOffset = (Mathf.InverseLerp(0, track.TrackData.Range, track.TrackData.Notes[i].value) * 2 - 1) * _pianoNoteRangeMultiplier;
            Vector2 offset = new Vector2(timeOffset, heightOffset);

            layerSprite.Layers[i] = new LayeredSprite.Layer() { Sprite = track.TrackData.Range <= 0 ? _noteSpriteDrums : _noteSpritePiano, Offset = offset, Size = new Vector2(track.TrackData.Notes[i].length * track.TrackData.LengthMultiplier * 2, 1) };
        }
        layerSprite.RebuildLayers();

        track.NoteSprites = layerSprite;
        track.UpdateNotes();
        track.UpdateBars();
    }
}
