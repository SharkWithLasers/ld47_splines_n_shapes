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

    private int curLevelIdx;

    private HashSet<int> targetsHitProperly = new HashSet<int>();
    private List<Target> curLevelTargets = new List<Target>();
    private int _curExpectedTargetIdx = 0;
    private int targetsHitProperlyCount;

    // Start is called before the first frame update
    void Awake()
    {
        // DO some game loaded shit

        // Load Level 0
        curLevelIdx = 0;
        LoadLevel(curLevelIdx);
        
    }

    private void LoadLevel(int idx)
    {
        if (idx < 0 || idx >= levels.Count)
        {
            throw new Exception("da fuck u fuck why u fuckin do dat");
        }

        var curLevel = levels[idx];

        for (var i = 0; i < curLevel.TargetPositions.Count; i++)
        {
            var targetPos = curLevel.TargetPositions[i];
            // construct hash set of targets that got hit?
            var targetGO = Instantiate(targetPrefab, targetPos, Quaternion.identity);

            targetGO.Construct(i, curLevel.TargetPositions.Count, curLevel);
            curLevelTargets.Add(targetGO);
        }

        _curExpectedTargetIdx = 0;
    }

    private void ClearCurrentLevel()
    {

        for (var i = curLevelTargets.Count-1; i >= 0; i--)
        {
            curLevelTargets[i].BeginDeathSentence();
        }

        curLevelTargets = new List<Target>();
        targetsHitProperly = new HashSet<int>();
        targetsHitProperlyCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerLooped()
    {
        return;

        var loopValid = targetsHitProperly.Count == curLevelTargets.Count;

        if (loopValid)
        {
            var semiToneDiff = targetCollisionNotes.GetSemiDiffForLoopValid(curLevelTargets.Count);

            sfxManager.PlayLoopValidAt(semiToneDiff);


            // clear current level
            ClearCurrentLevel();

            // load next level (modulo tee hee)

            curLevelIdx = (curLevelIdx + 1) % levels.Count;

            LoadLevel(curLevelIdx);
        }
        else
        {

            sfxManager.PlayEnoughLoopsPassed();

            foreach (var target in curLevelTargets)
            {
                target.OnPlayerLoopedIncomplete();
            }

            _curExpectedTargetIdx = 0;
            targetsHitProperly = new HashSet<int>();
        }
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


                // clear current level
                ClearCurrentLevel();

                // load next level (modulo tee hee)

                curLevelIdx = (curLevelIdx + 1) % levels.Count;

                LoadLevel(curLevelIdx);
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

                target.OnIncorrectlyHit();
            }
        }
    }
}
