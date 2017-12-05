using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SunColor : MonoBehaviour {

    public Color NormalColor;
    public Color FightColor;

    private WaveManager wm;
    private Light lg;

	// Use this for initialization
	void Start () {
        wm = FindObjectOfType<WaveManager>();
        lg = GetComponent<Light>();
        if (wm != null)
        {
            wm.OnWaveReady += WaveStartChangeColor;
            wm.OnWaveFinished += WaveFinishChangeColor;
        }
	}

    private void WaveFinishChangeColor()
    {
        if (lg.color != NormalColor)
            StartCoroutine(ChangeColor(lg.color, NormalColor, 2f));
    }

    private void WaveStartChangeColor()
    {
        if (lg.color != FightColor)
            StartCoroutine(ChangeColor(lg.color, FightColor, 2f));
    }

    IEnumerator ChangeColor(Color start, Color end, float time)
    {

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            if (elapsed > time)
                elapsed = time;
            lg.color = Color.Lerp(start, end, elapsed / time);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
