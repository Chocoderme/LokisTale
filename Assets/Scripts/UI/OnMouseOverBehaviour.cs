using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseOverBehaviour : MonoBehaviour {

    public AudioClip clip;
    public EasyTween[] Animations;
    public AudioSource source;


    bool isMouseOver = false;

    void OnMouseOver()
    {
        if (!isMouseOver)
        {
            OpenAll();
            isMouseOver = true;
            source.clip = clip;
            source.Play();
            source.loop = false;
        }
    }

    void OnMouseExit()
    {
        if (isMouseOver)
        {
            OpenAll();
            isMouseOver = false;
        }
    }

    void OpenAll()
    {
        foreach (EasyTween anim in Animations)
        {
            anim.OpenCloseObjectAnimation();
        }
    }
}
