using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredSprite : MonoBehaviour
{
    public Layer[] _layers;
    SpriteRenderer[] _renderers;

    private void Awake()
    {
        _renderers = new SpriteRenderer[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
        {
            var go = new GameObject($"Layer {i}", typeof(SpriteRenderer));
            var rend = go.GetComponent<SpriteRenderer>();
            rend.sprite = _layers[i].Sprite;
            rend.color = _layers[i].Colour;
            go.transform.SetParent(transform);
            go.transform.localPosition = _layers[i].Offset;
            _renderers[i] = rend;
        }
    }

    public void SetLayerColour(int index, Color colour)
    {
        _renderers[index].color = colour;
    }

    public void SetLayerSprite(int index, Sprite sprite)
    {
        _renderers[index].sprite = sprite;
    }

    [System.Serializable]
    public struct Layer
    {
        public Sprite Sprite;
        public Color Colour;
        public Vector2 Offset;
    }
}
