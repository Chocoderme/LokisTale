using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class WaveDisplay : MonoBehaviour {

    WaveManager wm;
    TMPro.TextMeshProUGUI gui;

	// Use this for initialization
	void Start () {
        wm = FindObjectOfType<WaveManager>();
        gui = GetComponent<TMPro.TextMeshProUGUI>();

        if (wm != null)
        {
            wm.OnWavePrepare += DisplayWave;
        }
	}

    private void DisplayWave()
    {
        Display();
        gui.SetText("Wave: " + wm.GetCurrentWaveCount().ToString());
        Invoke("Hide", 2);
    }

    private void Display()
    {
        StartCoroutine(Fade(0, 1, 1));
    }

    private void Hide()
    {
        StartCoroutine(Fade(1, 0, 1));
    }

    IEnumerator Fade(float from, float to, float time)
    {
        float elapsed = 0f;
        var start = gui.color;
        var end = gui.color;
        start.a = from;
        end.a = to;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            if (elapsed > time)
                elapsed = time;
            gui.color = Color.Lerp(start, end, elapsed);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
