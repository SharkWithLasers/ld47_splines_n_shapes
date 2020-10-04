using KammBase;
using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private PlayerRenderer playerRenderer;

    [SerializeField] private BSplineDrawer bSplineDrawer;

    [SerializeField] private float tMoveSpeed;

    [SerializeField] private GameEvent playerLoopedEvent;

    [SerializeField] private GameEvent enoughLoopItersPassedEvent;


    private float curT;

    private bool isMoving;
    private float triangleT;
    private int numLoopItersSinceTri;
    private bool currentlyInTLoop;

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

        var prevT = curT;
        curT = (prevT + tMoveSpeed * Time.deltaTime) % 1;
        
        if (currentlyInTLoop)
        {
            CheckTriLoop(prevT, curT);
        }

        /*
        // we looped!
        if (prevT > curT)
        {
            playerLoopedEvent.Raise();
        }*/


        playerRenderer.SetColorForT(curT);
        transform.position = bSplineDrawer.GetPointAtT(curT);
    }

    private void CheckTriLoop(float prevT, float curT)
    {
        if ((prevT < curT && prevT <= triangleT && curT > triangleT)
            || (prevT > curT && prevT <= triangleT)
            || (prevT > curT && triangleT < curT)
            )
        {
            numLoopItersSinceTri++;
        }

        var inSecondHalfOfTriLoop =
            (triangleT >= 0.5f && curT < triangleT && ((triangleT + 0.5f) % 1) < curT)
            || (triangleT < 0.5f && (triangleT + 0.5f) < curT);
        // 1.5 iters has passed
        if (numLoopItersSinceTri >= 1 && inSecondHalfOfTriLoop)
        {
            currentlyInTLoop = false;
            // should set triangleT and numLoopIters to none, or use struct
            enoughLoopItersPassedEvent.Raise();
        }
    }

    public void OnBSplineDrawerComplete()
    {
        isMoving = true;
    }

    public void OnTargetCollided(int targetIdx)
    {
        if (targetIdx == 0)
        {
            // start dat loop! (a little bit before to handle Update)
            triangleT = MathUtil.mod(curT - .1f, 1);
            numLoopItersSinceTri = 0;
            currentlyInTLoop = true;

            // in a valid loop
        }
    }
}
