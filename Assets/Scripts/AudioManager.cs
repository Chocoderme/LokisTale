using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    public Action<AudioState> OnAudioStateChange;
    public Action<AudioState> OnAudioStateChanged;

    public AudioClipInfos[] AudioClips;

    [HideInInspector]
    public AudioClipInfos currentClip;

    private AudioSource Speaker;
    public AudioClipInfos newClip;

    // Use this for initialization
    void Start () {
        Speaker = GetComponent<AudioSource>();
        Speaker.loop = true;
        if (AudioClips.Length > 0)
            Play(AudioClips[0]);
	}
	
    public void Play(AudioClipInfos clip)
    {
        currentClip = clip;
        Speaker.clip = currentClip.Clip;
        Speaker.Play();
    }

	// Update is called once per frame
	void Update () {
        if (!Speaker.isPlaying && newClip != null)
            ChangeState();
	}

    private void ChangeState()
    {
        if (newClip != null)
        {
            Play(newClip);
            Speaker.loop = true;
            newClip = null;
            if (OnAudioStateChanged != null)
                OnAudioStateChanged(currentClip.State);
        }
    }

    public void ChangeStateAfter(AudioState state)
    {
        if (state != currentClip.State)
        {
            var clip = AudioClipInfos.Find(AudioClips, state);
            if (clip != null)
            {
                newClip = clip;
                Speaker.loop = false;
                if (OnAudioStateChange != null)
                    OnAudioStateChange(state);
            }
            else
                Debug.LogError("Audio state not found");
        }
    }

    public bool hasQueue()
    {
        return newClip != null;
    }

    [Serializable]
    public class AudioClipInfos
    {
        public AudioClip Clip;
        public AudioState State;

        public static AudioClipInfos Find(AudioClipInfos[] clips, AudioState state)
        {
            foreach (var clip in clips)
            {
                if (clip.State == state)
                    return clip;
            }
            return null;
        }
    }

    public enum AudioState
    {
        WaveWaiting = 0,
        WaveWarning,
        WaveIn,
        WaveAction,
        WaveActionVariation,
        WaveActionEpic,
        WaveOut
    }
}
