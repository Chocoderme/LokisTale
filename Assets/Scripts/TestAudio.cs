using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour {

    AudioManager manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindObjectOfType<AudioManager>();
        manager.OnAudioStateChanged += NextTheme;
        Invoke("PlayNext", 2);
	}

    private void PlayNext()
    {
        if (manager.currentClip.State < AudioManager.AudioState.WaveOut)
            manager.ChangeStateAfter(manager.currentClip.State + 1);
        else
            manager.ChangeStateAfter(AudioManager.AudioState.WaveWaiting);
    }

    private void NextTheme(AudioManager.AudioState obj)
    {
        //Debug.Log("New theme just started:" + obj);
        if (manager.currentClip.State != AudioManager.AudioState.WaveWaiting)
            Invoke("PlayNext", 2);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
