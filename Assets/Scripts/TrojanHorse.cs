using UnityEngine;

public class TrojanHorse : Enemy
{
    WaveManager waveManager;
    public int payloadCount;

    protected void Awake() => waveManager = FindObjectOfType<WaveManager>();

    public override void Die()
    {
        PayLoad();
        base.Die();
    }

    private void PayLoad()
    {
        for(int i = 0; i < payloadCount; i++)
        {
            Enemy newMinion = waveManager.SpawnEnemy(EnemyType.DDoSBug, currentTile);
            newMinion.transform.position += (Vector3)UnityEngine.Random.insideUnitCircle;
            newMinion.AdjustSpeed(UnityEngine.Random.Range(-0.1f, 0.1f));
            newMinion.pathingToNode = pathingToNode;
        }
    }
}