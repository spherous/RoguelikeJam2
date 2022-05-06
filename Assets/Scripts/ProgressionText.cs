using UnityEngine;
using TMPro;

public class ProgressionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GroupFader fader;
    [SerializeField] private WaveManager waveManager;

    public float displayTime;
    private float? hideAtTime;

    private void Awake()
    {
        waveManager.onWaveStart += OnWaveStart;
        waveManager.onWaveComplete += OnWaveComplete;
    }

    private void Update()
    {
        TryHide();
    }

    private void TryHide()
    {
        if(hideAtTime.HasValue && fader.visible && Time.timeSinceLevelLoad >= hideAtTime.Value)
        {
            hideAtTime = null;
            fader.FadeOut();
        }
    }

    private void OnWaveComplete(Wave wave)
    {
        TryShow();
        text.text = $"Wave {waveManager.currentWave + 1} Complete";
    }

    private void TryShow()
    {
        if(!fader.visible)
        {
            fader.FadeIn();
            hideAtTime = Time.timeSinceLevelLoad + displayTime;
        }
    }

    private void OnWaveStart(Wave wave)
    {
        TryShow();
        text.text = $"Wave {waveManager.currentWave + 1} Start";
    }
}