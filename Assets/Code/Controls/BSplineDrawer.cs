using Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSplineDrawer : MonoBehaviour
{
    // doesn't include last part of curve
    [SerializeField] private int numSamplePointsPerCurve = 10;

    [SerializeField] private List<Transform> bPointsLoop;

    [SerializeField] private Polyline polyLine;
    private int numTotalPoints;

    [SerializeField] private List<Vector2> sPoints;
    [SerializeField] private List<Vector2> pPoints;



    // Start is called before the first frame update
    void Start()
    {
        polyLine.SetPoints(new List<Vector3>());

        numTotalPoints = (numSamplePointsPerCurve * bPointsLoop.Count);
        //numTotalPoints = (numSamplePointsPerCurve * bPointsLoop.Count) + 1;

        for (var i = 0; i < numTotalPoints; i++)
        {
            polyLine.AddPoint(new Vector3(0, 0));
            polyLine.SetPointColor(i, new Color(1, 1, 1, 1 - ((float)i) / numTotalPoints));
        }

        sPoints = new List<Vector2>();
        pPoints = new List<Vector2>();
        foreach (var _ in bPointsLoop)
        {
            sPoints.Add(Vector2.zero);
            pPoints.Add(Vector2.zero);
            pPoints.Add(Vector2.zero);
        }

        GeneratePPoints();
        GenerateSPoints();

        GeneratePolyline();
    }

    private void GeneratePolyline()
    {

        for (var i = 0; i < bPointsLoop.Count; i++)
        {
            for (var s = 0; s < numSamplePointsPerCurve; s++)
            {
                var t = ((float)s) / numSamplePointsPerCurve;

                var pInitial = sPoints[i];
                var pFinal = i == bPointsLoop.Count - 1 
                    ? sPoints[0]
                    : sPoints[i + 1];

                var pControl1 = pPoints[2 * i];
                var pControl2 = pPoints[2 * i + 1];

                var bT = Mathf.Pow(1 - t, 3) * pInitial
                    + 3 * Mathf.Pow(1 - t, 2) * t * pControl1
                    + 3 * (1 - t) * t * t * pControl2
                    + t * t * t * pFinal;

                polyLine.SetPointPosition(numSamplePointsPerCurve * i + s, bT);
            }

        }
    }

    private void GenerateSPoints()
    {
        for (var i = 0; i < sPoints.Count; i++)
        {
            var p0 = i == 0 ? pPoints.Count - 1 : 2 * i - 1;
            var p1 = 2 * i;

            sPoints[i] = Vector2.Lerp(pPoints[p0], pPoints[p1], 0.5f);


            /*
            var bCur = bPointsLoop[i].position;
            var bNext = i == bPointsLoop.Count - 1
                ? bPointsLoop[0].position
                : bPointsLoop[i + 1].position;

            var p1 = Vector2.Lerp(bCur, bNext, 1f / 3);
            var p2 = Vector2.Lerp(bCur, bNext, 2f / 3);

            pPoints.Add(p1);
            pPoints.Add(p2);*/
        }
    }

    private void GeneratePPoints()
    {
        for (var i = 0; i < bPointsLoop.Count; i++)
        {
            var bCur = bPointsLoop[i].position;
            var bNext = i == bPointsLoop.Count - 1
                ? bPointsLoop[0].position
                : bPointsLoop[i + 1].position;

            pPoints[2 * i] = Vector2.Lerp(bCur, bNext, 1f / 3);
            pPoints[2 * i + 1] = Vector2.Lerp(bCur, bNext, 2f / 3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
