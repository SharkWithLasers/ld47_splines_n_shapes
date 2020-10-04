using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(menuName = "SO/Target/Audio/CollisionNotes")]
public class TargetCollisionNotes : ScriptableObject
{

    // TODO THERE HSOULD BE A CHECK ON ALL OF THESE SO THEY HAVE
    // n+1 VALUES EACH! THE LAST VALUE IS FOR THE GOAL SOUND

    [SerializeField]
    private List<int> oneTargetSemis;

    [SerializeField]
    private List<int> twoTargetSemis;

    [SerializeField]
    private List<int> threeTargetSemis;

    [SerializeField]
    private List<int> fourTargetSemis;

    [SerializeField]
    private List<int> fiveTargetSemis;

    [SerializeField]
    private List<int> sixTargetSemis;

    public int GetSemiDiffForLoopValid(int numTotalTargets)
    {
        switch (numTotalTargets)
        {
            case 1:
                return oneTargetSemis.Last();
            case 2:
                return twoTargetSemis.Last();
            case 3:
                return threeTargetSemis.Last();
            case 4:
                return fourTargetSemis.Last();
            case 5:
                return fiveTargetSemis.Last();
            case 6:
                return sixTargetSemis.Last();
            default:
                throw new ArgumentException($"unsupported numTotalTargets {numTotalTargets}");
        }
    }

    public int GetSemiDiffForLoopInvalid()
    {
        return -12;
    }

    public int GetSemiDiffForTarget(int targetIdx, int numTotalTargets)
    {
        if (targetIdx < 0 || targetIdx >= numTotalTargets)
        {
            throw new ArgumentException(
                $"targetIdx {targetIdx} must be within numTotalTargets {numTotalTargets}"
                );
        }

        switch (numTotalTargets)
        {
            case 1:
                return oneTargetSemis[targetIdx];
            case 2:
                return twoTargetSemis[targetIdx];
            case 3:
                return threeTargetSemis[targetIdx];
            case 4:
                return fourTargetSemis[targetIdx];
            case 5:
                return fiveTargetSemis[targetIdx];
            case 6:
                return sixTargetSemis[targetIdx];
            default:
                throw new ArgumentException($"unsupported numTotalTargets {numTotalTargets}");
        }
    }
}
