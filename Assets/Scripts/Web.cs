using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Web : MonoBehaviour
{
    // WaveManager waveManager;
    [SerializeField] private LineRenderer line;
    [SerializeField] private BoxCollider2D boxCollider;
    WebTower parentA;
    WebTower parentB;
    public List<WebTower> connectedTowers = new List<WebTower>();
    public float damage;

    WebEffect parentAEffect = WebEffect.None;
    WebEffect parentBEffect = WebEffect.None;

    public List<WebEffect> effects = new List<WebEffect>();

    public List<(Enemy, float)> thorns = new List<(Enemy, float)>();
    public float thornsProcTime;

    // private void Awake() => waveManager = GameObject.FindObjectOfType<WaveManager>();
    // private void Start() => waveManager.onWaveComplete += OnWaveComplete;
    // private void OnDestroy() => waveManager.onWaveComplete -= OnWaveComplete;

    private void Update()
    {
        float thornsDamage = damage / 3 * effects.Count(e => e == WebEffect.Thorns);

        for(int i = thorns.Count - 1; i >= 0; i--)
        {
            (Enemy enemy, float time) thorn = thorns[i];
            if(thorn.enemy == null)
                thorns.RemoveAt(i);
            else if(Time.timeSinceLevelLoad >= thorn.time)
            {
                thorns[i] = (thorn.enemy, Time.timeSinceLevelLoad + thornsProcTime);
                thorn.enemy.TakeDamage(thornsDamage);
            }
        }
    }

    private void OnWaveComplete(Wave wave)
    {
        // effects.Clear();
    }

    public void Connect(WebTower a, WebTower b, Transform aPoint, Transform bPoint)
    {
        parentA = a;
        parentB = b;
        Vector3 webToA = aPoint.position - transform.position;
        Vector3 webToB = bPoint.position - transform.position;
        line.SetPosition(0, webToA);
        line.SetPosition(1, webToB);
        connectedTowers.Add(a);
        connectedTowers.Add(b);

        // This hack sucks on diagonal lines, but it works for now.
        Bounds bounds = line.bounds;
        boxCollider.offset = bounds.center - transform.position;
        boxCollider.size = bounds.size;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(damage * (1 + (effects.Count(e => e == WebEffect.Wounding) * 3f)));
            if(effects.Any(e => e == WebEffect.Stun))
                enemy.Stun(damage / 5 * effects.Count(e => e == WebEffect.Stun));
            if(effects.Any(e => e == WebEffect.Slow))
                enemy.AdjustSpeed(-0.333f * effects.Count(e => e == WebEffect.Slow));
            if(effects.Any(e => e == WebEffect.DamageAmp))
                enemy.AdjustDamageTaken(damage * effects.Count(e => e == WebEffect.DamageAmp));
            if(effects.Any(e => e == WebEffect.Thorns) && !thorns.Any(t => t.Item1 == enemy))
            {
                float thornsDamage = damage / 3 * effects.Count(e => e == WebEffect.Thorns);
                thorns.Add((enemy, Time.timeSinceLevelLoad + thornsProcTime));
                enemy.TakeDamage(thornsDamage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if(effects.Any(e => e == WebEffect.Slow))
                enemy.AdjustSpeed(0.333f * effects.Count(e => e == WebEffect.Slow));
            if(effects.Any(e => e == WebEffect.DamageAmp))
                enemy.AdjustDamageTaken(-damage * effects.Count(e => e == WebEffect.DamageAmp));
            if(effects.Any(e => e == WebEffect.Thorns) && thorns.Any(t => t.Item1 == enemy))
            {
                for(int i = thorns.Count - 1; i >= 0; i--)
                {
                    if(thorns[i].Item1 == enemy)
                        thorns.RemoveAt(i);
                }
            }
        }
    }

    public void GainWebEffect(WebEffect gaining, WebTower source)
    {
        if(source == parentA && parentAEffect != WebEffect.None)
        {
            effects.Remove(parentAEffect);
            parentAEffect = WebEffect.None;
            line.startColor = GetWebEffectColor(WebEffect.None);
        }
        else if(source == parentB && parentBEffect != WebEffect.None)
        {
            effects.Remove(parentBEffect);
            parentBEffect = WebEffect.None;
            line.endColor = GetWebEffectColor(WebEffect.None);
        }

        effects.Add(gaining);

        if(source == parentA)
        {
            parentAEffect = gaining;
            line.startColor = GetWebEffectColor(gaining);
        }
        else if(source == parentB)
        {
            parentBEffect = gaining;
            line.endColor = GetWebEffectColor(gaining);
        }
    }

    public Color GetWebEffectColor(WebEffect effect) => effect switch{
        WebEffect.Slow => Color.green,
        WebEffect.Stun => Color.yellow,
        WebEffect.Thorns => Color.blue,
        WebEffect.Wounding => Color.red,
        WebEffect.DamageAmp => Color.cyan,
        _ => Color.white
    };
}