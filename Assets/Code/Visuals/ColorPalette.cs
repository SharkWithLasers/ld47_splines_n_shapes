using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Visuals/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color trailStart;
    public Color TrailStart => trailStart;

    [SerializeField] private Color trailEnd;
    public Color TrailEnd => trailEnd;

    [SerializeField] private List<Color> trailColorLoop;
    public List<Color> TrailColorLoop => trailColorLoop;

    public Color GetColorAtT(float t)
    {
        t = Mathf.Clamp(t, 0, 1);

        var colorIdxToStart = (int)(trailColorLoop.Count * t);
        var colorIdxToEnd = (colorIdxToStart + 1) % trailColorLoop.Count;


        var colorStart = trailColorLoop[colorIdxToStart];
        var colorEnd = trailColorLoop[colorIdxToEnd];

        var colorTStart = (colorIdxToStart + 0f) / trailColorLoop.Count;
        var colorTEnd = (colorIdxToStart + 1f) / trailColorLoop.Count;

        var localT = (t - colorTStart) / (colorTEnd - colorTStart);

        return Color.Lerp(colorStart, colorEnd, localT);
    }
}
