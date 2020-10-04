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
            //AddControlPointsFlow(totalNumControlPoints);

            AdjustCurrentPointSet(totalNumControlPoints);
        }

        else if (totalNumControlPoints > bSplinePoints.Count)
        {
            //RemoveControlPointsFlow(totalNumControlPoints);
        }
    }

    private void RemoveControlPointsFlow(int totalNumControlPoints)
    {
        throw new NotImplementedException();
    }

    private void AddControlPointsFlow(int totalNumControlPoints)
    {
        throw new NotImplementedException();
    }

    private void AdjustCurrentPointSet(int totalNumControlPoints)
    {
        if (totalNumControlPoints < 3)
        {
            throw new Exception("need at least 3");
        }

        while (bSplinePoints.Count < totalNumControlPoints)
        {
            // todo instantiates and destroys should really be coroutined
            var bspGO = Instantiate(bSplinePointPrefab);

            bspGO.transform.position = (bSplinePoints[0].transform.position
                + bSplinePoints[bSplinePoints.Count - 1].transform.position) / 2;

            bSplinePoints.Add(bspGO);
        }

        while (bSplinePoints.Count > totalNumControlPoints)
        {
            var bspLast = bSplinePoints.Last();
            Destroy(bspLast);

            bSplinePoints.RemoveAt(bSplinePoints.Count - 1);
        }

        for (var i = 0; i < totalNumControlPoints; i++)
        {
            var bspGO = bSplinePoints[i];

            var color = colorPalette.GetColorAtT(
                ((float)i) / totalNumControlPoints);
            bspGO.Construct(i, color);

            var t = (((float)i) / totalNumControlPoints) * Mathf.PI * 2;
            bspGO.transform.position = new Vector2(
                initCircleSize * Mathf.Cos(t),
                initCircleSize * Mathf.Sin(t));
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

            yield return new WaitForSeconds(1f);

            bSplinePoints.Add(bspGO);
        }

        //yield return new WaitForSeconds(.5f);
        bSplinePointsGeneratedEvent.Raise();
    }
}
