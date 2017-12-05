using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    static GameManager instance;

    WaveManager WaveManager;
    AudioManager AudioManager;
    Shoot PlayerShoot;
    CharacterControllerLegacy PlayerController;
    WeaponSelectionDisplay weaponDisplay;
    public Image TransitionImage;
    public float EndGameTransition = 1.0f;
    public float FirstWaveRestTime = 5f;
    public float WaveRestTime = 3f;

    [HideInInspector]
    public int mScore = 0;

    public static GameManager GetInstance()
    {
        return instance;
    }

	void Awake () {
		if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Several instance of Game Manager");
            Destroy(this);
        }

        WaveManager = GetComponent<WaveManager>();
        AudioManager = FindObjectOfType<AudioManager>();
        PlayerShoot = FindObjectOfType<Shoot>();
        PlayerController = FindObjectOfType<CharacterControllerLegacy>();
        weaponDisplay = FindObjectOfType<WeaponSelectionDisplay>();
        if (WaveManager == null)
            Debug.LogError("No Wave Manager");
        if (AudioManager == null)
            Debug.LogError("No Audio Manager");
        if (PlayerShoot == null)
            Debug.LogError("No Shoot Script for player");
        if (PlayerController == null)
            Debug.LogError("No PlayerController Script for player");
        if (weaponDisplay == null)
            Debug.LogError("Couldn't find weapon selection display");
    }

    private void Start()
    {

        if (!WaveManager)
            return;

        WaveManager.OnWaveReady += WaveReady;
        WaveManager.OnWaveFinished += WaveFinished;

        AudioManager.OnAudioStateChanged += AudioChanged;

        Debug.Log("Spawning first wave in " + FirstWaveRestTime + " seconds");
        Invoke("PrepareNextWave", FirstWaveRestTime);
        PlayerShoot.SelectNewWeapon();
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player)
             Player.GetComponent<Health>().OnDie += GoBackMenu;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            GoBackMenu();
    }

    private void GoBackMenu()
    {
        StartCoroutine(Transition(EndGameTransition));
    }

    private IEnumerator Transition(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            TransitionImage.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Menu");
    }

    private void AudioChanged(AudioManager.AudioState currentState)
    {
        /* Loop for combat music*/
        if (currentState == AudioManager.AudioState.WaveAction && !AudioManager.hasQueue())
            AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveActionEpic);
        else if (currentState == AudioManager.AudioState.WaveActionEpic && !AudioManager.hasQueue())
            AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveAction);

        /* Logic based on music */
        if (currentState == AudioManager.AudioState.WaveWaiting)
        {
            PlayerShoot.SelectNewWeapon();
            Invoke("PrepareNextWave", WaveRestTime);
        }
        else if (currentState == AudioManager.AudioState.WaveWarning)
            WaveManager.PrepareNextWave();
        else if (currentState == AudioManager.AudioState.WaveIn)
        {
            AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveActionEpic);
            LaunchWave();
        }
        else if (currentState == AudioManager.AudioState.WaveOut)
            AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveWaiting);
    }

    private void WaveReady()
    {
        Debug.Log("New wave ready");
        AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveIn);
        PlayerController.preventActions = true;
        var buid = 0;
        for (var i = 0; i < PlayerShoot.Bullets.Length; ++i)
        {
            if (PlayerShoot.Bullets[i] == PlayerShoot.currentBullet)
            {
                buid = i;
                break;
            }
        }
        weaponDisplay.ShowDisplay(buid);
        // beautiful code
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void WaveFinished()
    {
        Debug.Log("Wave finished.. launching new wave in " + WaveRestTime + " seconds");
        AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveWaiting);
        PlayerShoot.FireOnMouseClick = false;
    }

    private void PrepareNextWave()
    {
        Debug.Log("Preparing next wave");
        AudioManager.ChangeStateAfter(AudioManager.AudioState.WaveWarning);
    }

    private void LaunchWave()
    {
        Debug.Log("Launching wave");
        WaveManager.LaunchWave();
        PlayerShoot.FireOnMouseClick = true;
        PlayerController.preventActions = false;
    }
}
