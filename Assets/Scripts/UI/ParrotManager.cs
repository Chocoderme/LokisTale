using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotManager : MonoBehaviour {

    public EasyTween mParrotSpawning;
    public EasyTween mParrotIdle;
    public EasyTween mCredit;
    public EasyTween mControl;

    public AudioClip[] ParrotSound;
    public AudioSource audioSource;


    bool isVisible = false;
    bool isCreditVisible = false;
    bool isControlVisible = false;
    string CurrentMenu = "";
    IEnumerator coroutinePlaySound;

    private void Start()
    {
        coroutinePlaySound = PlayRandomSound();
    }

    IEnumerator PlayRandomSound()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = ParrotSound[Random.Range(0, ParrotSound.Length - 1)];
                audioSource.loop = false;
                audioSource.Play();
                mParrotIdle.OpenCloseObjectAnimation();
            }
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void Show()
    {
        if (!isVisible)
        {
            isVisible = true;
            mParrotSpawning.OpenCloseObjectAnimation();
            StartCoroutine(coroutinePlaySound);
        }
    }

    public void Hide()
    {
        if (isVisible)
        {
            isVisible = false;
            mParrotSpawning.OpenCloseObjectAnimation();
            StopCoroutine(coroutinePlaySound);
        }
    }

    public void Toggle(string AskedMenu)
    {
        if (!isVisible)
            Show();
        else if (AskedMenu == CurrentMenu)
            Hide();

        CurrentMenu = AskedMenu;
        if (AskedMenu == "Control")
        {
            isControlVisible = !isControlVisible;
            mControl.OpenCloseObjectAnimation();
            if (isCreditVisible)
            {
                mCredit.OpenCloseObjectAnimation();
                isCreditVisible = false;
            }
        }
        else if (AskedMenu == "Credit")
        {
            isCreditVisible = !isCreditVisible;
            mCredit.OpenCloseObjectAnimation();
            if (isControlVisible)
            {
                mControl.OpenCloseObjectAnimation();
                isControlVisible = false;
            }
        }
    }
}
