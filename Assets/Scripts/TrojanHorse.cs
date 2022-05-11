using UnityEngine;

public class TrojanHorse : Enemy
{
    WaveManager waveManager;
    public int payloadCount;

    protected void Awake() => waveManager = FindObjectOfType<WaveManager>();

    public new void Die()
    {
        PayLoad();
        base.Die();
    }

    private void PayLoad()
    {
        // waveManager
        // Spawn multiple DDoS Bugs
    }
}