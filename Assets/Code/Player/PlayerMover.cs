using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private PlayerRenderer playerRenderer;

    [SerializeField] private BSplineDrawer bSplineDrawer;

    [SerializeField] private float tMoveSpeed;

    private float curT;

    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        playerRenderer.SetColorForT(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            return;
        }

        curT = (curT + tMoveSpeed * Time.deltaTime) % 1;

        playerRenderer.SetColorForT(curT);
        transform.position = bSplineDrawer.GetPointAtT(curT);
    }

    public void OnBSplineDrawerComplete()
    {
        isMoving = true;
    }
}
