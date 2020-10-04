using DG.Tweening;
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

    [SerializeField] private float onTargetHitScale = 1.35f;
    [SerializeField] private float scaleTweenTime = .15f;
    private bool currentlyScaling;

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

    public void OnPlayerHitTarget()
    {
        if (currentlyScaling)
        {
            return;
        }

        currentlyScaling = true;


        outlineDisc.transform.localScale = Vector3.one;
        outlineDisc.transform
            .DOScale(onTargetHitScale, scaleTweenTime)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => currentlyScaling = false);

        innerDisc.transform.localScale = Vector3.one;
        innerDisc.transform
            .DOScale(onTargetHitScale, scaleTweenTime)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => currentlyScaling = false);
    }
}
