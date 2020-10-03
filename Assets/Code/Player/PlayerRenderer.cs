using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float tOffsetAmount;

    [SerializeField] private bool setForOutlineOnly;

    [SerializeField] private Disc outlineDisc;
    [SerializeField] private Disc innerDisc;

    [SerializeField] private ColorPalette colorPalette;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetColorForT(float t)
    {
        var tToUse = (t + tOffsetAmount) % 1;
        if (setForOutlineOnly)
        {
            outlineDisc.Color = colorPalette.GetColorAtT(tToUse);
        }
        else
        {
            innerDisc.Color = colorPalette.GetColorAtT(tToUse);
        }
    }
}
