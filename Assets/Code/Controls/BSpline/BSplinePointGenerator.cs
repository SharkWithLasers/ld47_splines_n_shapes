using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSplinePointGenerator : MonoBehaviour
{
    [SerializeField] private int numControlPoints = 5;
    [SerializeField] private float initCircleSize = 2;

    [SerializeField] private BSplinePoint bSplinePointPrefab;

    [SerializeField] private GameEvent bSplinePointsGeneratedEvent;

    [SerializeField] private ColorPalette colorPalette;

    // yeah its mutable i know
    public List<BSplinePoint> bSplinePoints { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        bSplinePoints = new List<BSplinePoint>();
        for (var i = 0; i < numControlPoints; i++)
        {
            var bspGO = Instantiate(bSplinePointPrefab);

            var color = colorPalette.GetColorAtT(
                ((float)i) / numControlPoints);
            bspGO.Construct(i, color);

            var t = (((float)i) / numControlPoints) * Mathf.PI * 2;
            bspGO.transform.position = new Vector2(
                initCircleSize * Mathf.Cos(t),
                initCircleSize * Mathf.Sin(t));

            bSplinePoints.Add(bspGO);
        }
        bSplinePointsGeneratedEvent.Raise();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
