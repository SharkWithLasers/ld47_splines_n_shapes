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

    [SerializeField] private uint colliderTailInterpPoints;

    private float speedToUse;

    [SerializeField] private GameEvent enoughLoopItersPassedEvent;
    private float _prevT;
    private float curT;

    private bool isMoving;
    private float triangleT;
    private int numLoopItersSinceTri;
    private bool currentlyInTLoop;
    private List<(Vector2, float)> _tailPointsAndT;

    public int NumTailPoints => _tailPointsAndT == null ? 0 : _tailPointsAndT.Count;

    public float Radius { get; private set; }

    public float DebugPrevT => _prevT;
    public float DebugCurT => curT;

    private void Awake()
    {
        Radius = GetComponent<CircleCollider2D>().radius;
    }

    // Start is called before the first frame update
    void Start()
    {
        speedToUse = tRegMoveSpeed;

        playerRenderer.SetColorForT(0);

        _tailPointsAndT = new List<(Vector2, float)>();

        for (var i = 0; i < colliderTailInterpPoints + 1; i++)
        {
            _tailPointsAndT.Add((Vector2.zero, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            return;
        }

        _prevT = curT;
        curT = (_prevT + speedToUse * Time.deltaTime) % 1;
        
        if (currentlyInTLoop)
        {
            CheckTriLoop(_prevT, curT);
        }

        playerRenderer.SetColorForT(curT);
        transform.position = bSplineDrawer.GetPointAtT(curT);

        UpdateInterpTail();
    }

    private void UpdateInterpTail()
    {
        // not sure if this handles multiple revolutions in a single frame
        var tDiff = (curT >= _prevT)
            ? curT - _prevT
            : (1 - _prevT) + curT;

        //perhaps this should be an SO or something
        var interpAmtForT = tDiff / _tailPointsAndT.Count;

        for (var i=0; i < _tailPointsAndT.Count; i++)
        {
            var t = (_prevT + interpAmtForT * i) % 1;

            _tailPointsAndT[i] = (bSplineDrawer.GetPointAtTPrevFrame(t), t);
        }
    }

    public Option<Vector2> GetTailPointAt(int idx)
    {
        if (idx < 0 || _tailPointsAndT == null || idx >= _tailPointsAndT.Count)
        {
            return Option<Vector2>.None;
        }

        return _tailPointsAndT[idx].Item1;
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

    public void DebugLogAllTailPointsAndPoint()
    {
        Debug.Log($"cur pos: {transform.position.ToString("F4")}");
        if (_tailPointsAndT == null)
        {
            return;
        }

        for (var i = 0; i < _tailPointsAndT.Count; i++)
        {
            var (tp, t) = _tailPointsAndT[i];

            Debug.Log($"tail pos i:{i} => {tp.ToString("F4")}...t: {t}");
        }
    }

    public void MakeSkinny(float secs)
    {
        isMoving = false;

        playerRenderer.MakeSkinny(secs);
    }
}
