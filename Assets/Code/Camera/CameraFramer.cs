using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFramer : MonoBehaviour
{
    [SerializeField] private BezierCurveControls bccTarget;

    [SerializeField] private float halfSizeExtension = 1;

    [SerializeField] private float smoothTimeCenter = 0.2f;

    [SerializeField] private float smoothTimeOrthoSize = 0.2f;

    [SerializeField] private float minOrthoSize = 3f;

    [SerializeField] private float maxOrthoSize = 10f;

    private float _orthoVelocity = 0;
    private Vector2 _centerVelocity = Vector2.zero;
    private Camera _camera;
    private float initialZ;

    // Start is called before the first frame update
    void Awake()
    {
        _camera = GetComponent<Camera>();

        initialZ = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var (targetCenter, targetHalfSize) = bccTarget.GetCenterAndHalfSizeForCamera();

        var newXYPos = Vector2.SmoothDamp(
            transform.position,
            targetCenter,
            ref _centerVelocity,
            smoothTimeCenter);

        transform.position = new Vector3(newXYPos.x, newXYPos.y, transform.position.z);


        var orthoSize = Mathf.Max(targetHalfSize.y, targetHalfSize.x / _camera.aspect) * halfSizeExtension;

        _camera.orthographicSize = Mathf.Clamp(
            Mathf.SmoothDamp(
                _camera.orthographicSize,
                orthoSize,
                ref _orthoVelocity,
                smoothTimeOrthoSize),
            minOrthoSize,
            maxOrthoSize);
    }
}
