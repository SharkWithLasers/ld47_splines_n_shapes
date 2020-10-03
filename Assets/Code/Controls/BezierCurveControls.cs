using Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveControls : MonoBehaviour
{
    [SerializeField] private int numSamplePoints = 50;

    [SerializeField] private float ellipseAMovespeed = 2.5f;

    [SerializeField] private float cpMoveSpeed = 5f;

    [SerializeField] private float minEllipseWidth = 0.25f;


    [SerializeField] private GameObject controlPoint1;

    [SerializeField] private GameObject controlPoint2;

    [SerializeField] private Polyline polyLine;

    public enum CurveType
    {
        Ellipse,
        Cubic
    }

    [SerializeField] private CurveType curveType = CurveType.Cubic;
    private Vector3 pInitial;
    private Vector3 pFinal;
    private Vector3 pEllipseCenter;
    private Vector3 ellipseOriginOffset;
    private float ellipseB;
    private Vector3 ellipseDirY;
    private Vector3 ellipseDirX;
    private float ellipseA = 1f;



    // Start is called before the first frame update
    void Awake()
    {
        polyLine.SetPoints(new List<Vector3>());

        for (var i = 0; i < numSamplePoints; i++)
        {
            polyLine.AddPoint(new Vector3(0, 0));
            polyLine.SetPointColor(i, new Color(1, 1, 1, 1 - ((float) i) / (numSamplePoints - 1)));
        }
    }

    public (Vector2, Vector2) GetCenterAndHalfSizeForCamera()
    {
        if (curveType == CurveType.Cubic)
        {
            var center = (controlPoint1.transform.position + controlPoint2.transform.position + transform.position)
                / 3;

            var halfSizeX = Mathf.Max(
                Mathf.Abs(center.x - transform.position.x),
                Mathf.Abs(center.x - controlPoint1.transform.position.x),
                Mathf.Abs(center.x - controlPoint2.transform.position.x));

            var halfSizeY = Mathf.Max(
                Mathf.Abs(center.y - transform.position.y),
                Mathf.Abs(center.y - controlPoint1.transform.position.y),
                Mathf.Abs(center.y - controlPoint2.transform.position.y));

            return (center, new Vector2(halfSizeX, halfSizeY));
        }
        else
        {
            var p1 = GetEllipsePointAtT(0);
            var p2 = GetEllipsePointAtT(Mathf.PI);

            var halfSizeX = Mathf.Max(
                Mathf.Abs(pEllipseCenter.x - pInitial.x),
                Mathf.Abs(pEllipseCenter.x - p1.x),
                Mathf.Abs(pEllipseCenter.x - p2.x),
                Mathf.Abs(pEllipseCenter.x - pFinal.x));

            var halfSizeY = Mathf.Max(
                Mathf.Abs(pEllipseCenter.y - pInitial.y),
                Mathf.Abs(pEllipseCenter.y - p1.y),
                Mathf.Abs(pEllipseCenter.y - p2.y),
                Mathf.Abs(pEllipseCenter.y - pFinal.y));

            return (pEllipseCenter, new Vector3(halfSizeX, halfSizeY));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curveType == CurveType.Cubic)
        {
            CubicPointFlow();

            CubicVizFlow();
        }
        else
        {
            EllipsePointFlow();

            EllipseVizFlow();
            //QuadraticPointFlow();

            //QuadraticVizFlow();
        }
        //var blah = Input.GetAx
    }

    private void EllipseVizFlow()
    {
        for (var i = 0; i < numSamplePoints; i++)
        {
            var t = ((float)i) / (numSamplePoints-1) * Mathf.PI * 2 - Mathf.PI / 2;
            var p = GetEllipsePointAtT(t);
            polyLine.SetPointPosition(i, p);
        }
    }

    private Vector3 GetEllipsePointAtT(float t)
    {
        var unTranslatedOrRotatedP = new Vector3(
                        ellipseA * Mathf.Cos(t),
                        ellipseB * Mathf.Sin(t),
                        0);

        var rotatedP = new Vector3(
            ellipseDirX.x * unTranslatedOrRotatedP.x + ellipseDirY.x * unTranslatedOrRotatedP.y,
            ellipseDirX.y * unTranslatedOrRotatedP.x + ellipseDirY.y * unTranslatedOrRotatedP.y,
            0);

        var p = rotatedP + ellipseOriginOffset;
        return p;
    }

    private void EllipsePointFlow()
    {
        pInitial = transform.position;

        var cp1Vec = new Vector3(
            Input.GetAxisRaw("Horizontal_Arrows"),
            Input.GetAxisRaw("Vertical_Arrows"),
            0).normalized;

        ellipseA = Mathf.Max(
            minEllipseWidth,
            ellipseA + Input.GetAxisRaw("Vertical_WASD") * ellipseAMovespeed * Time.deltaTime);

        controlPoint1.transform.position += cp1Vec * cpMoveSpeed * Time.deltaTime;

        pFinal = controlPoint1.transform.position;

        // origin for now
        pEllipseCenter = (pFinal - pInitial) / 2;

        //pEllipseCenter = Vector3.zero;

        ellipseOriginOffset = pEllipseCenter - pInitial;

        ellipseB = (pFinal - pEllipseCenter).magnitude;

        ellipseDirY = (pFinal - pEllipseCenter).normalized;
        ellipseDirX = new Vector3(ellipseDirY.y, -ellipseDirY.x);

    }

    private void CubicVizFlow()
    {
        // because polyline is closed this should be ight
        for (var i = 0; i < numSamplePoints; i++)
        {
            var t = ((float)i) / (numSamplePoints - 1);

            var bT = Mathf.Pow(1 - t, 3) * pInitial
                + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1.transform.position
                + 3 * (1 - t) * t * t * controlPoint2.transform.position
                + t * t * t * pFinal;

            polyLine.SetPointPosition(i, bT);
        }
    }

    private void CubicPointFlow()
    {
        pInitial = transform.position;
        pFinal = transform.position;

        var cp1Vec = new Vector3(
            Input.GetAxisRaw("Horizontal_Arrows"),
            Input.GetAxisRaw("Vertical_Arrows"),
            0).normalized;

        controlPoint1.transform.position += cp1Vec * cpMoveSpeed * Time.deltaTime;

        var cp2Vec = new Vector3(
            Input.GetAxisRaw("Horizontal_WASD"),
            Input.GetAxisRaw("Vertical_WASD"),
            0).normalized;

        controlPoint2.transform.position += cp2Vec * cpMoveSpeed * Time.deltaTime;
    }
}
