using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName= "SO/Level/LevelData")]
public class LevelData : ScriptableObject
{
    [ReorderableList]
    [SerializeField]
    private List<Vector2> targetPositions;
    public List<Vector2> TargetPositions => targetPositions;

    [SerializeField]
    private int numControlPoints;
    public int NumControlPoints => numControlPoints;
    
}
