using NaughtyAttributes;
using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    [ReorderableList]
    private List<LevelData> levels;

    [SerializeField]
    private Target targetPrefab;

    [SerializeField]
    private BoolGameEvent loopValidationEvent;

    [SerializeField]
    private TargetCollisionNotes targetCollisionNotes;

    [SerializeField]
    private SFXManager sfxManager;

    [SerializeField]
    private BSplinePointGenerator bsPointGen;

    [SerializeField]
    private GameEvent levelLoadCompletedEvent;

    [SerializeField]
    private GameEvent levelCompletedEvent;

    [SerializeField]
    private GameEvent shrinkPlayerAndSplineEvent;

    [SerializeField]
    private GameEvent incorrectHitEvent;

    private int curLevelIdx;

    private HashSet<int> targetsHitProperly = new HashSet<int>();
    private List<Target> curLevelTargets = new List<Target>();
    private int _curExpectedTargetIdx = 0;
    private int targetsHitProperlyCount;

    // Start is called before the first frame update
    void Start()
    {
        // DO some game loaded shit

        // Load Level 0
        curLevelIdx = 0;
        StartCoroutine(ChillThen(1f, BeginLoadLevel));
        
    }

    private IEnumerator ChillThen(float secs, Action a)
    {
        yield return new WaitForSeconds(secs);
        a.Invoke();
    }

    private void BeginLoadLevel()
    {
        if (curLevelIdx < 0 || curLevelIdx >= levels.Count)
        {
            throw new Exception("da fuck u fuck why u fuckin do dat");
        }

        var curLevel = levels[curLevelIdx];



        // PART 1 control points 
        bsPointGen.UpdateControlPointCount(curLevel.NumControlPoints);

        // no need to update spline...so we can just reveal it

    }

    // ugh I can't imagine someone else looking at this would be able to step through
    // this weird flow
    public void OnSplineThickened()
    {
        StartCoroutine(CreateTargets());
    }

    private IEnumerator CreateTargets()
    {
        var curLevel = levels[curLevelIdx];

        curLevelTargets = new List<Target>();
        targetsHitProperly = new HashSet<int>();
        targetsHitProperlyCount = 0;

        // PART 3 ... now we gotta create the targets
        yield return new WaitForSeconds(.25f);


        for (var i = 0; i < curLevel.TargetPositions.Count; i++)
        {
            var targetPos = curLevel.TargetPositions[i];
            // construct hash set of targets that got hit?
            var targetGO = Instantiate(targetPrefab, targetPos, Quaternion.identity);

            targetGO.Construct(i, curLevel.TargetPositions.Count, curLevel);

            yield return new WaitForSeconds(1f);

            curLevelTargets.Add(targetGO);
        }

        _curExpectedTargetIdx = 0;
        levelLoadCompletedEvent.Raise();
        // level load completed!!!
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnoughLoopsPassed()
    {
        // gotta deal with next level
        _curExpectedTargetIdx = 0 % curLevelTargets.Count;

        targetsHitProperlyCount = 0;

        // set all the others back to default?
        for (var i = 0; i < curLevelTargets.Count; i++)
        {
            curLevelTargets[i].OnDefaultState();
        }

        // could listen itself lol
        sfxManager.PlayEnoughLoopsPassed();
    }

    public void OnTargetCollided(int targetIdx)
    {
        var target = curLevelTargets[targetIdx];

        if (targetIdx == _curExpectedTargetIdx)
        {
            targetsHitProperlyCount++;

            targetsHitProperly.Add(targetIdx);



            // we did da loop de loop! next level bah-bee
            if (targetsHitProperlyCount == curLevelTargets.Count + 1)
            {
                target.OnCorrectlyHit(playAudio: false);


                var semiToneDiff = targetCollisionNotes.GetSemiDiffForLoopValid(curLevelTargets.Count);

                sfxManager.PlayLoopValidAt(semiToneDiff);

                StartCoroutine(BeginLevelCompleteFlow());
            }
            else
            {
                target.OnCorrectlyHit();

                if (targetIdx == 0)
                {
                    // set all the others back to default
                    for (var i = 1; i < curLevelTargets.Count; i++)
                    {
                        curLevelTargets[i].OnDefaultState();
                    }
                }

                _curExpectedTargetIdx = (_curExpectedTargetIdx + 1) % curLevelTargets.Count;
            }

        }
        else
        {
            // start a new loop on triangle
            if (targetIdx == 0)
            {
                target.OnCorrectlyHit();

                _curExpectedTargetIdx = 1 % curLevelTargets.Count;

                targetsHitProperlyCount = 1;

                // set all the others back to default?
                for (var i = 1; i < curLevelTargets.Count; i++)
                {
                    curLevelTargets[i].OnDefaultState();
                }
            }
            // mark invalid...? that's it?
            else
            {
                incorrectHitEvent.Raise();

                target.OnIncorrectlyHit();
            }
        }
    }

    private IEnumerator BeginLevelCompleteFlow()
    {
        for (var i = curLevelTargets.Count - 1; i >= 0; i--)
        {
            // turn off colliders
            curLevelTargets[i].OnLevelCompleted();
        }


        levelCompletedEvent.Raise();

        yield return new WaitForSeconds(0.75f);

        for (var i = curLevelTargets.Count - 1; i >= 0; i--)
        {
            curLevelTargets[i].BeginDeathSentence();
        }

        // modulo tee hee funny joke
        var nextLevelIdx = (curLevelIdx + 1) % levels.Count;

        var curLevel = levels[curLevelIdx];
        var nextLevel = levels[nextLevelIdx];

        yield return new WaitForSeconds(1.5f);

        // sooo clunky
        if (curLevel.NumControlPoints != nextLevel.NumControlPoints)
        {
            shrinkPlayerAndSplineEvent.Raise();
            /*yield return new WaitForSeconds(
                1 * Mathf.Abs(curLevel.NumControlPoints - nextLevel.NumControlPoints));
                */
        }

        curLevelIdx = nextLevelIdx;

        BeginLoadLevel();

        //yield return new WaitForSeconds(1f);

        //curLevelIdx = nextLevelIdx;
    }
}
