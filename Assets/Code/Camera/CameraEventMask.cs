using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraEventMask : MonoBehaviour
{
    [SerializeField]
    private LayerMask cameraEventMask;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Camera>().eventMask = cameraEventMask;
    }
}
