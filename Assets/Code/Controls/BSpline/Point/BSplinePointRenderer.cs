using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiscColorStrat
{
    OuterOnly,
    InnerOnly,
    Both
}

public class BSplinePointRenderer : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private DiscColorStrat discColorStrat;

    [SerializeField] private Disc discOuter;
    [SerializeField] private Disc discInner;



    // Start is called before the first frame update
    void Awake()
    {
        //_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color c)
    {
        if (discColorStrat != DiscColorStrat.InnerOnly)
        {
            discOuter.Color = c;
        }

        if (discColorStrat != DiscColorStrat.OuterOnly)
        {
            discInner.Color = c;
        }
    }
}
