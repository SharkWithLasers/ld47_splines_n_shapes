using DG.Tweening;
using KammBase;
using NaughtyAttributes;
using Shapes;
using System;
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
    [SerializeField] private Disc discInner;


    [SerializeField] private float playerHitTargetScaleAmt;

    [SerializeField] private float playerHitTargetScaleDur;

    private bool currentlyScaling;

    [SerializeField] private float mouseOverTweenTime = .2f;

    [SerializeField] private float initialDiscRadius = .115f;

    [SerializeField] private float mouseOverRadius = .13f;

    [SerializeField] private float mouseDownRadius = .15f;

    [SerializeField] private Disc backgroundAuraDisc;

    [SerializeField] private float initialBGAuraRadius = .05f;

    [Range(0f, 1f)]
    [SerializeField] private float initialBGAuraAlpha = .5f;

    [SerializeField] private float mouseOutAuraRadius = .3f;

    [SerializeField] private float mouseOutRadiusAuraSecs = 2f;

    [SerializeField] private float mouseOutColorAuraSecs = .5f;

    [SerializeField] private float mouseOutPauseInterval = 1f;

    [SerializeField] private int mouseOutNumLoops = -1;

    [SerializeField] private float mouseInAuraRadius = .5f;

    [SerializeField] private float mouseInRadiusAuraSecs = 1f;

    [SerializeField] private float mouseInColorAuraSecs = .25f;

    [SerializeField] private float mouseInPauseInterval = .33f;

    [SerializeField] private int mouseInNumLoops = -1;


    [SerializeField] private float mouseDownAuraRadius = .75f;

    [SerializeField] private float mouseDownRadiusAuraSecs = 1.5f;

    [SerializeField] private float mouseDownColorAuraSecs = .5f;

    [SerializeField] private float mouseDownPauseInterval = 1f;

    [SerializeField] private int mouseDownNumLoops = 1;

    private Option<Tween> discRadiusTween = Option<Tween>.None;

    private Option<Sequence> radiusSeq = Option<Sequence>.None;

    private Color _initialBackgroundDiscColor;
    private Color _finalBackgroundDiscColor;

    private void Awake()
    {
        discInner.Radius = initialDiscRadius;
    }

    public void SetColor(Color c)
    {
        discInner.Color = c;

        _initialBackgroundDiscColor = new Color(c.r, c.g, c.b, initialBGAuraAlpha);

        _finalBackgroundDiscColor = new Color(c.r, c.g, c.b, 0);

        backgroundAuraDisc.Radius = initialBGAuraRadius;
        backgroundAuraDisc.Color = _initialBackgroundDiscColor;

    }

    public void UpdateColorTo(Color newColor)
    {
        var curColor = discInner.Color;

        DOTween.To(
            () => curColor,
            x => {
                curColor = x;
                SetColor(x);
            },
            newColor,
            .2f);
    }

    private void SetMouseOutAura()
    {
        // when I'm not so tired, make these SOs
        SetBackgroundAuraLoop(
            mouseOutAuraRadius,
            mouseOutRadiusAuraSecs,
            mouseOutColorAuraSecs,
            mouseOutPauseInterval,
            mouseOutNumLoops,
            resetRadius: true);
    }

    private void SetMouseInAura()
    {
        SetBackgroundAuraLoop(
            mouseInAuraRadius,
            mouseInRadiusAuraSecs,
            mouseInColorAuraSecs,
            mouseInPauseInterval,
            mouseInNumLoops,
            resetRadius: true);
    }

    private void SetMouseDownAura()
    {
        SetBackgroundAuraLoop(
            mouseDownAuraRadius,
            mouseDownRadiusAuraSecs,
            mouseDownColorAuraSecs,
            mouseDownPauseInterval,
            mouseDownNumLoops,
            resetRadius: true);
    }

    private void SetBackgroundAuraLoop(
        float radiusEnd, float radiusTweenTime, float finalColorTime, float pauseInterval,
        int numLoops, bool resetRadius)
    {
        if (radiusSeq.HasValue)
        {
            radiusSeq.Value.Kill();
        }

        radiusSeq = DOTween.Sequence();
        
        // shit don't work right lol
        // because its looping, initial starting point is ass cheeks
        if (resetRadius || backgroundAuraDisc.Radius >= radiusEnd)
        {
            backgroundAuraDisc.Radius = initialBGAuraRadius;
        }

        backgroundAuraDisc.Color = _initialBackgroundDiscColor;
        var radiusTween = DOTween.To(
            () => backgroundAuraDisc.Radius,
            x => backgroundAuraDisc.Radius = x,
            radiusEnd,
            radiusTweenTime).SetEase(Ease.InSine);

        var colorTween = DOTween.To(
            () => backgroundAuraDisc.Color,
            x => backgroundAuraDisc.Color = x,
            _finalBackgroundDiscColor,
            finalColorTime).SetEase(Ease.OutSine);

        radiusSeq.Value
            .Append(radiusTween)
            .Append(colorTween)
            .AppendInterval(pauseInterval)
            .SetLoops(numLoops, LoopType.Restart);
    }

    private IEnumerator ChillThen(float secs, Action a)
    {
        yield return new WaitForSeconds(secs);
        a.Invoke();
    }

    public void OnPlayerHitTarget()
    {
        if (currentlyScaling)
        {
            return;
        }

        currentlyScaling = true;


        discInner.transform.localScale = Vector3.one;
        discInner.transform
            .DOScale(playerHitTargetScaleAmt, playerHitTargetScaleDur)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => currentlyScaling = false);
    }

    public void MouseEnterHappened()
    {
        SetDiscRadiusTween(mouseOverRadius, mouseOverTweenTime);

        SetMouseInAura();

    }

    public void MouseExitHappened()
    {
        SetDiscRadiusTween(initialDiscRadius, mouseOverTweenTime);

        SetMouseOutAura();
    }

    public void MouseDownHappened()
    {
        SetDiscRadiusTween(mouseDownRadius, mouseOverTweenTime);

        SetMouseDownAura();
    }

    public void MouseUpHappened(bool withinCollider)
    {
        if (withinCollider)
        {
            SetDiscRadiusTween(
                mouseOverRadius,
                mouseOverTweenTime);

            SetMouseInAura();
        }
        else
        {
            SetDiscRadiusTween(
                initialDiscRadius,
                mouseOverTweenTime);

            SetMouseOutAura();
        }
    }

    private void SetDiscRadiusTween(float toThis, float tweenTime, Ease ease = Ease.InSine)
    {
        if (discRadiusTween.HasValue)
        {
            discRadiusTween.Value.Kill();
        }

        discRadiusTween = DOTween.To(
            () => discInner.Radius,
            x => discInner.Radius = x,
            toThis,
            tweenTime).SetEase(ease);
    }

    public void OnConstructed()
    {
        transform.localScale = Vector3.one * .1f;

        transform
            .DOScale(1, 0.75f);
    }
}
