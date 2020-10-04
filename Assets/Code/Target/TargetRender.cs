using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRender : MonoBehaviour
{
    [SerializeField] private RegularPolygon polygonShape;

    [SerializeField] private Disc innerCircle;


    [SerializeField] private Color defaultDiscColor;

    [SerializeField] private Color correctOrderDiscColor;

    [SerializeField] private Color wrongOrderDiscColor;

    //[SerializeField] private 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetShapeForIndex(int idx)
    {
        polygonShape.Sides = idx + 3;
    }


    public void SetDefaultDiscColor()
    {
        innerCircle.Color = defaultDiscColor;
    }

    public void SetCorrectOrderDiscColor()
    {
        innerCircle.Color = correctOrderDiscColor;
    }

    public void SetWrongOrderDiscColor()
    {
        innerCircle.Color = wrongOrderDiscColor;
    }
}
