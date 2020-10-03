using Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveControls : MonoBehaviour
{
    [SerializeField] private int numSamplePoints = 50;

    [SerializeField] private float playerMoveSpeed = 5f;

    [SerializeField] private float cpMoveSpeed = 5f;

    [SerializeField] private GameObject controlPoint1;

    [SerializeField] private GameObject controlPoint2;

    [SerializeField] private Polyline polyLine;

    public enum CurveType
    {
        Quadratic,
        Cubic
    }

    [SerializeField] private CurveType curveType = CurveType.Cubic;
    private Vector3 pInitial;
    private Vector3 pFinal;



    // Start is called before the first frame update
    void Awake()
    {
        polyLine.SetPoints(new List<Vector3>());

        for (var i = 0; i < numSamplePoints; i++)
        {
            polyLine.AddPoint(new Vector3(0, 0));
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
            QuadraticPointFlow();

            QuadraticVizFlow();
        }
        //var blah = Input.GetAx
    }

    private void QuadraticVizFlow()
    {
        // because polyline is closed this should be ight
        for (var i = 0; i < numSamplePoints; i++)
        {
            var t = ((float)i) / numSamplePoints;

            var bT = Mathf.Pow(1 - t, 2) * pInitial
                + 2 * (1 - t) * t * controlPoint1.transform.position
                + t * t * pFinal;

            polyLine.SetPointPosition(i, bT);
        }
    }

    private void CubicVizFlow()
    {
        // because polyline is closed this should be ight
        for (var i = 0; i < numSamplePoints; i++)
        {
            var t = ((float)i) / numSamplePoints;

            var bT = Mathf.Pow(1 - t, 3) * pInitial
                + 3 * Mathf.Pow(1 - t, 2) * t * controlPoint1.transform.position
                + 3 * (1 - t) * t * t * controlPoint2.transform.position
                + t * t * t * pFinal;

            polyLine.SetPointPosition(i, bT);
        }
    }

    private void QuadraticPointFlow()
    {
        var moveVec = new Vector3(
            Input.GetAxisRaw("Horizontal_WASD"),
            Input.GetAxisRaw("Vertical_WASD"),
            0).normalized;

        transform.position += moveVec * playerMoveSpeed * Time.deltaTime;

        pInitial = transform.position;
        pFinal = transform.position;

        var cp1Vec = new Vector3(
            Input.GetAxisRaw("Horizontal_Arrows"),
            Input.GetAxisRaw("Vertical_Arrows"),
            0).normalized;

        controlPoint1.transform.position += cp1Vec * cpMoveSpeed * Time.deltaTime;
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
