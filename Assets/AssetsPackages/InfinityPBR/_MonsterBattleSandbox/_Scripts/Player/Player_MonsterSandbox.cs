using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Keeps a list of Units*/

public class Player_MonsterSandbox : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform = null;

    private Color teamColor = new Color();
    [SerializeField] private List<Unit> myUnits = new List<Unit>();
    
    #region Getters
    public Transform GetCameraTransform() => cameraTransform;
    public Color GetTeamColor() => teamColor;
    public List<Unit> GetMyUnits() => myUnits;
    #endregion

    private void OnEnable()
    {
        Unit.OnUnitSpawned += HandleUnitSpawned;
        Unit.OnUnitDespawned += HandleUnitDespawned;
    }

    public void OnDestroy()
    {
        Unit.OnUnitSpawned -= HandleUnitSpawned;
        Unit.OnUnitDespawned -= HandleUnitDespawned;
    }

    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    private void HandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
        Debug.Log(unit.name + " spawned");
    }

    private void HandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }
}