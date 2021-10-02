using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/New Level", order = 1)]
public class Level : ScriptableObject
{
    [Header("Starting Values")]
    public int money;
    public int happiness;
    public int winThreshold;
    public int loseThreshold;

    [Header("Spawn Points")]
    public Vector3[] buildingSpawnPositions;
    public Vector3[] generatorSpawnPositions;
    public Vector3[] pylonSpawnRotations;

    [Header("Options")]
    public bool canBuildGenerators;
    public bool canBuildPylons;
    public int maxBuildings = 50;
    public int maxGenerators = 5;
    public int maxPylons = 10;
}
