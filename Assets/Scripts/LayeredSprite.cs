using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredSprite : MonoBehaviour
{
    public Layer[] Layers;
    public bool _ignoreSize = true;
    SpriteRenderer[] _renderers;

    private void Awake()
    {
        RebuildLayers();
    }

    public void RebuildLayers()
    {
        DeleteLayers();
        _renderers = new SpriteRenderer[Layers.Length];
        for (int i = 0; i < Layers.Length; i++)
        {
            var go = new GameObject($"Layer {i}", typeof(SpriteRenderer));
            var rend = go.GetComponent<SpriteRenderer>();
            rend.sprite = Layers[i].Sprite;
            rend.color = Layers[i].Colour;
            go.transform.SetParent(transform);
            go.transform.localPosition = Layers[i].Offset;
            if (!_ignoreSize)
            {
                rend.drawMode = SpriteDrawMode.Tiled;
                rend.size = new Vector2(Layers[i].Size.x, rend.size.y);
            }
            _renderers[i] = rend;
        }
    }
    
    void DeleteLayers()
    {
        if (_renderers == null) return;
        for (int i = 0; i < _renderers.Length; i++)
        {
            Destroy(_renderers[i].gameObject);
        }
    }

    public void SetLayerColour(int index, Color colour)
    {
        var layer = Layers[index];
        layer.Colour = colour;
        Layers[index] = layer;
        _renderers[index].color = colour;
    }

    public void SetLayerSprite(int index, Sprite sprite)
    {
        _renderers[index].sprite = sprite;
    }

    [System.Serializable]
    public class Layer
    {
        public Sprite Sprite;
        public Color Colour = Color.white;
        public Vector2 Offset;
        public Vector2 Size = Vector2.one;
    }
}
