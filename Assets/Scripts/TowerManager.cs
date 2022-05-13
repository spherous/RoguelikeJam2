using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private ChainTower chainTowerPrefab;
    [SerializeField] private WebTower webTowerPrefab;
    [SerializeField] private AOETower aoeTowerPrefab;
    [SerializeField] private LongRangeCannonTower longRangeCannonTowerPrefab;
    WaveManager waveManager;
    public List<ITower> towers {get; private set;} = new List<ITower>();
    public float rangeAdjustment;
    public float damageAdjustment;
    public float attackSpeedAdjustment;

    private void Awake()
    {
        waveManager = GameObject.FindObjectOfType<WaveManager>();
    }

    private void Start()
    {
        waveManager.onWaveComplete += OnWaveComplete;    
    }
    
    private void OnDestroy()
    {
        waveManager.onWaveComplete -= OnWaveComplete;
    }

    public ITower SpawnTower(TowerType type, Tile location)
    {
        ITower toSpawn = GetTowerPrefab(type);
        if(toSpawn == null)
            return null;
        ITower tower = Instantiate((MonoBehaviour)toSpawn, location.transform.position, Quaternion.identity, location.transform).GetComponent<ITower>();
        towers.Add(tower);
        tower.AdjustRange(rangeAdjustment);
        tower.AdjustDamage(damageAdjustment);
        tower.AdjustAttackSpeed(attackSpeedAdjustment);
        return tower;
    }

    public ITower GetTowerPrefab(TowerType type) => type switch {
        TowerType.Chain => chainTowerPrefab,
        TowerType.Web => webTowerPrefab,
        TowerType.AOE => aoeTowerPrefab,
        TowerType.Range => longRangeCannonTowerPrefab,
        _ => null
    };

    private void OnWaveComplete(Wave wave)
    {
        if(rangeAdjustment != 0)
            rangeAdjustment = 0;
        if(damageAdjustment != 0)
            damageAdjustment = 0;
        if(attackSpeedAdjustment != 0)
            attackSpeedAdjustment = 0;
    }

    public void AdjustTowerRange(float percent)
    {
        rangeAdjustment = percent;
        foreach(ITower tower in towers)
            tower.AdjustRange(percent);
    }

    public void AdjustTowerDamage(float percent)
    {
        damageAdjustment = percent;
        foreach(ITower tower in towers)
            tower.AdjustDamage(percent);
    }

    internal void AdjustTowerAttackSpeed(float percent)
    {
        attackSpeedAdjustment = percent;
        foreach(ITower tower in towers)
            tower.AdjustAttackSpeed(percent);
    }
}