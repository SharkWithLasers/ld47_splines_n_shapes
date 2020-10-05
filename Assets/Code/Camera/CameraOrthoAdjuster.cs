using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraOrthoAdjuster : MonoBehaviour
{
    [SerializeField] private float referenceAspectRatio = 16f / 9f;

    [SerializeField] private float referenceOrthoSize;

    // Start is called before the first frame update
    private void Awake()
    {
        var camera = GetComponent<Camera>();

        var refHeight = 1;
        var refWidth = referenceAspectRatio;

        var currentHeight = 1;
        var currentWidth = camera.aspect;

        if (currentWidth < refWidth)
        {
            // zoom out until currentWidth == refWidth
            var newWidth = refWidth;
            var newHeight = (currentHeight * newWidth) / currentWidth;

            // orthosize is half-height.

            camera.orthographicSize = (newHeight / refHeight) * referenceOrthoSize;
        }
    }
}
