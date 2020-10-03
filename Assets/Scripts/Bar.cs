using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    [SerializeField] private LayeredSprite _layeredSprite = null;
    public LayeredSprite LayeredSprite { get => _layeredSprite; set => _layeredSprite = value; }
}
