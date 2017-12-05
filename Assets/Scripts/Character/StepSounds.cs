using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StepSounds : MonoBehaviour {

    public AudioClip[] Clips;
    AudioSource Speaker;

    CharacterControllerLegacy CC;

    // Use this for initialization
    void Start () {
        Speaker = GetComponent<AudioSource>();
        CC = FindObjectOfType<CharacterControllerLegacy>();
        if (CC == null)
            Debug.LogError("Couldn't find player Character Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (CC.IsGrounded() && CC.isMoving() && !Speaker.isPlaying)
        {
            Speaker.clip = Clips[Random.Range(0, Clips.Length)];
            Speaker.Play();
        }
	}
}
