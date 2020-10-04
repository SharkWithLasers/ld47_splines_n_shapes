using DG.Tweening;
using KammBase;
using Shapes;
using System;
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

    private Option<Tween> outlineDiscFatTween = Option<Tween>.None;
    private Option<Tween> innerDiscFatTween = Option<Tween>.None;


    // Start is called before the first frame update
    void Start()
    {
        // stupid way of hiding it initially
        outlineDisc.transform.localScale = Vector3.zero;
        innerDisc.transform.localScale = Vector3.zero;
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

    public void MakeThick(float secs)
    {
        if (outlineDiscFatTween.HasValue
            && !outlineDiscFatTween.Value.IsComplete())
        {
            outlineDiscFatTween.Value.Kill();
        }

        if (innerDiscFatTween.HasValue
            && !innerDiscFatTween.Value.IsComplete())
        {
            innerDiscFatTween.Value.Kill();
        }

        outlineDisc.transform.localScale = Vector3.zero;
        outlineDiscFatTween = outlineDisc.transform
            .DOScale(Vector3.one, secs)
            .SetEase(Ease.OutQuad);

        innerDisc.transform.localScale = Vector3.zero;
        innerDiscFatTween = innerDisc.transform
            .DOScale(Vector3.one, secs)
            .SetEase(Ease.OutQuad);
    }

    public void MakeSkinny(float secs)
    {
        if (outlineDiscFatTween.HasValue
            && !outlineDiscFatTween.Value.IsComplete())
        {
            outlineDiscFatTween.Value.Kill();
        }

        if (innerDiscFatTween.HasValue
            && !innerDiscFatTween.Value.IsComplete())
        {
            innerDiscFatTween.Value.Kill();
        }

        outlineDiscFatTween = outlineDisc.transform
            .DOScale(Vector3.zero, secs)
            .SetEase(Ease.OutQuad);

        innerDisc.transform.localScale = Vector3.zero;
        innerDiscFatTween = innerDisc.transform
            .DOScale(Vector3.zero, secs)
            .SetEase(Ease.OutQuad);
    }
}
