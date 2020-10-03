using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSplinePointRenderer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color c)
    {
        _spriteRenderer.color = c;
    }
}
