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

    [SerializeField] private float tRegMoveSpeed;

    [SerializeField] private float tLoopMoveSpeed;

    private float speedToUse;

    [SerializeField] private GameEvent enoughLoopItersPassedEvent;

    private float curT;

    private bool isMoving;
    private float triangleT;
    private int numLoopItersSinceTri;
    private bool currentlyInTLoop;

    // Start is called before the first frame update
    void Start()
    {
        speedToUse = tRegMoveSpeed;

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
        curT = (prevT + speedToUse * Time.deltaTime) % 1;
        
        if (currentlyInTLoop)
        {
            CheckTriLoop(prevT, curT);
        }


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

        var inLastNineTenthsOfTriLoop =
            (triangleT >= 0.9f && curT < triangleT && ((triangleT + 0.1f) % 1) < curT)
            || (triangleT < 0.9f && (triangleT + 0.1f) < curT);

        // 1.1 iters has passed
        if (numLoopItersSinceTri >= 1 && inLastNineTenthsOfTriLoop)
        {
            currentlyInTLoop = false;
            speedToUse = tRegMoveSpeed;
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
            triangleT = MathUtil.mod(curT - .05f, 1);
            numLoopItersSinceTri = 0;
            currentlyInTLoop = true;

            // tLoopSpeed!
            speedToUse = tLoopMoveSpeed;

            // in a valid loop
        }

        playerRenderer.OnPlayerHitTarget();
    }

    public void MakeThick(float secs)
    {
        playerRenderer.MakeThick(secs);
    }

    public void OnIncorrectTarget()
    {
        speedToUse = tRegMoveSpeed;
    }

    // OnLevelCompleted remove t-loop?

    public void OnLevelCompleted()
    {
        currentlyInTLoop = false;
        speedToUse = tRegMoveSpeed;
    }

    public void MakeSkinny(float secs)
    {
        isMoving = false;

        playerRenderer.MakeSkinny(secs);
    }
}
