using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    [SerializeField] private SpawnUIManager spawnUIManager;
    [SerializeField] private Transform monsterSpawnPoint = null;
    public List<Transform> waypointList;
    GameObject monsterPrefab;

    private void Awake()
    {
        MonsterStateController.OnMonsterSpawn += GiveWaypoints;
    }

    void Start()
    {

    }

    private void OnDisable()
    {
        MonsterStateController.OnMonsterSpawn -= GiveWaypoints;
    }

    public void SpawnMonsterOnButtonPress()
    {
        monsterPrefab = spawnUIManager.currentCharacterSO.monsterPrefab;
        GameObject newMonster = Instantiate(
            monsterPrefab,
            monsterSpawnPoint.position,
            monsterSpawnPoint.rotation);
    }

    private void GiveWaypoints(MonsterStateController monsterController)
    {
        monsterController.SetWaypoints(waypointList);
    }
}
