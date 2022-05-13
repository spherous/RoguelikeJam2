using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorm : Enemy
{
    WaveManager waveManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public List<Sprite> stage1Walk = new List<Sprite>();
    public List<Sprite> stage2Walk = new List<Sprite>();
    public List<Sprite> stage3Walk = new List<Sprite>();

    int stage;
    public int framesBetweenSteps;
    int framesSinceStep = 0;
    int stepIndex = 0;

    public List<float> shrinkThresholds = new List<float>(){.5f, .25f};
    // public new int scoreValue => 4 * (stage + 1);
    // public new int damage => 4 * (stage + 1);

    private void Awake()
    {
        damage = 4 * (3 - stage);
        scoreValue = 4 * (3 - stage);
        waveManager = GameObject.FindObjectOfType<WaveManager>();
    }
    protected override void Start()
    {
        base.Start();
        onHealthChanged += CheckForShrink;
    }
    private void OnDestroy() 
    {
        onHealthChanged -= CheckForShrink;
    }

    private void Update()
    {
        if(framesSinceStep > framesBetweenSteps)
        {
            List<Sprite> walk = GetStageWalk(stage);
            if(walk != null)
            {
                stepIndex = stepIndex + 1 > walk.Count - 1 ? 0 : stepIndex + 1;
                spriteRenderer.sprite = walk[stepIndex];
            }
            framesSinceStep = 0;
        }
        else
            framesSinceStep++;
    }

    private List<Sprite> GetStageWalk(int stage) => stage switch{
        0 => stage1Walk,
        1 => stage2Walk,
        2 => stage3Walk,
        _ => null
    };

    public void SetStage(int stage)
    {
        this.stage = stage;
        stepIndex = 0;
        spriteRenderer.sprite = GetStageWalk(stage)[0];
        framesSinceStep = 0;
        damage = 4 * (3 - stage);
        scoreValue = 4 * (3 - stage);
    }

    private void CheckForShrink(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(stage >= 2 || waveManager == null)
            return;

        if(percent < shrinkThresholds[stage])
        {
            NetWorm newWorm = (NetWorm)waveManager.SpawnEnemy(EnemyType.NetWorm, currentTile);
            stage++;
            newWorm.SetStage(stage);
            SetStage(stage);
            newWorm.transform.position += (Vector3)UnityEngine.Random.insideUnitCircle;
            newWorm.AdjustSpeed(UnityEngine.Random.Range(-0.1f, 0.1f));
            newWorm.pathingToNode = pathingToNode;
        }
    }
}