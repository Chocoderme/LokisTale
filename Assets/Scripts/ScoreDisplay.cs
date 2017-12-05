using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class ScoreDisplay : MonoBehaviour {

    WaveManager wm;
    TMPro.TextMeshProUGUI gui;

    public int Score = 0;
    public int oldScore = -1;

    // Use this for initialization
    void Start()
    {
        wm = FindObjectOfType<WaveManager>();
        gui = GetComponent<TMPro.TextMeshProUGUI>();

        if (wm != null)
        {
            wm.OnWaveReady += SubscribeEnemies;
            wm.OnWaveFinished += WaveFinished;
        }
    }

    private void WaveFinished()
    {
        Score += 30;
    }

    private void SubscribeEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDie += EnemyDied;
            }
        }
    }

    private void EnemyDied()
    {
        Debug.Log("ENEMY DIEDDDDDDDD");
        var bullets = GameObject.FindGameObjectsWithTag("Bullet");
        Score += 20 - bullets.Length;
    }

    // Update is called once per frame
    void Update () {
		if (oldScore != Score)
        {
            gui.SetText("Score: " + Score);
            oldScore = Score;
            GameManager.GetInstance().mScore = Score;
        }
	}
}
