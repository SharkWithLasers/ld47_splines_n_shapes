using KammBase;
using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSplinePointGenerator : MonoBehaviour
{
    [SerializeField] private int sampleNumControlPoints = 3;
    [SerializeField] private float initCircleSize = 2;

    [SerializeField] private BSplinePoint bSplinePointPrefab;

    [SerializeField] private GameEvent noGenerationNecessaryEvent;


    [SerializeField] private GameEvent bSplinePointsGeneratedEvent;

    [SerializeField] private ColorPalette colorPalette;

    // yeah its mutable i know
    public List<BSplinePoint> bSplinePoints { get; private set; }

    private void Awake()
    {
        bSplinePoints = new List<BSplinePoint>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // returns false if no update necessary
    public void UpdateControlPointCount(int totalNumControlPoints)
    {
        if (totalNumControlPoints < 3)
        {
            throw new Exception("need three brah");
        }

        if (totalNumControlPoints == bSplinePoints.Count)
        {
            noGenerationNecessaryEvent.Raise();
            return;
        }

        if (bSplinePoints == null)
        {
            bSplinePoints = new List<BSplinePoint>();
        }

        if (bSplinePoints.Count == 0)
        {
            StartCoroutine(GenerateNewPointSet(totalNumControlPoints));
        }
        // TODO addition and subtraction stuff
        else if (totalNumControlPoints < bSplinePoints.Count)
        {
            StartCoroutine(RemoveControlPointsFlow(totalNumControlPoints));

            //AddControlPointsFlow(totalNumControlPoints);

            //AdjustCurrentPointSet(totalNumControlPoints);
        }

        else if (totalNumControlPoints > bSplinePoints.Count)
        {
            StartCoroutine(AddControlPointsFlow(totalNumControlPoints));

        }
    }

    private IEnumerator RemoveControlPointsFlow(int totalNumControlPoints)
    {
        for (var i = 0; i < totalNumControlPoints; i++)
        {
            var bspGO = bSplinePoints[i];

            var newColor = colorPalette.GetColorAtT(
                ((float)i) / totalNumControlPoints);

            bspGO.UpdateColorTo(newColor);
        }

        yield return new WaitForSeconds(0.2f);

        for (var i = bSplinePoints.Count-1; i >= totalNumControlPoints; i--)
        {
            var bspGO = bSplinePoints[i];

            bspGO.BeginDeathSequence();

            yield return new WaitForSeconds(1f);

            bSplinePoints.RemoveAt(bSplinePoints.Count - 1);
        }

        bSplinePointsGeneratedEvent.Raise();
    }

    private IEnumerator AddControlPointsFlow(int totalNumControlPoints)
    {
        // adjust color
        for (var i = 0; i < bSplinePoints.Count; i++)
        {
            var bspGO = bSplinePoints[i];

            var newColor = colorPalette.GetColorAtT(
                ((float) i) / totalNumControlPoints);

            bspGO.UpdateColorTo(newColor);
        }

        yield return new WaitForSeconds(0.2f);

        var firstEl = bSplinePoints[0];
        var lastEl = bSplinePoints[bSplinePoints.Count - 1];

        var lowerLeft = new Vector2(
            Mathf.Min(firstEl.transform.position.x, lastEl.transform.position.x),
            Mathf.Min(firstEl.transform.position.y, lastEl.transform.position.y));

        var topRight = new Vector2(
            Mathf.Max(firstEl.transform.position.x, lastEl.transform.position.x),
            Mathf.Max(firstEl.transform.position.y, lastEl.transform.position.y));

        var quartDist = (topRight - lowerLeft) * .25f;

        lowerLeft += quartDist;

        topRight -= quartDist;

        while (bSplinePoints.Count < totalNumControlPoints)
        {
            var curI = bSplinePoints.Count;

            var bspGO = Instantiate(bSplinePointPrefab);

            var color = colorPalette.GetColorAtT(
                ((float)curI) / totalNumControlPoints);


            bspGO.transform.position = new Vector3(
                UnityEngine.Random.Range(lowerLeft.x, topRight.x),
                UnityEngine.Random.Range(lowerLeft.y, topRight.y),
                0);

            bspGO.Construct(curI, color);

            bSplinePoints.Add(bspGO);

            yield return new WaitForSeconds(1f);

        }

        bSplinePointsGeneratedEvent.Raise();
    }

    private IEnumerator GenerateNewPointSet(int totalNumControlPoints)
    {
        if (totalNumControlPoints < 3)
        {
            throw new Exception("need at least 3");
        }

        for (var i = 0; i < totalNumControlPoints; i++)
        {
            var bspGO = Instantiate(bSplinePointPrefab);

            var color = colorPalette.GetColorAtT(
                ((float)i) / totalNumControlPoints);

            var t = (((float)i) / totalNumControlPoints) * Mathf.PI * 2;
            bspGO.transform.position = new Vector2(
                initCircleSize * Mathf.Cos(t),
                initCircleSize * Mathf.Sin(t));

            bspGO.Construct(i, color);

            bSplinePoints.Add(bspGO);

            yield return new WaitForSeconds(1f);
        }

        //yield return new WaitForSeconds(.5f);
        bSplinePointsGeneratedEvent.Raise();
    }
}
