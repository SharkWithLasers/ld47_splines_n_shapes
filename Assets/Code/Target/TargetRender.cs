using DG.Tweening;
using KammBase;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRender : MonoBehaviour
{
    [SerializeField] private RegularPolygon polygonShape;

    [SerializeField] private RegularPolygon polygonAura;

    [SerializeField] private Disc innerCircle;

    [SerializeField] private Color defaultDiscColor;

    [SerializeField] private Color correctOrderDiscColor;

    [SerializeField] private Color wrongOrderDiscColor;

    [SerializeField] private float collisionRadiusPeak = 1.2f;

    [SerializeField] private float collisionTweenTime = .15f;

    [SerializeField] private float revertTweenTime = .2f;

    [SerializeField] private float polygonAuraRadiusEnd = .5f;

    [SerializeField] private float polygonAuraAlphaStart = .05f;

    [SerializeField] private float auraRadiusTime = .25f;

    [SerializeField] private float auraAlphaTime = .25f;

    private Option<Tween> innerColorTween = Option<Tween>.None;

    private Option<Sequence> auraSeq = Option<Sequence>.None;

    private Option<Sequence> shapeTweenSeq = Option<Sequence>.None;


    private Color _defaultPolygonColor;
    private float _initShapeRadius;
    private float _initAuraRadius;

    //[SerializeField] private 

    // Start is called before the first frame update
    void Awake()
    {
        polygonAura.gameObject.SetActive(false);

        _defaultPolygonColor = polygonShape.Color;
        _initShapeRadius = polygonShape.Radius;
        _initAuraRadius = polygonAura.Radius;
    }

    public void SetShapeForIndex(int idx)
    {
        polygonShape.Sides = idx + 3;
        polygonAura.Sides = idx + 3;
    }


    public void SetToDefault()
    {
        ShapeTween(false, defaultDiscColor, revertTweenTime);
        //innerCircle.Color = defaultDiscColor;
    }

    public void SetToCorrectOrder()
    {
        AuraTween(correctOrderDiscColor);
        ShapeTween(true, correctOrderDiscColor, collisionTweenTime);
        //innerCircle.Color = correctOrderDiscColor;
    }

    public void SetToWrongOrder()
    {
        AuraTween(wrongOrderDiscColor);
        ShapeTween(true, wrongOrderDiscColor, collisionTweenTime);
        //innerCircle.Color = wrongOrderDiscColor;
    }

    public void ShapeTween(bool shouldScale, Color peakColor, float tweenTime)
    {
        if (shapeTweenSeq.HasValue && !shapeTweenSeq.Value.IsComplete())
        {
            shapeTweenSeq.Value.Kill();
        }

        if (innerColorTween.HasValue && !innerColorTween.Value.IsComplete())
        {
            innerColorTween.Value.Kill();
        }

        shapeTweenSeq = DOTween.Sequence();

        polygonShape.Color = _defaultPolygonColor;
        innerCircle.Color = _defaultPolygonColor;

        innerColorTween = DOTween.To(
            () => innerCircle.Color,
            x => innerCircle.Color = x,
            peakColor,
            tweenTime).SetEase(Ease.OutQuad);

        var colorTween = DOTween.To(
            () => polygonShape.Color,
            x => polygonShape.Color = x,
            peakColor,
            tweenTime).SetEase(Ease.OutQuad);

        shapeTweenSeq.Value.Append(colorTween);

        if (shouldScale)
        {
            polygonShape.Radius = _initShapeRadius;

            var radiusTween = DOTween.To(
                    () => polygonShape.Radius,
                    x => polygonShape.Radius = x,
                    collisionRadiusPeak,
                    tweenTime)
                .SetEase(Ease.OutQuad);

            shapeTweenSeq.Value.Join(radiusTween);
        }

        shapeTweenSeq.Value
            .SetLoops(2, LoopType.Yoyo);
    }

    public void AuraTween(Color colorToUse)
    {
        if (auraSeq.HasValue)
        {
            auraSeq.Value.Kill();
        }

        auraSeq = DOTween.Sequence();

        var initColor = new Color(colorToUse.r, colorToUse.g, colorToUse.b, polygonAuraAlphaStart);
        var finalColor = new Color(colorToUse.r, colorToUse.g, colorToUse.b, 0);

        polygonAura.gameObject.SetActive(true);

        polygonAura.Radius = _initAuraRadius;
        polygonAura.Color = initColor;
        
        var radiusTween = DOTween.To(
            () => polygonAura.Radius,
            x => polygonAura.Radius = x,
            polygonAuraRadiusEnd,
            auraRadiusTime).SetEase(Ease.OutQuad);

        var colorTween = DOTween.To(
            () => polygonAura.Color,
            x => polygonAura.Color = x,
            finalColor,
            auraAlphaTime).SetEase(Ease.OutQuad);

        auraSeq.Value
            .Append(radiusTween)
            .Append(colorTween)
            .OnComplete(() =>
            {
                polygonAura.gameObject.SetActive(false);
                auraSeq = Option<Sequence>.None;
            });
    }
}
