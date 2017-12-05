using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Health))]
public class SoundOnDamage : MonoBehaviour {

    private AudioSource Speaker;
    private Health health;

    public SoundInfo[] Clips;

	// Use this for initialization
	void Start () {
        if (Clips.Length > 0)
        {
            Speaker = GetComponent<AudioSource>();
            health = GetComponent<Health>();
            Speaker.loop = false;

            health.OnDamaged += PlayDamagedSound;
        }
	}

    private void PlayDamagedSound(int damageCount, Health.HealthInfo info)
    {
        var sound = Clips[UnityEngine.Random.Range(0, Clips.Length)];
        Speaker.clip = sound.Clip;
        Speaker.pitch = UnityEngine.Random.Range(sound.PitchRange.x, sound.PitchRange.y);
        Speaker.volume = UnityEngine.Random.Range(sound.VolumeRange.x, sound.VolumeRange.y);
        Speaker.Play();
    }

    [Serializable]
    public struct SoundInfo
    {
        public AudioClip Clip;
        public Vector2 PitchRange;
        public Vector2 VolumeRange;
    }
}
