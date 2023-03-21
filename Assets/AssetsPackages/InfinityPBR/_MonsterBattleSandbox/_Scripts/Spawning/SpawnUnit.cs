using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnit : MonoBehaviour
{
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private Transform battleReadyPoint = null;
    [SerializeField] private SpawnUIManager spawnUIManager;
    GameObject unitPrefab = null;

    public void SpawnUnitOnButtonPress()
    {
        unitPrefab = spawnUIManager.currentCharacterSO.unitPrefab;
        GameObject newUnitGameObject = Instantiate(
            unitPrefab,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);

        Unit unit = newUnitGameObject.GetComponent<Unit>();
        unit.SetDestination(battleReadyPoint.position);
    }
}
