using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour {

    public Action OnWavePrepare;
    public Action OnWaveReady;
    public Action OnWaveStarted;
    public Action OnWaveFinished;

    [SerializeField] Transform[] SpawningPointsTransform;
    public Wave[] Waves;

    private int waveIndex = -1;
    private int waveCount = 0;
    private bool isPreparingWave = false;
    private bool hasWaveStarted = false;
    private List<Vector3> SpawningPoints = new List<Vector3>();

	// Use this for initialization
	void Start () {
		foreach (Transform spawnTransform in SpawningPointsTransform) {
            SpawningPoints.Add(spawnTransform.position);
        }
        //Debug.Log("There are " + SpawningPoints.Count + " spawns for this map");
	}

    public void PrepareNextWave()
    {
        waveIndex++;
        waveCount++;
        if (OnWavePrepare != null)
            OnWavePrepare();
        if (waveIndex >= Waves.Length)
            waveIndex = Waves.Length - 1;
        Waves[waveIndex].Spawn(SpawningPoints);
        isPreparingWave = true;
    }

	// Update is called once per frame
	void Update () {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (isPreparingWave)
        {
            bool waveReady = true;
            foreach (var enemy in enemies)
            {
                AIBrain brain = enemy.GetComponent<AIBrain>();
                if (brain != null && !brain.isReady)
                    waveReady = false;
            }
            if (waveReady)
            {
                isPreparingWave = false;
                if (OnWaveReady != null)
                    OnWaveReady();
            }
        }
        if (enemies.Length <= 0 && hasWaveStarted)
            EndWave();
	}

    public void EndWave(bool destroyEnemies = false)
    {
        if (destroyEnemies)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (var i = enemies.Length - 1; i >= 0; --i)
            {
                var enemy = enemies[i];
                Destroy(enemy);
            }
        }
        hasWaveStarted = false;
        if (OnWaveFinished != null)
            OnWaveFinished();
        Debug.Log("The Wave is destroyed");
    }

    public void LaunchWave()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            AIBrain brain = enemy.GetComponent<AIBrain>();
            brain.isActive = true;
        }
        hasWaveStarted = true;

        if (OnWaveStarted != null)
            OnWaveStarted();
    }

	public int GetCurrentWaveIndex()
    {
		return waveIndex;
	}

    public int GetCurrentWaveCount()
    {
        return waveCount;
    }

    public bool isWaveStarted()
    {
        return hasWaveStarted;
    }

    [Serializable]
    public class Wave
    {
        public Enemy[] Enemies;

        public void Spawn()
		{
            foreach (var enemy in Enemies)
                enemy.Spawn();
        }

        public void Spawn(List<Vector3> spawns)
        {
            // Debug.Log("Spawn enemies with spawner");
            foreach (var enemy in Enemies)
            {
				int i = UnityEngine.Random.Range (0, spawns.Count - 1);

				//Debug.Log (i);

                enemy.SpawnPoint = spawns[i];
                enemy.Spawn();
            }
        }
    }

    [Serializable]
    public class Enemy
    {
        public GameObject Model;

		[HideInInspector]
        public Vector3 SpawnPoint;
    
        public void Spawn()
        {
			//Debug.Log (SpawnPoint);
            Instantiate(Model, SpawnPoint, Quaternion.identity);
        }
    }
}
